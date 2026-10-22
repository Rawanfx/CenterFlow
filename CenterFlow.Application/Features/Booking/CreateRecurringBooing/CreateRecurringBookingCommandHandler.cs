using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CenterFlow.Application.Features.Booking.CreateRecurringBooing
{
    public class CreateRecurringBookingCommandHandler : IRequestHandler<CreateRecurringBookingCommand, Response<BatchBookingResult>>
    {
        private readonly IAppDbContext context;
        private readonly ICurrentUserService userService;
        public CreateRecurringBookingCommandHandler(IAppDbContext context,ICurrentUserService userService)
        {
            this.userService = userService;
            this.context = context;
        }
        public async Task<Response<BatchBookingResult>> Handle(CreateRecurringBookingCommand request, CancellationToken cancellationToken)
        {
            var dayOfWeaks = request.items.Select(x => x.Day).ToList();
            var dates = request.items.Select(x=>x.)

            var availability = await context.TeacherAvailabilities
                .Where(x => x.TeacherId == Guid.Parse(userService.UserId)
                 && dayOfWeaks.Contains(x.DayOfWeek) && !x.IsDelete)
                .Select(x => new
                {
                    x.DayOfWeek,
                    x.From,
                    x.To,
                }).ToListAsync();

            var books=await context.Books
                .Where (x=> re
                && x.Status != Domain.Enum.BookingStatus.Cancelled 
                )
        }
    }
}
