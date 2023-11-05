using IISLogReader.Domain.Common;
using IISLogReader.Domain.IISLog;
using System.Net;

namespace IISLogReader.Tests;

[TestClass]
public class IISLogTests
{
    [TestInitialize]
    public void Setup()
    {
    }

    [TestMethod]
    public void CanInstantiateIISLog()
    {
        var log = new IISLog { Created = DateTime.UtcNow, LogEntries = new List<LogEntry>() };
        Assert.IsNotNull(log);
    }

    [TestMethod]
    public void CanInstantiateIISLogEntry()
    {
        var logEntry = new LogEntry();
        Assert.IsNotNull(logEntry);
    }

    public void CanCreateLogFromLogLines()
    {
        var log = new IISLog { Created = DateTime.UtcNow, LogEntries = new List<LogEntry>() };

        var logLine1 = "2020-12-09 00:00:50 192.168.170.27 GET /ost/login.aspx - 80 - 192.168.160.131 check_http/v2.1.2+(monitoring-plugins+2.1.2) - 302 0 0 2";
        var logEntry1 = new LogEntry().Build(logLine1);
        log.LogEntries.Add(logEntry1);

        Assert.IsTrue(log.LogEntries.Count == 1);
        Assert.AreEqual(logEntry1.Date, DateTime.Parse("2020-12-09 00:00:50"));
        Assert.AreEqual(logEntry1.TimeTaken, 2);
        Assert.AreEqual(logEntry1.Client?.IP, IPAddress.Parse("192.168.160.131"));
        Assert.AreEqual(logEntry1.Server?.IP, IPAddress.Parse("192.168.170.27"));
        Assert.AreEqual(logEntry1.Server?.Port, 80);
        Assert.AreEqual(logEntry1.ClientToServer?.Method, "GET");
        Assert.AreEqual(logEntry1.ClientToServer?.UriStem, "/ost/login.aspx");
        Assert.AreEqual(logEntry1.ClientToServer?.UriQuery, "-");
        Assert.AreEqual(logEntry1.ClientToServer?.Username, "-");
        Assert.AreEqual(logEntry1.ClientToServer?.UserAgent, "check_http/v2.1.2+(monitoring-plugins+2.1.2)");
        Assert.AreEqual(logEntry1.ServerToClient?.Status, HttpStatusCode.Found);
        Assert.AreEqual(logEntry1.ServerToClient?.Substatus, LogEntry.SC.SubstatusCode._0);
        Assert.AreEqual(logEntry1.ServerToClient?.Win32Status, 0);

        var logLine2 = "2020-03-05 17:03:24 192.168.170.27 GET /public/plugins/grafana-clock-panel/module.js.map X-ARR-CACHE-HIT=0&X-ARR-LOG-ID=94661963-6d5f-4b3d-a00d-279537ea6029&SERVER-STATUS=200 80 - 192.168.174.12 Mozilla/5.0+(Windows+NT+10.0;+Win64;+x64)+AppleWebKit/537.36+(KHTML,+like+Gecko)+Chrome/80.0.3987.122+Safari/537.36 - 200 0 0 70";
        var logEntry2 = new LogEntry().Build(logLine2);
        log.LogEntries.Add(logEntry2);

        Assert.IsTrue(log.LogEntries.Count == 2);
        Assert.AreEqual(logEntry2.Date, DateTime.Parse("2020-03-05 17:03:24"));
        Assert.AreEqual(logEntry2.TimeTaken, 70);
        Assert.AreEqual(logEntry2.Client?.IP, IPAddress.Parse("192.168.174.12"));
        Assert.AreEqual(logEntry2.Server?.IP, IPAddress.Parse("192.168.170.27"));
        Assert.AreEqual(logEntry2.Server?.Port, 80);
        Assert.AreEqual(logEntry2.ClientToServer?.Method, "GET");
        Assert.AreEqual(logEntry2.ClientToServer?.UriStem, "/public/plugins/grafana-clock-panel/module.js.map");
        Assert.AreEqual(logEntry2.ClientToServer?.UriQuery, "X-ARR-CACHE-HIT=0&X-ARR-LOG-ID=94661963-6d5f-4b3d-a00d-279537ea6029&SERVER-STATUS=200");
        Assert.AreEqual(logEntry2.ClientToServer?.Username, "-");
        Assert.AreEqual(logEntry2.ClientToServer?.UserAgent, "Mozilla/5.0+(Windows+NT+10.0;+Win64;+x64)+AppleWebKit/537.36+(KHTML,+like+Gecko)+Chrome/80.0.3987.122+Safari/537.36");
        Assert.AreEqual(logEntry2.ServerToClient?.Status, HttpStatusCode.OK);
        Assert.AreEqual(logEntry2.ServerToClient?.Substatus, LogEntry.SC.SubstatusCode._0);
        Assert.AreEqual(logEntry2.ServerToClient?.Win32Status, 0);
    }

    [TestMethod]
    [ExpectedException(typeof(LogParseException), Constants.InvalidLogLineExMsg)]

    public void ThrowsOnBusinessRuleError()
    {
        var logLine1 = "2020-12-03 00:04:50 192.168.170.27 GET /ost/login.aspx - 100000 - 192.168.160.131 check_http/v2.1.2+(monitoring-plugins+2.1.2) - 302 1 0 2";
        _ = new LogEntry().Build(logLine1);
    }

    [TestMethod]
    [ExpectedException(typeof(LogParseException), Constants.FieldDefinitionExMsg)]
    public void ThrowsOnInvalidFieldDefinition()
    {
        _ = new LogEntry().Build(string.Empty);
    }
}
