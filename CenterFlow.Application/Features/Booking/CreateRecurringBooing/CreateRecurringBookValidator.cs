using FluentValidation;

namespace CenterFlow.Application.Features.Booking.CreateRecurringBooing
{
    public class RecurringBookValidator:AbstractValidator<CreateRecurringBookingCommand>
    {
        public RecurringBookValidator()
        {
            RuleFor(x => x.Items)
                 .NotEmpty()
                 .Must(x => x.Count <= 10).WithMessage("You can't send more than 10 items in one request.");
            RuleForEach(x => x.Items).SetValidator(new BookingItemValidator());
                
        }
       public class BookingItemValidator : AbstractValidator<BookingItem>
        {
            public  BookingItemValidator() {
                RuleFor(a => a.StartDate)
                    .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow));
                RuleFor(x => x)
                    .Must(x => x.From > x.To).WithMessage("From must be before To.");
                RuleFor(x => x.TeacherSubjectAssignId)
                    .NotEqual(Guid.Empty);
                RuleFor(x => x.Type)
                    .IsInEnum();
            }
        }
    }
}
