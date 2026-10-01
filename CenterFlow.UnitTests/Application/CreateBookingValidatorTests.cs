using CenterFlow.Application.Features.Booking.CreateBooking;
using FluentAssertions;

namespace CenterFlow.UnitTests.Application
{
    public class CreateBookingValidatorTests
    {
        [Theory]
        [InlineData(10, 0, 9, 0, false)]   // From أكبر من To → invalid
        [InlineData(9, 0, 9, 0, false)]    // From == To → invalid
        [InlineData(9, 0, 10, 0, true)]    // From أصغر من To → valid
        public void Validate_ChecksFromAndTo(int fromHour, int fromMin, int toHour, int toMin, bool expectedValid)
        {
            var validator = new CreateBookinValidator();
            var command = new CreateBookingCommand(
                Guid.NewGuid(),
                new TimeSpan(fromHour, fromMin, 0),
                new TimeSpan(toHour, toMin, 0),
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)));

            var result = validator.Validate(command);

            result.IsValid.Should().Be(expectedValid);
        }

        [Theory]
        [InlineData(-1, false)]   // امبارح → invalid
        [InlineData(0, true)]     // النهاردة → valid
        [InlineData(1, true)]     // بكرة → valid
        public void Validate_ChecksDate(int daysFromToday, bool expectedValid)
        {
            var validator = new CreateBookinValidator();
            var command = new CreateBookingCommand(
                Guid.NewGuid(),
                new TimeSpan(9, 0, 0),
                new TimeSpan(10, 0, 0),
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(daysFromToday)));

            var result = validator.Validate(command);

            result.IsValid.Should().Be(expectedValid);
        }
    }
}