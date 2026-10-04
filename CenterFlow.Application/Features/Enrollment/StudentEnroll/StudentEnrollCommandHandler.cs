using CenterFlow.Application.Common.Exceptions;
using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Application.Common.Models;
using CenterFlow.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RedLockNet;
using RedLockNet.SERedis;

namespace CenterFlow.Application.Features.Enrollment.StudentEnroll
{
   
    public class StudentEnrollCommandHandler:IRequestHandler<StudentEnrollCommand,Response<Guid>>
    {
        private readonly IAppDbContext context;
        private readonly ICurrentUserService userService;
        private readonly IDistributedLockFactory redLockFactory;
        public StudentEnrollCommandHandler(IAppDbContext context
            ,ICurrentUserService userService
            , IDistributedLockFactory redLockFactory)
        {
            this.context = context;
            this.userService = userService;
            this.redLockFactory = redLockFactory;
        }

        public async Task<Response<Guid>> Handle(StudentEnrollCommand request, CancellationToken cancellationToken)
        {
            var student = await context.Students.FirstOrDefaultAsync(x => x.Id == userService.UserId);
            var book = await context.Books
                .Include(x=>x.Room)
                .FirstOrDefaultAsync(x => x.Id == request.BookId);

            if (student == null || book == null)
                throw new ConflictException("Data not found");
            if (book.Status == Domain.Enum.BookingStatus.Cancelled)
                throw new ConflictException("this book isn't available");
            var lockKey = $"enrollment-lock:booking:{request.BookId}";
            await using var redLock = await redLockFactory.CreateLockAsync(
    resource: lockKey,
    expiryTime: TimeSpan.FromSeconds(10),
    waitTime: TimeSpan.FromSeconds(2),
    retryTime: TimeSpan.FromMilliseconds(200));

            if (!redLock.IsAcquired)
                throw new ConflictException("Please try again in a moment.");
            var alreadyEnrolled = await context.StudentBookings
    .AnyAsync(x => x.StudentId == Guid.Parse(student.Id)
    && x.BookId == request.BookId
    && x.Status != Domain.Enum.StudentBookingStatus.Cancelled);
            if (alreadyEnrolled)
                throw new ConflictException("You are already enrolled in this session.");


            var studentCount = await context.StudentBookings
                .Where(x => x.BookId == request.BookId && x.Status != Domain.Enum.StudentBookingStatus.Cancelled)
                .CountAsync();
            if (studentCount + 1 > book.Room.Capacity)
                throw new ConflictException("This session is fully booked.");
            var studentBook = new StudentBooking()
            {
                BookId = request.BookId,
                EnrolledAt = DateTime.UtcNow,
                StudentId = Guid.Parse(student.Id),
                Status = Domain.Enum.StudentBookingStatus.Pending
            };
            await context.StudentBookings.AddAsync(studentBook);
            await context.SaveChangesAsync();
            return new Response<Guid>()
            {
                Success = true,
                Data = studentBook.Id,
                Message ="booking successufully"
            };
        }
    }
}
