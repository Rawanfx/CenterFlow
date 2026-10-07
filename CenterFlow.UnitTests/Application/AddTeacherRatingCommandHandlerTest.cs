using CenterFlow.Application.Common.Exceptions;
using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Application.Features.Rates.AddTeacherRating;
using CenterFlow.Domain.Entities;
using CenterFlow.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Org.BouncyCastle.Tls;

namespace CenterFlow.UnitTests.Application
{
    public class AddTeacherRatingCommandHandlerTest
    {
        private AppDbContext BuildInMemory()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }
        private  ICurrentUserService CurrentUserServiceMock(string userId)
        {
            var userService =new  Mock<ICurrentUserService>();
            userService.Setup(x => x.UserId)
                .Returns(userId);
            return userService.Object;
        }
        [Fact]
        public async Task Handle_ShouldAddRating_WhenValid()
        {
            var context = BuildInMemory();
            Student student = new Student()
            {
                Id = Guid.NewGuid().ToString(),
                FullName = "Rawan",
                Email = "st1@gmail.com"
            };
            var teacher = new Teacher()
            {
                Id = Guid.NewGuid().ToString(),
                FullName = "Ahmed",
                Email = "tech1@gmail.com"
            };
            var book = new Book()
            {
                Id = Guid.NewGuid(),
                TeacherId = Guid.Parse(teacher.Id),
                RoomId = Guid.NewGuid(),
                From = new TimeSpan(3, 0, 0),
                To = new TimeSpan(4, 0, 0),
                Status = Domain.Enum.BookingStatus.Pending
            };
            var studentBook = new StudentBooking()
            {
                Id = Guid.NewGuid(),
                BookId = book.Id,
                StudentId = Guid.Parse(student.Id),
                Status = Domain.Enum.StudentBookingStatus.Pending,
            };
            var grade = new GradeLevel()
            {
                Id = Guid.NewGuid(),
                Name = "Prep1"
            };
            var subject = new Subject()
            {
                Id = Guid.NewGuid(),
                Name = "Arabic"
            };
            var assign = new TeacherSubjectAssignment()
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                GradeLevelId = grade.Id,
                SubjectId = subject.Id,
                TeacherId = teacher.Id
            };
            await context.Students.AddAsync(student);
            await context.Teachers.AddAsync(teacher);
            await context.GradeLevels.AddAsync(grade);
            await context.Subjects.AddAsync(subject);
            await context.Books.AddAsync(book);
            await context.StudentBookings.AddAsync(studentBook);
            await context.TeacherSubjectAssignment.AddAsync(assign);
            await context.SaveChangesAsync();
            var userId = CurrentUserServiceMock(student.Id);
            var command = new AddTeacherRatingCommand(assign.Id, 3, studentBook.Id, "Thanks for your effort !");
            var handler = new AddTeacherRatingCommandHandler(context, userId);
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);
           await act.Should().NotThrowAsync();
            var savedRating = await context.Rates.FirstOrDefaultAsync();
            savedRating.Should().NotBeNull();
            savedRating!.Rate.Should().Be(3);
            savedRating.StudentId.Should().Be(student.Id);
            savedRating.TeacherGradeLevelId.Should().Be(assign.Id);
            savedRating.StudentBookingId.Should().Be(studentBook.Id);
            savedRating.Comment.Should().Be("Thanks for your effort !");
        }
        [Fact]
        public async Task Handle_ShouldNotAllowRatingForAnotherStudentsBooking()
        {
            var context = BuildInMemory();
            Student student = new Student()
            {
                Id = Guid.NewGuid().ToString(),
                FullName = "Rawan",
                Email = "st1@gmail.com"
            };
            Student student2 = new Student()
            {
                Id = Guid.NewGuid().ToString(),
                FullName = "Rawan",
                Email = "st2@gmail.com"
            };
            var teacher = new Teacher()
            {
                Id = Guid.NewGuid().ToString(),
                FullName = "Ahmed",
                Email = "tech1@gmail.com"
            };
            var book = new Book()
            {
                Id = Guid.NewGuid(),
                TeacherId = Guid.Parse(teacher.Id),
                RoomId = Guid.NewGuid(),
                From = new TimeSpan(3, 0, 0),
                To = new TimeSpan(4, 0, 0),
                Status = Domain.Enum.BookingStatus.Pending
            };
            var studentBook2 = new StudentBooking()
            {
                Id = Guid.NewGuid(),
                BookId = book.Id,
                StudentId = Guid.Parse(student2.Id),
                Status = Domain.Enum.StudentBookingStatus.Pending,
            };
            var grade = new GradeLevel()
            {
                Id = Guid.NewGuid(),
                Name = "Prep1"
            };
            var subject = new Subject()
            {
                Id = Guid.NewGuid(),
                Name = "Arabic"
            };
            var assign = new TeacherSubjectAssignment()
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                GradeLevelId = grade.Id,
                SubjectId = subject.Id,
                TeacherId = teacher.Id
            };
            await context.Students.AddRangeAsync(new List<Student>() { student ,student2});
            await context.Teachers.AddAsync(teacher);
            await context.GradeLevels.AddAsync(grade);
            await context.Subjects.AddAsync(subject);
            await context.Books.AddAsync(book);
            await context.StudentBookings.AddAsync(studentBook2);
            await context.TeacherSubjectAssignment.AddAsync(assign);
            await context.SaveChangesAsync();

            var userService = CurrentUserServiceMock(student.Id);
            var command = new AddTeacherRatingCommand(assign.Id, 5, studentBook2.Id,string.Empty);
            var handler = new AddTeacherRatingCommandHandler(context, userService);
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<NotFoundException>().WithMessage("Data not found"); 
        }
        [Fact]
        public async Task Handle_ShouldThrowConflictException_WhenAlreadyRated()
        {
            var context = BuildInMemory();
            Student student = new Student()
            {
                Id = Guid.NewGuid().ToString(),
                FullName = "Rawan",
                Email = "st1@gmail.com"
            };
            var teacher = new Teacher()
            {
                Id = Guid.NewGuid().ToString(),
                FullName = "Ahmed",
                Email = "tech1@gmail.com"
            };
            var book = new Book()
            {
                Id = Guid.NewGuid(),
                TeacherId = Guid.Parse(teacher.Id),
                RoomId = Guid.NewGuid(),
                From = new TimeSpan(3, 0, 0),
                To = new TimeSpan(4, 0, 0),
                Status = Domain.Enum.BookingStatus.Pending
            };
            var studentBook2 = new StudentBooking()
            {
                Id = Guid.NewGuid(),
                BookId = book.Id,
                StudentId = Guid.Parse(student.Id),
                Status = Domain.Enum.StudentBookingStatus.Pending,
            };
            var grade = new GradeLevel()
            {
                Id = Guid.NewGuid(),
                Name = "Prep1"
            };
            var subject = new Subject()
            {
                Id = Guid.NewGuid(),
                Name = "Arabic"
            };
            var assign = new TeacherSubjectAssignment()
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                GradeLevelId = grade.Id,
                SubjectId = subject.Id,
                TeacherId = teacher.Id
            };
            var rate = new TeacherRatings()
            {
                Id = Guid.NewGuid(),
                Rate = 2,
                StudentBookingId = studentBook2.Id,
                TeacherGradeLevelId = assign.Id,
                StudentId = student.Id,
            };
            await context.Students.AddAsync(student );
            await context.Teachers.AddAsync(teacher);
            await context.GradeLevels.AddAsync(grade);
            await context.Subjects.AddAsync(subject);
            await context.Books.AddAsync(book);
            await context.StudentBookings.AddAsync(studentBook2);
            await context.TeacherSubjectAssignment.AddAsync(assign);
            await context.Rates.AddAsync(rate);
            await context.SaveChangesAsync();

            var userService = CurrentUserServiceMock(student.Id);
            var command = new AddTeacherRatingCommand(assign.Id, 5, studentBook2.Id, string.Empty);
            var handler = new AddTeacherRatingCommandHandler(context, userService);
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ConflictException>().WithMessage("You have already rated this teacher for this subject.");


        }
    }
}
