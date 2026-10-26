using CenterFlow.Application.Common.Models;
using CenterFlow.Domain.Enum;
using MediatR;

public record CreateRecurringBookingCommand(List<BookingItem> Items)
    : IRequest<Response<BatchBookingResult>>;

public record BookingItem(
    Guid RoomId,
    Guid TeacherSubjectAssignId,
    DateOnly StartDate,
    TimeSpan From,
    TimeSpan To,
    RecurringType Type,
    int Count);   // يتجاهل لو Type = None (يتحسب 1)

public class BookResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public SkipReason? FailureReason { get; set; }
    public DateOnly Date { get; set; }
    public TimeSpan From { get; set; }
    public TimeSpan To { get; set; }
    public Guid RoomId { get; set; }
    public Guid? BookId { get; set; }
    public Guid RecurringBookingId { get; set; }
}

public class BatchBookingResult
{
    public List<BookResult> Results { get; set; } = new();
    public int SuccessCount => Results.Count(x => x.Success);
    public int FailCount => Results.Count(x => !x.Success);
}