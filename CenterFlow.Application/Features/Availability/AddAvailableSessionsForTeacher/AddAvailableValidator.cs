using FluentValidation;

namespace CenterFlow.Application.Features.Availability.AddAvailableSessionsForTeacher
{
    public class AddAvailableValidator:AbstractValidator<AddAvailableSessionCommand>
    {
        public AddAvailableValidator()
        {
            RuleFor(x => x.To)
                .GreaterThan(x => x.From);
        }
    }
}
