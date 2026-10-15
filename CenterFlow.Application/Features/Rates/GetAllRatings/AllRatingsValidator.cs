using FluentValidation;

namespace CenterFlow.Application.Features.Rates.GetAllRatings
{
    public class AllRatingsValidator:AbstractValidator<GetAllRatingsQuery>
    {
        public AllRatingsValidator()
        {
            RuleFor(x => x.To)
                .GreaterThan(x => x.From);
        }
    }
}
