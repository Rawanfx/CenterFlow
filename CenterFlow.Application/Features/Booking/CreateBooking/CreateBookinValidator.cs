
using FluentValidation;

namespace CenterFlow.Application.Features.Booking.CreateBooking
{
    public class CreateBookinValidator:AbstractValidator<CreateBookingCommand>
    {
        public CreateBookinValidator()
        {
            RuleFor(x => x.date)
      .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow));
            RuleFor(x => x)
                .Must(ValidPeriod);
        }
        private bool ValidPeriod(CreateBookingCommand command)=>
             !(command.From >= command.To);
        
    }
}
