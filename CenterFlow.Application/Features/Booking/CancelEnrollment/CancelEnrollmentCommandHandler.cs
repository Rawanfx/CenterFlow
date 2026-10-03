using CenterFlow.Application.Common.Exceptions;
using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CenterFlow.Application.Features.Booking.CancelEnrollment
{
    public class CancelEnrollmentCommandHandler : IRequestHandler<CancelEnrollmentCommand, Response<string>>
    {
        private readonly ICurrentUserService userService;
        private readonly IAppDbContext context;
        public CancelEnrollmentCommandHandler(ICurrentUserService userService,IAppDbContext context)
        {
            this.context = context;
            this.userService = userService;
        }
        public async Task<Response<string>> Handle(CancelEnrollmentCommand request, CancellationToken cancellationToken)
        {
            var studentEnroll = await context.StudentBookings
                .FirstOrDefaultAsync(x => x.Id == request.EnrollId 
                && x.StudentId == Guid.Parse(userService.UserId)
                && !x.IsCancelled);
            if ( studentEnroll == null)
                throw new NotFoundException("Data not found");
            studentEnroll.IsCancelled = true;
            await context.SaveChangesAsync();
            return new Response<string>() { Success = true, Message = "Cancelled Successfully" };
        }
    }
}
