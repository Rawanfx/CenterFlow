using CenterFlow.Application.Common.Exceptions;
using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Application.Features.Booking.CreateBooking;
using CenterFlow.Domain.Entities;
using CenterFlow.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RedLockNet;
using RedLockNet.SERedis;

namespace CenterFlow.UnitTests.Application
{
    public class CreateBookingCommandHandlerTest
    {
        private AppDbContext BuildInMemory()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }
        private class FakeCurrentUserService : ICurrentUserService
        {
            public string UserId { get; set; }
        }
        private IDistributedLockFactory BuildRedLockFactory()
        {
            var redLockFactoryMock = new Mock<IDistributedLockFactory>();

            var redLockMock = new Mock<IRedLock>();
            redLockMock.Setup(l => l.IsAcquired).Returns(true);

            redLockFactoryMock
                .Setup(x => x.CreateLockAsync(
                    It.IsAny<string>(),
                    It.IsAny<TimeSpan>(),
                    It.IsAny<TimeSpan>(),
                    It.IsAny<TimeSpan>(),
                    null))
                .ReturnsAsync(redLockMock.Object);

            return redLockFactoryMock.Object;
        }
        [Fact]
        public async Task Handle_ShouldThrowConflictException_WhenBookingOutsideAvailability()
        {
            // Arrange
            var context = BuildInMemory();
            var teacher = new Teacher(
            )
            { Id = Guid.NewGuid().ToString(), FullName = "Test Teacher", Email = "t@test.com", UserName = "t@test.com" };
            var room = new Room() { Id = Guid.NewGuid(), Capacity = 10, Name = "Room test" };
            var mockNotiticationService = new Mock<INotificationService>();
            mockNotiticationService.Setup(x => x.SendAsync(teacher.Id, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<NotificationType>()))
                .Returns(Task.CompletedTask);
            var exictedBooking = new Book()
            {
                Id = Guid.NewGuid(),
                TeacherId = Guid.Parse(teacher.Id),
                RoomId = room.Id,
                From = new TimeSpan(10, 0, 0),
                To = new TimeSpan(12, 0, 0),
                Date = new DateOnly(2024, 6, 1),
                Status = Domain.Enum.BookingStatus.Confirmed
            };
            await context.AddAsync(teacher);
            await context.AddAsync(room);
            await context.AddAsync(exictedBooking);
            await context.SaveChangesAsync();

            var fakeCurrentUserService = new FakeCurrentUserService { UserId = teacher.Id };
            var mockRedLockFactory = BuildRedLockFactory();
            var handler = new CreateBookingCommandHandler(context, fakeCurrentUserService, mockRedLockFactory, mockNotiticationService.Object);
            var command = new CreateBookingCommand(
                room.Id,
                new TimeSpan(11, 0, 0), // Overlaps with existing booking
                new TimeSpan(13, 0, 0),
                new DateOnly(2024, 6, 1));
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ConflictException>().WithMessage("This time is outside your declared availability.");
        }
        [Fact]
        public async Task Handel_ShouldThrowConfilctException_WhenBookingOverlapsWithExistingBooking()
        {
            var context = BuildInMemory();
            Subject subject = new Subject()
            {
                Id = Guid.NewGuid(),
                Name = "Test Subject"
            };
            var teacher = new Teacher()
            {
                Id = Guid.NewGuid().ToString(),
                FullName = "Test Teacher",
                Email="t@test.com",
                UserName="t@test.com",
                SubjectId=subject.Id
            };
            var room = new Room() { Id = Guid.NewGuid(), Capacity = 10, Name = "Room test" };
            var availability = new TeacherAvailability()
            {
                Id = Guid.NewGuid(),
                TeacherId = Guid.Parse(teacher.Id),
                From = new TimeSpan(9, 0, 0),
                To = new TimeSpan(17, 0, 0),
                DayOfWeek = DayOfWeek.Saturday
            };
            var existingBooking = new Book()
            {
                Id = Guid.NewGuid(),
                TeacherId = Guid.Parse(teacher.Id),
                RoomId = room.Id,
                From = new TimeSpan(10, 0, 0),
                To = new TimeSpan(12, 0, 0),
                Date = new DateOnly(2026, 10, 3),
            };
            var bookingCommand = new CreateBookingCommand(
                room.Id,
                new TimeSpan(11, 0, 0), // Overlaps with existing booking
                new TimeSpan(13, 0, 0),
                new DateOnly(2026, 10, 3));
            await context.Rooms.AddAsync(room);
            await context.Subjects.AddAsync(subject);
            await context.Books.AddAsync(existingBooking);
            await context.Teachers.AddAsync(teacher);
            await context.TeacherAvailabilities.AddAsync(availability);
            await context.SaveChangesAsync();
            var FakeCurrentUserService = new FakeCurrentUserService { UserId = teacher.Id };
            var buildRedLockFactory = BuildRedLockFactory();
            var notificationServiceMock = new Mock<INotificationService>();
            notificationServiceMock.Setup(x => x.SendAsync(FakeCurrentUserService.UserId, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<NotificationType>()));
            var handler =  new CreateBookingCommandHandler(context,FakeCurrentUserService,buildRedLockFactory,notificationServiceMock.Object);
            Func<Task> act = async () => await handler.Handle(bookingCommand, CancellationToken.None);
            await act.Should().ThrowAsync<ConflictException>().WithMessage("This room has been booked");
        }
        [Fact]
        public async Task Handle_ShouldThrowConfilictException_WhenConfilictOnTeacherAvailable()
        {
            var context = BuildInMemory();
            Subject subject = new Subject()
            {
                Id = Guid.NewGuid(),
                Name = "Test Subject"
            };
            var teacher = new Teacher()
            {
                Id = Guid.NewGuid().ToString(),
                FullName = "Test Teacher",
                Email = "t@test.com",
                UserName = "t@test.com",
                SubjectId = subject.Id
            };
            var room = new Room() { Id = Guid.NewGuid(), Capacity = 10, Name = "Room test" };
            var room2 = new Room() { Id = Guid.NewGuid(), Capacity = 10, Name = "Room test" };
            var availability = new TeacherAvailability()
            {
                Id = Guid.NewGuid(),
                TeacherId = Guid.Parse(teacher.Id),
                From = new TimeSpan(9, 0, 0),
                To = new TimeSpan(17, 0, 0),
                DayOfWeek = DayOfWeek.Saturday
            };
            var existingBooking = new Book()
            {
                Id = Guid.NewGuid(),
                TeacherId = Guid.Parse(teacher.Id),
                RoomId = room.Id,
                From = new TimeSpan(10, 0, 0),
                To = new TimeSpan(12, 0, 0),
                Date = new DateOnly(2026, 10, 3),
                Status = Domain.Enum.BookingStatus.Confirmed
            };
            var bookingCommand = new CreateBookingCommand(
                room2.Id,
                new TimeSpan(10, 30, 0), // Overlaps with existing booking
                new TimeSpan(12, 30, 0),
                new DateOnly(2026, 10, 3));
            
            await context.Rooms.AddAsync(room);
            await context.Rooms.AddAsync(room2);
            await context.Subjects.AddAsync(subject);
            await context.Books.AddAsync(existingBooking);
            await context.Teachers.AddAsync(teacher);
            await context.TeacherAvailabilities.AddAsync(availability);
            await context.SaveChangesAsync();

            var fakeUser = new FakeCurrentUserService()
            {
                UserId = teacher.Id
            };

            var redLock = BuildRedLockFactory();

            var notificationServiceMock = new Mock<INotificationService>();
            notificationServiceMock.Setup(x => x.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<NotificationType>()));

            var handler = new CreateBookingCommandHandler(context, fakeUser, redLock, notificationServiceMock.Object);

            Func<Task> act = async () => await handler.Handle(bookingCommand, CancellationToken.None);
            await act.Should().ThrowAsync<ConflictException>().WithMessage("You already have a booking during this time.");
        }
    }
}
