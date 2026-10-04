using CenterFlow.Application.Common.Exceptions;
using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Application.Common.Models;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CenterFlow.Application.Features.Booking.CancelBooking
{
    public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, Response<string>>
    {
        private readonly IBackgroundJobClient client;
        private readonly ICurrentUserService userService;
        private readonly IAppDbContext context;
        public CancelBookingCommandHandler(ICurrentUserService userService
            , IAppDbContext context
            ,IBackgroundJobClient client)
        {
            this.context = context;
            this.userService = userService;
            this.client = client;
        }
        public async Task<Response<string>> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            var book = await context.Books
                .Include(x=>x.Teacher)
                 .FirstOrDefaultAsync(x => x.Id == request.BookId
                 && x.Status != Domain.Enum.BookingStatus.Cancelled
                 && x.TeacherId == Guid.Parse(userService.UserId));
            if (book == null)
                throw new NotFoundException("this book not found");
            book.Status = Domain.Enum.BookingStatus.Cancelled;
            var enrollment = await context.StudentBookings
                .Include(x=>x.Student)
                .Where(x => x.BookId == request.BookId &&x.Status!=Domain.Enum.StudentBookingStatus.Cancelled).ToListAsync();
            var sendEmail = enrollment
                .Select(x => new SendEmailDto()
                {
                    date= book.Date,
                    from = book.From,
                    to = book.To,
                    studentEmail = x.Student.Email,
                    studentName = x.Student.FullName,
                    teacherName= book.Teacher.FullName
                }).ToList();
            foreach (var i in enrollment)
            {
               i.Status = Domain.Enum.StudentBookingStatus.Cancelled;
            }
            await context.SaveChangesAsync();
           
            client.Enqueue<ICancelEnrollment>(x => x.BookingCancellationNotifier(sendEmail));
            return new Response<string>()
            {
                Success = true,
                Message = "Book Cancelled Successfully"
            };
        }
    }
}
