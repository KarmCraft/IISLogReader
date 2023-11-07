using IISLogReader.Domain.IISLog;

namespace IISLogReader.Presentation.Web.Models;

public class LogViewModel : ViewModelBase
{
    public string? FileName { get; set; }
    public DateTime? Uploaded { get; set; }
    public IISLog? Log { get; set; }
    public List<LogEntrySummary> LogEntriesSummary { get; set; }
}
