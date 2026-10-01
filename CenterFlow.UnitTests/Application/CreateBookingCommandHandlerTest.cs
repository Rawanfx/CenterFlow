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
            var room = new Room() { Id = Guid.NewGuid(),Capacity=10,Name="Room test" };
            var mockNotiticationService = new Mock<INotificationService>();
            mockNotiticationService.Setup(x => x.SendAsync(teacher.Id, It.IsAny<string>(),It.IsAny<string>(),It.IsAny<NotificationType>()))
                .Returns(Task.CompletedTask);
            var exictedBooking = new Book()
            {
                Id = Guid.NewGuid(),
                TeacherId =Guid.Parse( teacher.Id),
                RoomId = room.Id,
           From=new TimeSpan(10,0,0),
           To = new TimeSpan (12,0,0),
           Date = new DateOnly(2024, 6, 1),
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
             await act.Should().ThrowAsync<ConflictException>().WithMessage("You already have a booking during this time.");
        }
    }
}
