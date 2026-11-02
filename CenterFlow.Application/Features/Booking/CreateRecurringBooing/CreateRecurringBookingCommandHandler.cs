using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Application.Common.Models;
using CenterFlow.Application.Features.Booking.CreateBooking;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CenterFlow.Application.Features.Booking.CreateRecurringBooing
{
    public class CreateRecurringBookingCommandHandler : IRequestHandler<CreateRecurringBookingCommand, Response<BatchBookingResult>>
    {
        private readonly IMediator mediator;
        private readonly IAppDbContext context;
        private readonly ICurrentUserService userService;
        public CreateRecurringBookingCommandHandler(IMediator medaitor,IAppDbContext context,ICurrentUserService userService)
        {
            this.mediator = medaitor;
            this.context = context;
            this.userService = userService;
        }
       
        public async Task<Response<BatchBookingResult>> Handle(CreateRecurringBookingCommand request, CancellationToken cancellationToken)
        {
            foreach (var item in request.Items)
            {
                var recurringBooking = new CenterFlow.Domain.Entities.RecurringBooking
                {
                    Id = Guid.NewGuid(),
                    TeacherId =Guid.Parse( userService.UserId),
                    RoomId = item.RoomId,
                    TeacherSubjectAssignId = item.TeacherSubjectAssignId,
                    From = item.From,
                    To = item.To,
                    Type = item.Type,
                    StartDate = item.StartDate,
                    Count = item.Count,
                    CreatedAt = DateTime.UtcNow
                };
                var createBookingHandler = new CreateBookingCommand(item.RoomId, item.From, item.To, item.StartDate, item.TeacherSubjectAssignId, recurringBooking.Id);
                var result = await mediator.Send(createBookingHandler);
                context.RecurringBookings.Add(recurringBooking);
            }
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
