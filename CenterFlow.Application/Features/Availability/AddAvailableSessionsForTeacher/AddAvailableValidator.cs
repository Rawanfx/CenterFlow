using FluentValidation;

namespace CenterFlow.Application.Features.Availability.AddAvailableSessionsForTeacher
{
    public class AddAvailableValidator:AbstractValidator<AddAvailableSessionCommand>
    {
        public AddAvailableValidator()
        {
            RuleFor(x => x.To)
                .GreaterThan(x => x.From);
            RuleFor(x => x.Day).IsInEnum();
            RuleFor(x=>x.To-x.From)
                     .Must(d => d.TotalMinutes >= 30)
            .WithMessage("Availability must be at least 30 minutes.");
        }
    }
}
