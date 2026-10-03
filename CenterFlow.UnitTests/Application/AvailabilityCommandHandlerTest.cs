
using CenterFlow.Application.Common.Exceptions;
using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Application.Features.Availability.DeleteAvailableSlot;
using CenterFlow.Domain.Entities;
using CenterFlow.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CenterFlow.UnitTests.Application
{
    public class AvailabilityCommandHandlerTest
    {
        private AppDbContext BuildInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }
        private ICurrentUserService MockUser(string userId)
        {
            var mockUserService = new Mock<ICurrentUserService>();
            mockUserService.Setup(x => x.UserId).Returns(userId);
            return mockUserService.Object;
        }
        [Fact]
        public async Task DeleteAvailab_ShouldThrowNotFoundException_WhenAvailabilitySlotDoesNotExist()
        {
            // Arrange
            var context = BuildInMemoryContext();
            var techear = new Teacher()
            {
                Id = Guid.NewGuid().ToString(),
                Email = "t@gmail.com",
                UserName = "teacher1",
                FullName = "Teacher One",
                SubjectId = Guid.NewGuid()
            };
            var availability = new TeacherAvailability()
            {
                Id = Guid.NewGuid(),
                TeacherId = Guid.Parse(techear.Id),
                DayOfWeek = DayOfWeek.Monday,
                From = new TimeSpan(9, 0, 0),
                To = new TimeSpan(17, 0, 0),
                IsDelete = true
            };
            await context.Teachers.AddAsync(techear);
            await context.TeacherAvailabilities.AddAsync(availability);
            await context.SaveChangesAsync();
            var userService = MockUser(techear.Id);
            var handler = new DeleteAvailableSlotCommandHandler(context, userService);
            var command = new DeleteAvailableSlotCommand(availability.Id);
            // Act & Assert
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<NotFoundException>().WithMessage("Availability slot not found.");
        }
        [Fact]
        public async Task _ShouldThrowNotFoundException_WhenAvailabilitySlotDoesNotBelongToCurrentUser()
        {
            // Arrange
            var context = BuildInMemoryContext();
            var techear1 = new Teacher()
            {
                Id = Guid.NewGuid().ToString(),
                Email = "t@gmail.com",
                UserName = "teacher1",
                FullName = "Teacher One",
                SubjectId = Guid.NewGuid()
            };
            var techear2 = new Teacher()
            {
                Id = Guid.NewGuid().ToString(),
                Email = "t2@gmail.com",
                UserName = "teacher2",
                FullName = "Teacher two",
                SubjectId = Guid.NewGuid()
            };

            var availability = new TeacherAvailability()
            {
                Id = Guid.NewGuid(),
                TeacherId = Guid.Parse(techear2.Id),
                DayOfWeek = DayOfWeek.Monday,
                From = new TimeSpan(9, 0, 0),
                To = new TimeSpan(17, 0, 0),
                IsDelete = false
            };
            await context.Teachers.AddAsync(techear1);
            await context.Teachers.AddAsync(techear2);
            await context.TeacherAvailabilities.AddAsync(availability);
            await context.SaveChangesAsync();
            var userService = MockUser(techear1.Id);
            var habdler = new DeleteAvailableSlotCommandHandler(context, userService);
            var command = new DeleteAvailableSlotCommand(availability.Id);
            Func<Task> act = async () => await habdler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<NotFoundException>().WithMessage("Availability slot not found.");
        }
        [Fact]
        public async Task DeleteAvailab_ShouldMarkAvailabilitySlotAsDeleted_WhenAvailabilitySlotExistsAndBelongsToCurrentUser()
        {
            // Arrange
            var context = BuildInMemoryContext();
            var techear1 = new Teacher()
            {
                Id = Guid.NewGuid().ToString(),
                Email = "t@gmail.com",
                UserName = "teacher1",
                FullName = "Teacher One",
                SubjectId = Guid.NewGuid()
            };
            var techear2 = new Teacher()
            {
                Id = Guid.NewGuid().ToString(),
                Email = "t2@gmail.com",
                UserName = "teacher2",
                FullName = "Teacher two",
                SubjectId = Guid.NewGuid()
            };
            var availability = new TeacherAvailability()
            {
                Id = Guid.NewGuid(),
                TeacherId = Guid.Parse(techear1.Id),
                DayOfWeek = DayOfWeek.Monday,
                From = new TimeSpan(9, 0, 0),
                To = new TimeSpan(17, 0, 0),
                IsDelete = false
            };
            await context.Teachers.AddAsync(techear1);
            await context.Teachers.AddAsync(techear2);
            await context.TeacherAvailabilities.AddAsync(availability);
            await context.SaveChangesAsync();
            var userService = MockUser(techear1.Id);
            var habdler = new DeleteAvailableSlotCommandHandler(context, userService);
            var command = new DeleteAvailableSlotCommand(availability.Id);
            Func<Task> act = async () => await habdler.Handle(command, CancellationToken.None);
           await act.Should().NotThrowAsync();
            var updatedAvailability = await context.TeacherAvailabilities.FindAsync(availability.Id);
            updatedAvailability.IsDelete.Should().BeTrue();
        }
    }
}
