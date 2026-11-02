using CenterFlow.Application.Common.Interfaces;
using MediatR;
using CenterFlow.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
namespace CenterFlow.Application.Features.Rates.AddTeacherRating
{
    public class AddTeacherRatingCommandHandler : IRequestHandler<AddTeacherRatingCommand, Unit>
    {
        private readonly IAppDbContext context;
        private readonly ICurrentUserService userService;
        public AddTeacherRatingCommandHandler(IAppDbContext context, ICurrentUserService userService)
        {
            this.context = context;
            this.userService = userService;
        }
        public async Task<Unit> Handle(AddTeacherRatingCommand request, CancellationToken cancellationToken)
        {
            var studentBooking = await context.StudentBookings.FirstOrDefaultAsync(x => x.Id == request.StudentBookingId && x.StudentId ==Guid.Parse(userService.UserId));
            var teacherGradeLevel =await context.TeacherSubjectAssignment.FirstOrDefaultAsync(x => x.Id == request.TeacherSubjectAssignmentId);
            if (teacherGradeLevel == null || studentBooking == null)
                throw new NotFoundException("Data not found");

            var rating = await context.Rates.AnyAsync(x => x.StudentId == userService.UserId && x.TeacherGradeLevelId == request.TeacherSubjectAssignmentId);
            if (rating)
                throw new ConflictException("You have already rated this teacher for this subject."

);

            var teacherRating = new Domain.Entities.TeacherRatings
            {
                StudentId = userService.UserId,
                TeacherGradeLevelId = request.TeacherSubjectAssignmentId,
                Comment = request.Comment,
                CreatedAt =DateTime.UtcNow,
                Id = Guid.NewGuid(),
                Rate = request.Rating,
                StudentBookingId = request.StudentBookingId,
                
            };

            await context.Rates.AddAsync(teacherRating);
            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
