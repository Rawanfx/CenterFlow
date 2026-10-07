using CenterFlow.Application.Features.Rates.GetTeacherRating;
using CenterFlow.Domain.Entities;
using CenterFlow.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CenterFlow.UnitTests.Application
{
    public class GetTeachersRatingQueryHandlerTest
    {
        private AppDbContext BuildMockDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }
        [Fact]
        public async Task Handle_ShouldReturnEmpty_WhenNoAssignments(){
            var context = BuildMockDbContext();
            var handle = new GetTeacherRatingQueryHandler(context);
            var subject = new Subject()
            {
                Id = Guid.NewGuid(),
                Name = "Arabic"
            };
            var grade = new GradeLevel()
            {
                Id = Guid.NewGuid(),
                Name = "Prep1"
            };
            var query = new GetTeacherRatingQuery(subject.Id, grade.Id);
           var result= await handle.Handle(query, CancellationToken.None);
            result.Should().BeEmpty();
             ;
        }
        [Fact]
        public async Task Handle_ShouldReturnRating_WhenRatingsExist()
        {
            var context = BuildMockDbContext();
            var student = new Student()
            {
                Email = "st@gmail.com",
                FullName = "student 1",
            };
            var student2 = new Student()
            {
                Email = "st2@gmail.com",
                FullName = "student 2",
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
            var teacher = new Teacher()
            {
                FullName = "tech1",
                Email = "ASR@gmail.com",
                Id = Guid.NewGuid().ToString()
            };

            var book = new Book()
            {
                Id = Guid.NewGuid(),
                RoomId = Guid.NewGuid(),
                From = new TimeSpan(3, 0, 0),
                To = new TimeSpan(4, 0, 0),
                TeacherId=Guid.Parse(teacher.Id),
                Status = Domain.Enum.BookingStatus.Pending
            };

            var studentBook = new StudentBooking()
            {
                Id = Guid.NewGuid(),
                BookId = book.Id,
                StudentId =Guid.Parse( student.Id),
                Status = Domain.Enum.StudentBookingStatus.Pending,
            };
            var studentBook2 = new StudentBooking()
            {
                Id = Guid.NewGuid(),
                BookId = book.Id,
                StudentId = Guid.Parse(student.Id),
                Status = Domain.Enum.StudentBookingStatus.Pending,
            };
            var studentBook3 = new StudentBooking()
            {
                Id = Guid.NewGuid(),
                BookId = book.Id,
                StudentId = Guid.Parse(student2.Id),
                Status = Domain.Enum.StudentBookingStatus.Pending,
            };
            

            TeacherSubjectAssignment teacherSubjectAssignment = new TeacherSubjectAssignment()
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                GradeLevelId = grade.Id,
                IsActive = true,
                SubjectId = subject.Id,
                TeacherId = teacher.Id,
            };
            var rates = new TeacherRatings()
            {
                Id = Guid.NewGuid(),
                Rate = 2,
                StudentBookingId = studentBook.Id,
                CreatedAt = DateTime.UtcNow,
                TeacherGradeLevelId = teacherSubjectAssignment.Id,
                StudentId= (student.Id),
            };
            var rates2 = new TeacherRatings()
            {
                Id = Guid.NewGuid(),
                Rate = 3,
                StudentBookingId = studentBook2.Id,
                CreatedAt = DateTime.UtcNow,
                TeacherGradeLevelId = teacherSubjectAssignment.Id,
                StudentId = student.Id
            };
            var rates3 = new TeacherRatings()
            {
                Id = Guid.NewGuid(),
                Rate = 4,
                StudentBookingId = studentBook3.Id,
                CreatedAt = DateTime.UtcNow,
                TeacherGradeLevelId = teacherSubjectAssignment.Id,
                StudentId = student2.Id 
            };
            var rates4 = new TeacherRatings()
            {
                Id = Guid.NewGuid(),
                Rate = 5,
                StudentBookingId = studentBook3.Id,
                CreatedAt = DateTime.UtcNow,
                TeacherGradeLevelId = teacherSubjectAssignment.Id,
                StudentId = student2.Id
            };
            await context.Subjects.AddAsync(subject);
            await context.GradeLevels.AddAsync(grade);
            await context.Teachers.AddAsync(teacher);
            await context.Books.AddAsync(book);
            await context.StudentBookings.AddRangeAsync(new List<StudentBooking>() { 
                studentBook,studentBook2,studentBook3
            });
            await context.TeacherSubjectAssignment.AddAsync(teacherSubjectAssignment);
            await context.Rates.AddRangeAsync(new List<TeacherRatings>()
            {
                rates,rates2,rates3,rates4});
            await context.SaveChangesAsync();
            var query = new GetTeacherRatingQuery(subject.Id,grade.Id);
            var handler = new GetTeacherRatingQueryHandler(context);
            var count = await context.TeacherSubjectAssignment.CountAsync();

            var result = await handler.Handle(query, CancellationToken.None);
            result.Should().HaveCount(1);
            result[0].Rating.Should().Be(3.5m);
            result[0].TeacherName.Should().Be("tech1");
            result[0].TotalRates.Should().Be(4);
        }
        [Fact]
        public async Task Handle_ShouldReturnZeroRating_WhenNoRatings()
        {
            var context = BuildMockDbContext();
            var teacher = new Teacher()
            {
                Id = Guid.NewGuid().ToString(),
                Email = "t@gmail.com",
                FullName = "t1"
            };
            var grade = new GradeLevel() { Id = Guid.NewGuid(), Name = "Prep1" };
            var subject = new Subject()
            {
                Id = Guid.NewGuid(),
                Name = "Arabic"
            };
            var teacherSubjectAssignment = new TeacherSubjectAssignment()
            {
                Id = Guid.NewGuid(),
                GradeLevelId = grade.Id,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                SubjectId = subject.Id,
                TeacherId = teacher.Id
            };
            await context.Subjects.AddAsync(subject);
            await context.GradeLevels.AddAsync(grade);
            await context.Teachers.AddAsync(teacher);
            await context.TeacherSubjectAssignment.AddAsync(teacherSubjectAssignment);
            await context.SaveChangesAsync();

            var query = new GetTeacherRatingQuery(subject.Id, grade.Id);
            var handler = new GetTeacherRatingQueryHandler(context);
            var result = await handler.Handle(query, CancellationToken.None);
            result.Should().HaveCount(1);
            result[0].Rating.Should().Be(0);
            result[0].TeacherName.Should().Be("t1");
            result[0].TeacherGradeLevelId.Should().Be(teacherSubjectAssignment.Id);
            result[0].TotalRates.Should().Be(0);
        }
    }
}
