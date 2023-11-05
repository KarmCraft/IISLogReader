namespace IISLogReader.Domain.IISLog;
internal static class LogEntryExtensions
{
    public static Uri CalcFqdn(this LogEntry logEntry)
    {
        if (logEntry == null)
        {
            throw new ArgumentNullException(nameof(logEntry));
        }

        return new UriBuilder
        {
            Scheme = "http",
            Host = logEntry.Server.IP.ToString(),
            Port = logEntry.Server.Port,
            Path = logEntry.ClientToServer.UriStem.ToString(),
            Query = logEntry.ClientToServer.UriQuery?.ToString()
        }.Uri;
    }
}
