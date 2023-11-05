using System.Net;

namespace IISLogReader.Domain.IISLog;

public class LogEntryDetails
{
    public IPAddress ClientIpAddress { get; set; }
    public string ClientFqdn { get; set; }
    public int HitCount { get; set; }
}