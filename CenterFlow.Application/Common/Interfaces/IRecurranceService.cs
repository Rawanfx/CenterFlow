using CenterFlow.Domain.Enum;

namespace CenterFlow.Application.Common.Interfaces
{
    public interface IRecurranceService
    {
        List<DateOnly> Dates( IEnumerable<DayOfWeek> day, DateOnly startDate, int times, RecuringType recuringType);
    }
}
