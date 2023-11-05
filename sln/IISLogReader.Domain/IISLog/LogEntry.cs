using IISLogReader.Domain.Common;
using System.ComponentModel;
using System.Net;

namespace IISLogReader.Domain.IISLog;

public record LogEntry
{
    private char[] _splitChars = Constants.LogSplitChars;
    private readonly LogEntryValidator _logEntryValidator = new();
    private readonly Dictionary<string, string?> _fieldValuesDict = new();

    public DateTime Date { get; set; }
    public int TimeTaken { get; set; }
    public C? Client { get; set; }
    public S? Server { get; set; }
    public CS? ClientToServer { get; set; }
    public SC? ServerToClient { get; set; }

    public LogEntry()
    {
        Constants.OrderedFieldNames.ToList().ForEach(fn => _fieldValuesDict.Add(fn, default));
    }

    public LogEntry Build(string logLine)
    {
        var lineValues = logLine.Split(_splitChars).ToList();

        if (_fieldValuesDict.Count != lineValues.Count)
            throw new LogParseException(Constants.FieldDefinitionExMsg);

        Constants.OrderedFieldNames.ForEach(fn =>
        {
            _fieldValuesDict[fn] = lineValues[Constants.OrderedFieldNames.IndexOf(fn)];
        });

        ParseLogLine();

        if (!_logEntryValidator.Validate(this).IsValid)
            throw new LogParseException(Constants.InvalidLogLineExMsg);

        return this;
    }

    private void ParseLogLine()
    {
        _ = DateOnly.TryParse(_fieldValuesDict[Constants.FieldDate], out var parsedDate);
        _ = TimeOnly.TryParse(_fieldValuesDict[Constants.FieldTime], out var parsedTime);
        _ = int.TryParse(_fieldValuesDict[Constants.FieldSPort], out var parsedPort);
        _ = int.TryParse(_fieldValuesDict[Constants.FieldWin32Status], out var parsedScWin32Status);
        _ = int.TryParse(_fieldValuesDict[Constants.FieldTimeTaken], out var parsedTimeTaken);
        var parsedStatus = Enum.Parse<HttpStatusCode>(_fieldValuesDict[Constants.FieldStatus]);
        var tmpSubStatus = _fieldValuesDict[Constants.FieldSubstatus];
        var subStatus = Enum.Parse<SC.SubstatusCode>(int.TryParse(tmpSubStatus, out _) ? $"_{tmpSubStatus}" : tmpSubStatus);
        var parsedSIP = IPAddress.Parse(_fieldValuesDict[Constants.FieldSIP]);
        var parsedCIP = IPAddress.Parse(_fieldValuesDict[Constants.FieldCIP]);


        Date = parsedDate.ToDateTime(parsedTime);
        TimeTaken = parsedTimeTaken;
        Server = new S
        {
            IP = parsedSIP,
            Port = parsedPort,
        };
        ClientToServer = new CS
        {
            Method = new HttpMethod(_fieldValuesDict[Constants.FieldMethod]),
            UriStem = _fieldValuesDict[Constants.FieldUriStem],
            UriQuery = _fieldValuesDict[Constants.FieldUriQuery],
            UserAgent = _fieldValuesDict[Constants.FieldUserAgent],
            Referer = _fieldValuesDict[Constants.FieldReferer],
            Username = _fieldValuesDict[Constants.FieldUsername],
        };
        ServerToClient = new SC
        {
            Status = parsedStatus,
            Substatus = subStatus,
            Win32Status = parsedScWin32Status
        };
        Client = new C
        {
            IP = parsedCIP
        };
    }

    public record C
    {
        public required IPAddress? IP { get; set; }
    }

    public record S
    {
        public required IPAddress? IP { get; set; }
        public required int Port { get; set; }
    }

    public record CS
    {
        public required HttpMethod Method { get; set; }
        public required string UriStem { get; set; }
        public string? UriQuery { get; set; }
        public string? Username { get; set; }
        public required string UserAgent { get; set; }
        public string? Referer { get; set; }
    }

    public record SC
    {
        public required HttpStatusCode Status { get; set; }
        public required SubstatusCode? Substatus { get; set; }
        public required int Win32Status { get; set; }

        public enum SubstatusCode
        {
            [Description("-")]
            _None,
            [Description("0")]
            _0,
            [Description("1")]
            _1,
            [Description("2")]
            _2,
            [Description("3")]
            _3,
            [Description("4")]
            _4,
            [Description("5")]
            _5,
            [Description("6")]
            _6,
            [Description("7")]
            _7,
            [Description("8")]
            _8,
            [Description("9")]
            _9,
            [Description("10")]
            _10,
            [Description("11")]
            _11,
            [Description("12")]
            _12,
            [Description("13")]
            _13,
            [Description("14")]
            _14,
            [Description("15")]
            _15,
            [Description("16")]
            _16,
            [Description("17")]
            _17,
            [Description("18")]
            _18,
            [Description("19")]
            _19,
            [Description("20")]
            _20,
            [Description("21")]
            _100,
            [Description("x")]
            X = 100_000,
            [Description("OK")]
            OK = 0,
        }
    }
}