using FluentValidation;

namespace IISLogReader.Domain.IISLog;
public class LogEntryValidator : AbstractValidator<LogEntry>
{
    public LogEntryValidator()
    {
        RuleFor(x => x.Date).NotEmpty();
        RuleFor(x => x.TimeTaken).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Client).NotNull();
        RuleFor(x => x.Server).NotNull();
        RuleFor(x => x.ClientToServer).NotNull();
        RuleFor(x => x.ServerToClient).NotNull();
        RuleFor(x => x.Client.IP).NotEmpty();
        RuleFor(x => x.Server.IP).NotEmpty();
        RuleFor(x => x.Server.Port).GreaterThanOrEqualTo(0).LessThanOrEqualTo(65535);
        RuleFor(x => x.ClientToServer.Method).NotEmpty();
        RuleFor(x => x.ClientToServer.UriStem).NotEmpty();
        RuleFor(x => x.ClientToServer.UserAgent).NotEmpty();
        RuleFor(x => x.ServerToClient.Status).NotEmpty();
        RuleFor(x => x.ServerToClient.Substatus).NotEmpty();
        RuleFor(x => x.ServerToClient.Win32Status).GreaterThanOrEqualTo(0);
    }
}