namespace IISLogReader.Domain.Common;
public static class Constants
{
    // Exceptions
    public const string InvalidDateFormatExMsg = "Invalid date format.";
    public const string FieldDefinitionExMsg = "Field definition doesn't fit with line values or known field names.";
    public const string UnsupportedLogFieldExMsg = "Unsupported field:";
    public const string InvalidLogLineExMsg = "Invalid log line.";

    // Log constants
    public const string FieldsPrefix = "#Fields:";
    public const string DatePrefix = "#Date:";
    public const string LogCommentPrefix = "#";
    public static char[] LogSplitChars = { ' ' };
    public const string LogNullValue = "-";

    // Log fields
    public const string FieldDate = "date";
    public const string FieldTime = "time";
    public const string FieldSIP = "s-ip";
    public const string FieldMethod = "cs-method";
    public const string FieldUriStem = "cs-uri-stem";
    public const string FieldUriQuery = "cs-uri-query";
    public const string FieldSPort = "s-port";
    public const string FieldUsername = "cs-username";
    public const string FieldCIP = "c-ip";
    public const string FieldUserAgent = "cs(User-Agent)";
    public const string FieldReferer = "cs(Referer)";
    public const string FieldStatus = "sc-status";
    public const string FieldSubstatus = "sc-substatus";
    public const string FieldWin32Status = "sc-win32-status";
    public const string FieldTimeTaken = "time-taken";

    public static List<string> OrderedFieldNames => new List<string>()   {
        FieldDate,
        FieldTime,
        FieldSIP,
        FieldMethod,
        FieldUriStem,
        FieldUriQuery,
        FieldSPort,
        FieldUsername,
        FieldCIP,
        FieldUserAgent,
        FieldReferer,
        FieldStatus,
        FieldSubstatus,
        FieldWin32Status,
        FieldTimeTaken,
    };
}
