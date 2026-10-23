using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Domain.Enum;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Evaluation;

namespace CenterFlow.Infrastructure.Services
{
    public class RecurranceService : IRecurranceService
    {
        private static string GetShortDay(DayOfWeek day) => day switch
        {
            DayOfWeek.Sunday => "SU",
            DayOfWeek.Monday => "MO",
            DayOfWeek.Tuesday => "TU",
            DayOfWeek.Wednesday => "WE",
            DayOfWeek.Thursday => "TH",
            DayOfWeek.Friday => "FR",
            DayOfWeek.Saturday => "SA",
            _ => throw new ArgumentOutOfRangeException(nameof(day))
        };

        private static string FreqString(RecuringType type) => type switch
        {
            RecuringType.Daily => "DAILY",
            RecuringType.Weekly => "WEEKLY",
            RecuringType.Monthly => "MONTHLY",
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };

        public List<DateOnly> Dates(IEnumerable<DayOfWeek> days, DateOnly startDate, int times, RecuringType recuringType)
        {
            var byDay = string.Join(",", days.Distinct().Select(GetShortDay));
            var freq = FreqString(recuringType);

            var rrule = $"FREQ={freq};BYDAY={byDay};COUNT={times}";

            var calendarEvent = new CalendarEvent
            {
                Start = new CalDateTime(startDate.ToDateTime(TimeOnly.MinValue)),
                RecurrenceRule = new RecurrencePattern(rrule) 
            };

            var calendar = new Ical.Net.Calendar();
            calendar.Events.Add(calendarEvent);

            var from = new CalDateTime(startDate.ToDateTime(TimeOnly.MinValue));
            var to = new CalDateTime(startDate.AddYears(2).ToDateTime(TimeOnly.MinValue)); 

            return calendarEvent
                .GetOccurrences(from,to)
                .Select(o => DateOnly.FromDateTime(o.Period.StartTime.Value))
                .Take(times)
                .ToList();
        }
    }
}