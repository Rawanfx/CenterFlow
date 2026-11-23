using CenterFlow.Application.Common.Exceptions;
using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Application.Common.Models;
using CenterFlow.Application.Features.Booking.CreateBooking;
using CenterFlow.Domain.Enum;
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
            var user = await context.TeacherSubjectAssignment
                .FirstOrDefaultAsync(x => x.TeacherId == userService.UserId && x.Id ==request.TeacherSubjectAssignId, cancellationToken);

            if (user == null)
                throw new NotFoundException("Teacher subject assignment not found");
            var roomsRequest = request.Items.Select(x => x.RoomId).Distinct().ToList();
            var rooms = await context.Rooms.Where(x => roomsRequest.Contains(x.Id)).ToListAsync(cancellationToken);
            if (rooms.Count != roomsRequest.Count)
            {
                throw new NotFoundException("One or more rooms not found");
            }

            var response = new BatchBookingResult();
            foreach (var item in request.Items)
            {
                var recurringBooking = new CenterFlow.Domain.Entities.RecurringBooking
                {
                    Id = Guid.NewGuid(),
                    TeacherId =Guid.Parse( userService.UserId),
                    RoomId = item.RoomId,
                    TeacherSubjectAssignId = request.TeacherSubjectAssignId,
                    From = item.From,
                    To = item.To,
                    Type = item.Type,
                    StartDate = item.StartDate,
                    Count = item.Count,
                    CreatedAt = DateTime.UtcNow
                };
                context.RecurringBookings.Add(recurringBooking);
                await context.SaveChangesAsync(cancellationToken);
                int success = 0;
                int count = item.Type == RecurringType.None ? 1 : Math.Max(item.Count,1);
                for (int i = 0; i < count; i++)
                {
                    var date = item.Type switch
                    {
                        RecurringType.Daily => item.StartDate.AddDays(i),
                        RecurringType.Weekly => item.StartDate.AddDays(i * 7),
                        RecurringType.Monthly => item.StartDate.AddMonths(i),
                        _ => item.StartDate
                    };
                    var createBookingHandler = new CreateBookingCommand(item.RoomId, item.From, item.To, date, request.TeacherSubjectAssignId, recurringBooking.Id);
                    try
                    {
                        var result = await mediator.Send(createBookingHandler);
                        response.Results.Add(new BookResult
                        {
                            Success = true,
                            Message = "Booking created successfully",
                            Date = date,
                            From = item.From,
                            To = item.To,
                            RoomId = item.RoomId,
                            BookId = result.Data,
                            RecurringBookingId = recurringBooking.Id
                        });
                        success++;
                    }
                    catch (ConflictException ex)
                    {
                        var freeRooms = await mediator.Send(new GetFreeRoomQuery(date, item.From, item.To));
                        response.Results.Add(new BookResult
                        {
                            Success = false,
                            Message = ex.Message,
                            Date = date,
                            From = item.From,
                            To = item.To,
                            RoomId = item.RoomId,
                            RecurringBookingId = recurringBooking.Id,
                            FailureReason = ex.SkipReason
                        });
                    }
                   catch (NotFoundException ex)
                    {
                        response.Results.Add(new BookResult
                        {
                            Success = false,
                            Message = ex.Message,
                            Date = date,
                            From = item.From,
                            To = item.To,
                            RoomId = item.RoomId,
                            RecurringBookingId = recurringBooking.Id
                        });
                    }
                  
                }
                if (success == 0)
                {
                    context.RecurringBookings.Remove(recurringBooking);
                    await context.SaveChangesAsync(cancellationToken);
                }
                
            }
            return new Response<BatchBookingResult>() { Data = response, Success = true };
        }
    }
}
