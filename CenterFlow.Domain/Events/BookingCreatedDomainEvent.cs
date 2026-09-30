using CenterFlow.Domain.Common;

namespace CenterFlow.Domain.Events;

    public record BookingCreatedDomainEvent(
         Guid BookId,
    string TeacherId,
    string StudentUserId,
    DateOnly SessionDate,
    TimeSpan From,       // 👈
    TimeSpan To          // 👈
) : IDomainEvent;

