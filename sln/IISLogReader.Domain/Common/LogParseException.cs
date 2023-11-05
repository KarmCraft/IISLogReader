using System.Runtime.Serialization;

namespace IISLogReader.Domain.Common;

[Serializable]
public class LogParseException : Exception
{
    private const string DefaultMessage = "Could not parse IIS log.";

    public LogParseException() : base(DefaultMessage) { }
    public LogParseException(string message) : base($"{DefaultMessage}{Environment.NewLine}{message}") { }
    public LogParseException(string message, Exception innerException) : base($"{DefaultMessage}{Environment.NewLine}{message}", innerException) { }
    protected LogParseException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
