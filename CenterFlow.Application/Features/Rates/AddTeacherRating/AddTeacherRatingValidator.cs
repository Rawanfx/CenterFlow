using FluentValidation;

namespace CenterFlow.Application.Features.Rates.AddTeacherRating
{
    public class AddTeacherRatingValidator:AbstractValidator<AddTeacherRatingCommand>
    {
        public AddTeacherRatingValidator()
        {
            RuleFor(x => x.Rating).InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");
        }
    }
}
