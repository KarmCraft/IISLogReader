using IISLogReader.App.Abstractions;
using IISLogReader.Domain.Common;
using IISLogReader.Domain.IISLog;
using System.Net.Sockets;
using System.Net;
using Microsoft.Extensions.Logging;

namespace IISLogReader.App;

public class IISLogService : IIISLogService
{
    private readonly ILogger<IISLogService> _logger;

    private char[] _splitChars = Constants.LogSplitChars;

    public IISLogService(ILogger<IISLogService> logger)
    {
        _logger = logger;
    }

    public IISLog? CurrentLog { get; private set; }

    public async Task<IISLog> ParseLogAsync(Stream logStream)
    {
        IISLog iisLog;
        DateTime created = DateTime.MinValue;
        var logEntries = new List<LogEntry>();

        using (var reader = new StreamReader(logStream))
        {
            reader.DiscardBufferedData();
            reader.BaseStream.Seek(0, SeekOrigin.Begin);

            while (!reader.EndOfStream)
            {
                var logLine = await reader.ReadLineAsync();

                if (logLine == null) continue;

                var fieldsFromLogHeader = new List<string>();
                if (logLine.StartsWith(Constants.FieldsPrefix) && !fieldsFromLogHeader.Any()) // Avoid re-reading of field names
                {
                    logLine
                        .Split(_splitChars)
                        .Skip(1)
                        .ToList()
                        .ForEach(fieldsFromLogHeader.Add);

                    if (fieldsFromLogHeader.Count != Constants.OrderedFieldNames.Count)
                        throw new LogParseException(Constants.FieldDefinitionExMsg);
                }
                else if (logLine.StartsWith(Constants.DatePrefix) && created == DateTime.MinValue) // Avoid re-reading of date
                {
                    if (!DateTime.TryParse(string.Join(_splitChars.First().ToString(), logLine.Split(_splitChars).Skip(1)), out created))
                        throw new LogParseException(Constants.InvalidDateFormatExMsg);
                }
                else if (!logLine.StartsWith(Constants.LogCommentPrefix))
                {
                    var logEntry = new LogEntry().Build(logLine);
                    logEntries.Add(logEntry);
                }
            }

            CurrentLog = new IISLog
            {
                Created = created,
                LogEntries = logEntries
            };
        }

        return CurrentLog;
    }

    public async IAsyncEnumerable<LogEntryDetails> GetLogEntriesSummaryAsync(IISLog log)
    {
        var query = log.LogEntries
            .GroupBy(entry => entry.Client?.IP)
            .Select(group => new LogEntryDetails
            {
                ClientIpAddress = group.Key,
                ClientFqdn = "Internal",
                HitCount = group.Count()
            });

        foreach (var item in query)
        {
            string fqdn;
            try
            {
                if (item.ClientIpAddress.ToString().StartsWith("192."))
                {
                    fqdn = "Internal";
                }
                else
                {
                    fqdn = await FetchFqdnAsync(item.ClientIpAddress);
                }
            }
            catch (SocketException ex)
            {
                fqdn = "Unknown";
                _logger.LogError(ex, "Unknown IP {IP}", item.ClientIpAddress);
            }
            catch (Exception ex)
            {
                fqdn = "Lookup failed";
                _logger.LogError(ex, "Lookup failed {IP}", item.ClientIpAddress);
            }

            yield return new LogEntryDetails
            {
                ClientIpAddress = item.ClientIpAddress,
                ClientFqdn = fqdn,
                HitCount = item.HitCount
            };
        }
    }

    private async Task<string> FetchFqdnAsync(IPAddress ipAddress)
    {
        IPHostEntry hostEntry = await Dns.GetHostEntryAsync(ipAddress);
        return hostEntry.HostName;
    }
}
