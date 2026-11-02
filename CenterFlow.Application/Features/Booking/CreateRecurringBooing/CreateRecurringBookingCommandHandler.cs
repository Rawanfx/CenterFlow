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
            var response = new BatchBookingResult();
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
                context.RecurringBookings.Add(recurringBooking);
                await context.SaveChangesAsync(cancellationToken);
                int success = 0;
                int count = item.Count > 1 ? item.Count : 1;
                for (int i = 0; i < count; i++)
                {
                    var date = item.Type switch
                    {
                        RecurringType.Daily => item.StartDate.AddDays(i),
                        RecurringType.Weekly => item.StartDate.AddDays(i * 7),
                        RecurringType.Monthly => item.StartDate.AddMonths(i),
                        _ => item.StartDate
                    };
                    var createBookingHandler = new CreateBookingCommand(item.RoomId, item.From, item.To, date, item.TeacherSubjectAssignId, recurringBooking.Id);
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
                    if (success==0)
                    {
                        context.RecurringBookings.Remove(recurringBooking);
                        await context.SaveChangesAsync(cancellationToken);
                        break;
                    };
                }

            }
            return new Response<BatchBookingResult>() { Data = response, Success = true };
        }
    }
}
