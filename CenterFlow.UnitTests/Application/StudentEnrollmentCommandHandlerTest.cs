using CenterFlow.Application.Common.Exceptions;
using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Application.Features.Enrollment.StudentEnroll;
using CenterFlow.Domain.Entities;
using CenterFlow.Domain.Enum;
using CenterFlow.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RedLockNet;

namespace CenterFlow.UnitTests.Application
{
    public class StudentEnrollmentCommandHandlerTest
    {
        private AppDbContext BuildInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
            return new AppDbContext(options.Options);
        }
        private ICurrentUserService currentUserService(string userId)
        {
            var mock = new Mock<ICurrentUserService>();
            mock.Setup(x => x.UserId).Returns(userId);
            return mock.Object;
        }
        private IDistributedLockFactory BuildDistributedLockFactory()
        {
            var mock = new Mock<IDistributedLockFactory>();
            var redLockMock = new Mock<IRedLock>();
            redLockMock.Setup(x => x.IsAcquired).Returns(true);
            mock.Setup(x => x.CreateLockAsync(
                It.IsAny<string>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<TimeSpan>()
                )).ReturnsAsync(redLockMock.Object);
            return mock.Object;
        }
        private IDistributedLockFactory BuildFailingDistributedLockFactory()
        {
            var mock = new Mock<IDistributedLockFactory>();
            var redLockMock = new Mock<IRedLock>();
            redLockMock.Setup(x => x.IsAcquired).Returns(false);
            mock.Setup(x => x.CreateLockAsync(
                It.IsAny<string>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<TimeSpan>()
                )).ReturnsAsync(redLockMock.Object);
            return mock.Object;
        }
        [Fact]
        public async Task StudentEnrollent_ShouldThrowConflictException_WhenBookinAnceled()
        {
            var context = BuildInMemoryContext();
            var student = new Student()
            {
                Id = Guid.NewGuid().ToString(),
                FullName = "Test Student",
                Email = "s@gmail.com"
            };
            var room = new Room()
            {
                Id = Guid.NewGuid(),
                Name = "Test Room",
                Capacity = 10
            };
            var book = new Book()
            {
                Id = Guid.NewGuid(),
                Date = new DateOnly(2026, 10, 3),
                From = new TimeSpan(13, 0, 0),
                To = new TimeSpan(14, 0, 0),
                RoomId = room.Id,
                Status = Domain.Enum.BookingStatus.Cancelled,
                TeacherId = Guid.NewGuid()
            };
            await context.Students.AddAsync(student);
            await context.Rooms.AddAsync(room);
            await context.Books.AddAsync(book);
            await context.SaveChangesAsync();
            var handler = new StudentEnrollCommandHandler(context, currentUserService(student.Id), BuildDistributedLockFactory());
            var enrollmentCommand = new StudentEnrollCommand(book.Id);
            Func<Task> act = async () => await handler.Handle(enrollmentCommand, CancellationToken.None);
            await act.Should().ThrowAsync<ConflictException>().WithMessage("this book isn't available");
        }
        [Fact]
        public async Task StudentEnrollent_ShouldThrowConflictException_WhenAlreadyEnrolled()
        {
            var context = BuildInMemoryContext();
            var student = new Student()
            {
                Id = Guid.NewGuid().ToString(),
                FullName = "Test Student",
                Email = "test@student.com"
            };
            var room = new Room()
            {
                Id = Guid.NewGuid(),
                Name = "Test Room",
                Capacity = 10
            };
            var book = new Book()
            {
                Id = Guid.NewGuid(),
                Date = new DateOnly(2026, 10, 3),
                From = new TimeSpan(13, 0, 0),
                To = new TimeSpan(14, 0, 0),
                RoomId = room.Id,
                Status = Domain.Enum.BookingStatus.Confirmed,
                TeacherId = Guid.NewGuid()
            };
            var enrollment = new StudentBooking()
            {
                Id = Guid.NewGuid(),
                BookId = book.Id,
                EnrolledAt = DateTime.UtcNow,
                StudentId = Guid.Parse(student.Id),
                Status = StudentBookingStatus.Pending
            };
            await context.Students.AddAsync(student);
            await context.Rooms.AddAsync(room);
            await context.Books.AddAsync(book);
            await context.StudentBookings.AddAsync(enrollment);
            await context.SaveChangesAsync();
            var handler = new StudentEnrollCommandHandler(context, currentUserService(student.Id), BuildDistributedLockFactory());
            var enrollmentCommand = new StudentEnrollCommand(book.Id);
            Func<Task> act = async () => await handler.Handle(enrollmentCommand, CancellationToken.None);
            await act.Should().ThrowAsync<ConflictException>().WithMessage("You are already enrolled in this session.");

        }
        [Fact]
        public async Task StudentEnrollent_ShouldThrowConflictException_WhenSessionIsFull()
        {
            var context = BuildInMemoryContext();
            var student = new Student()
            {
                Id = Guid.NewGuid().ToString(),
                FullName = "Test Student",
                Email = "test@gmail.com",
            };
            var room = new Room()
            {
                Id = Guid.NewGuid(),
                Name = "Test Room",
                Capacity = 1
            };
            var book = new Book()
            {
                Id = Guid.NewGuid(),
                Date = new DateOnly(2026, 10, 3),
                From = new TimeSpan(13, 0, 0),
                To = new TimeSpan(14, 0, 0),
                RoomId = room.Id,
                Status = Domain.Enum.BookingStatus.Confirmed,
                TeacherId = Guid.NewGuid()
            };
            var existingEnrollment = new StudentBooking()
            {
                Id = Guid.NewGuid(),
                BookId = book.Id,
                EnrolledAt = DateTime.UtcNow,
                Status = StudentBookingStatus.Pending,
                StudentId = Guid.NewGuid()
            };
            await context.Students.AddAsync(student);
            await context.Rooms.AddAsync(room);
            await context.Books.AddAsync(book);
            await context.StudentBookings.AddAsync(existingEnrollment);
            await context.SaveChangesAsync();

            var handler = new StudentEnrollCommandHandler(context, currentUserService(student.Id), BuildDistributedLockFactory());
            var enrollmentCommand = new StudentEnrollCommand(book.Id);
            Func<Task> act = async () => await handler.Handle(enrollmentCommand, CancellationToken.None);
            await act.Should().ThrowAsync<ConflictException>().WithMessage("This session is fully booked.");
        }

        [Fact]
        public async Task StudentEnrollment_ShouldThrowConflictException_WhenLockNotAcquired()
        {
            var context = BuildInMemoryContext();
            var student = new Student()
            {
                Id = Guid.NewGuid().ToString(),
                FullName = "Test Student",
                Email = "test@gmail.com",
            };
            var room = new Room()
            {
                Id = Guid.NewGuid(),
                Name = "Test Room",
                Capacity = 10
            };
            var book = new Book()
            {
                Id = Guid.NewGuid(),
                Date = new DateOnly(2026, 10, 3),
                From = new TimeSpan(13, 0, 0),
                To = new TimeSpan(14, 0, 0),
                RoomId = room.Id,
                Status = Domain.Enum.BookingStatus.Confirmed,
                TeacherId = Guid.NewGuid()
            };
            await context.Students.AddAsync(student);
            await context.Rooms.AddAsync(room);
            await context.Books.AddAsync(book);
            await context.SaveChangesAsync();

            var handler = new StudentEnrollCommandHandler(context, currentUserService(student.Id), BuildFailingDistributedLockFactory());
            var enrollmentCommand = new StudentEnrollCommand(book.Id);
            Func<Task> act = async () => await handler.Handle(enrollmentCommand, CancellationToken.None);
            await act.Should().ThrowAsync<ConflictException>().WithMessage("Please try again in a moment.");

        }
        [Fact]
        public async Task StudentEnrollment_ShouldCreateEnrollment_WhenLockAcquired()
        {
            var context = BuildInMemoryContext();
            var student = new Student()
            {
                Id = Guid.NewGuid().ToString(),
                FullName = "Test Student",
                Email = "test@gmail.com",
            };
            var room = new Room()
            {
                Id = Guid.NewGuid(),
                Name = "Test Room",
                Capacity = 10
            };
            var book = new Book()
            {
                Id = Guid.NewGuid(),
                Date = new DateOnly(2026, 10, 3),
                From = new TimeSpan(13, 0, 0),
                To = new TimeSpan(14, 0, 0),
                RoomId = room.Id,
                Status = Domain.Enum.BookingStatus.Confirmed,
                TeacherId = Guid.NewGuid()
            };
            await context.Students.AddAsync(student);
            await context.Rooms.AddAsync(room);
            await context.Books.AddAsync(book);
            await context.SaveChangesAsync();

            var handler = new StudentEnrollCommandHandler(context, currentUserService(student.Id), BuildDistributedLockFactory());
            var enrollmentCommand = new StudentEnrollCommand(book.Id);
            Func<Task> act = async () => await handler.Handle(enrollmentCommand, CancellationToken.None);
            await act.Should().NotThrowAsync();


        }
    } 
}
