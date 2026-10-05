using MediatR;

namespace CenterFlow.Application.Features.Rates.AddTeacherRating;

public record AddTeacherRatingCommand( Guid TeacherSubjectAssignmentId, decimal Rating,Guid StudentBookingId,string? Comment)
    :IRequest<Unit>;

