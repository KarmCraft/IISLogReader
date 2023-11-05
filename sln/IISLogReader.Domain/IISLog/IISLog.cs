namespace IISLogReader.Domain.IISLog;

public class IISLog
{
    public required DateTime Created { get; set; }

    public required List<LogEntry> LogEntries { get; set; }
}
