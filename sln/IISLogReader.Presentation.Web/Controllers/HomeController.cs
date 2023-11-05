using IISLogReader.App;
using IISLogReader.App.Abstractions;
using IISLogReader.Domain.IISLog;
using IISLogReader.Presentation.Web.Common;
using IISLogReader.Presentation.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using static IISLogReader.Domain.IISLog.LogEntry;

namespace IISLogReader.Presentation.Web.Controllers;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IIISLogService _logService;
    private LogViewModel _vm = new LogViewModel();
    private IISLog _currentLog;
    private readonly IMemoryCache _cache;

    public HomeController(ILogger<HomeController> logger, IIISLogService logService, IMemoryCache cache)
    {
        _logger = logger;
        _logService = logService;
        _cache = cache;
    }

    public IActionResult Index()
    {
        try
        {
            if (TempData["AlertMessage"] != null)
            {
                _vm.AlertMessage = TempData["AlertMessage"]?.ToString();
                var alertType = TempData["AlertType"] ?? AlertTypeEnum.Info;
                _vm.AlertType = Enum.Parse<AlertTypeEnum>(alertType.ToString());
            }

            if (TempData["CurrentLogCacheKey"] is string cacheKey && _cache.TryGetValue(cacheKey, out IISLog cachedLog))
            {
                _vm.Log = cachedLog;
                _vm.FileName = TempData["CurrentLogFileName"]?.ToString();
                var uploaded = TempData["CurrentLogUploaded"] == null ? DateTime.MinValue.ToString() : TempData["CurrentLogUploaded"]?.ToString();
                _vm.Uploaded = DateTime.Parse(uploaded!);

                if (TempData["CurrentLogDetailsCacheKey"] is string detailsCacheKey && _cache.TryGetValue(detailsCacheKey, out List<LogEntryDetails> logSummary))
                {
                    _vm.LogEntriesSummary = logSummary;
                }
            }

            return View(_vm);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while loading index page.");
            throw;
        }
    }

    [HttpPost()]
    public async Task<IActionResult> Upload(IFormFile logFile)
    {
        try
        {
            if (logFile == null || logFile.Length == 0)
                throw new Exception("No file selected.");
            if (!logFile.FileName.EndsWith(".log"))
                throw new Exception("Invalid file type. Make sure to upload .log files.");

            TempData["CurrentLogFileName"] = logFile.FileName;
            TempData["CurrentLogUploaded"] = DateTime.UtcNow;

            MemoryStream currentLog;

            using (currentLog = new MemoryStream())
            {
                await logFile.CopyToAsync(currentLog);
                _currentLog = await _logService.ParseLogAsync(currentLog);

                var cacheKey = "CurrentLog_" + Guid.NewGuid().ToString();
                _cache.Set(cacheKey, _currentLog, TimeSpan.FromMinutes(10));

                var cacheKeyDetails = "CurrentLogDetails_" + Guid.NewGuid().ToString();
                var logDetailsList = new List<LogEntryDetails>();

                await foreach (var detail in _logService.GetLogEntriesSummaryAsync(_currentLog))
                {
                    logDetailsList.Add(detail);
                }
                _cache.Set(cacheKeyDetails, logDetailsList, TimeSpan.FromMinutes(10));

                TempData["CurrentLogDetailsCacheKey"] = cacheKeyDetails;
                TempData["CurrentLogCacheKey"] = cacheKey;
                TempData["AlertMessage"] = "Upload OK";
                TempData["AlertType"] = AlertTypeEnum.Info;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while uploading file.");
            TempData["AlertMessage"] = ex.Message;
            TempData["AlertType"] = AlertTypeEnum.Error;
        }

        return RedirectToAction("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
