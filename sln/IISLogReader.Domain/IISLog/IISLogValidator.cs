using FluentValidation;

namespace IISLogReader.Domain.IISLog;

public class IISLogValidator : AbstractValidator<IISLog>
{
    public IISLogValidator()
    {
        RuleFor(x => x.Created).NotEmpty();
        RuleFor(x => x.LogEntries).NotEmpty();
    }
}