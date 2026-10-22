using CenterFlow.Application.Common.Models;
using CenterFlow.Domain.Enum;
using MediatR;

namespace CenterFlow.Application.Features.Booking.CreateRecurringBooing;

public record CreateRecurringBookingCommand(
    List<BookingItems> items):IRequest<Response<BatchBookingResult>>
{
}
public record BookingItems(
    Guid RoomId,
    DateOnly StartDate,
    TimeSpan From,
    TimeSpan To,
    RecuringType Type,
    DayOfWeek Day,
    int count);
public class BookResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan From { get; set; }
    public TimeSpan To { get; set; }
    public Guid RoomId { get; set; }
    public Guid BookId { get; set; }
}

public class BatchBookingResult
{
    public int TotalCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailCount { get; set; }
    public Guid RoomId { get; set; }
    public string RoomName { get; set; }
    public List<BookResult>Result { get; set; }
}

