using CenterFlow.Application.Features.Rates.GetTeacherRating;
using CenterFlow.Domain.Entities;
using CenterFlow.Infrastructure.Data;
using FluentAssertions;
using FluentAssertions.Equivalency.Tracing;
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
        [Fact]
        public async Task Handle_ShouldOrderByRatingDescending()
        {
            var context = BuildMockDbContext();
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
            var teacher1 = new Teacher()
            {
                Id = Guid.NewGuid().ToString(),
                Email = "t1@gmail.com",
                FullName = "Ahmed",
            }; 
            var teacher2 = new Teacher()
            {
                Id = Guid.NewGuid().ToString(),
                Email = "t2@gmail.com",
                FullName = "Ahmed",
            }; 
            var teacher3 = new Teacher()
            {
                Id = Guid.NewGuid().ToString(),
                Email = "t3@gmail.com",
                FullName = "Ahmed",
            }; 
            Book book1 = new Book()
            {
                Id = Guid.NewGuid(),
                TeacherId = Guid.Parse(teacher1.Id),
                RoomId = Guid.NewGuid(),
                From = new TimeSpan(1, 0, 0),
                To = new TimeSpan(2, 0, 0),
                Status = Domain.Enum.BookingStatus.Confirmed,
            };
            Book book2 = new Book()
            {
                Id = Guid.NewGuid(),
                TeacherId = Guid.Parse(teacher2.Id),
                RoomId = Guid.NewGuid(),
                From = new TimeSpan(1, 0, 0),
                To = new TimeSpan(2, 0, 0),
                Status = Domain.Enum.BookingStatus.Confirmed,
            };
            Book book3 = new Book()
            {
                Id = Guid.NewGuid(),
                TeacherId = Guid.Parse(teacher3.Id),
                RoomId = Guid.NewGuid(),
                From = new TimeSpan(1, 0, 0),
                To = new TimeSpan(2, 0, 0),
                Status = Domain.Enum.BookingStatus.Confirmed,
            };
            var student1 = new Student()
            {
                Email = "st1@gmail.com",
                FullName = "student 1"
            };
            var student2 = new Student()
            {
                Email = "st2@gmail.com",
                FullName = "student 1"
            };
            var student3 = new Student()
            {
                Email = "st3@gmail.com",
                FullName = "student 1"
            };
            var student4 = new Student()
            {
                Email = "st4@gmail.com",
                FullName = "student 1"
            };
            var student5 = new Student()
            {
                Email = "st5@gmail.com",
                FullName = "student 1"
            };
            var student6 = new Student()
            {
                Email = "st6@gmail.com",
                FullName = "student 1"
            };
            var studentBook1 = new StudentBooking()
            {
                Id = Guid.NewGuid(),
                BookId = book1.Id,
                StudentId = Guid.Parse(student1.Id),
                Status = Domain.Enum.StudentBookingStatus.Pending,
            };
            var studentBook2 = new StudentBooking()
            {
                Id = Guid.NewGuid(),
                BookId = book1.Id,
                StudentId = Guid.Parse(student2.Id),
                Status = Domain.Enum.StudentBookingStatus.Pending,
            };
            var studentBook3 = new StudentBooking()
            {
                Id = Guid.NewGuid(),
                BookId = book2.Id,
                StudentId = Guid.Parse(student3.Id),
                Status = Domain.Enum.StudentBookingStatus.Pending,
            };
            var studentBook4 = new StudentBooking()
            {
                Id = Guid.NewGuid(),
                BookId = book2.Id,
                StudentId = Guid.Parse(student4.Id),
                Status = Domain.Enum.StudentBookingStatus.Pending,
            };
            var studentBook5 = new StudentBooking()
            {
                Id = Guid.NewGuid(),
                BookId = book3.Id,
                StudentId = Guid.Parse(student5.Id),
                Status = Domain.Enum.StudentBookingStatus.Pending,
            };
            var studentBook6 = new StudentBooking()
            {
                Id = Guid.NewGuid(),
                BookId = book3.Id,
                StudentId = Guid.Parse(student6.Id),
                Status = Domain.Enum.StudentBookingStatus.Pending,
            };
            var teacherAssigne1 = new TeacherSubjectAssignment()
            {
                Id = Guid.NewGuid(),
                TeacherId = teacher1.Id,
                IsActive = true,
                SubjectId = subject.Id,
                GradeLevelId = grade.Id
            };
            var teacherAssigne2 = new TeacherSubjectAssignment()
            {
                Id = Guid.NewGuid(),
                TeacherId = teacher2.Id,
                IsActive = true,
                SubjectId = subject.Id,
                GradeLevelId = grade.Id
            };
            var teacherAssigne3 = new TeacherSubjectAssignment()
            {
                Id = Guid.NewGuid(),
                TeacherId = teacher3.Id,
                IsActive = true,
                SubjectId = subject.Id,
                GradeLevelId = grade.Id
            };
            // teacher1
            var rate1 = new TeacherRatings()
            {
                Id = Guid.NewGuid(),
                Rate = 3,
                StudentId = student1.Id,
                TeacherGradeLevelId = teacherAssigne1.Id,
                StudentBookingId = studentBook1.Id,
            };
            var rate2 = new TeacherRatings()
            {
                Id = Guid.NewGuid(),
                Rate = 1,
                StudentId = student2.Id,
                TeacherGradeLevelId = teacherAssigne1.Id,
                StudentBookingId = studentBook2.Id,
            };
            //teacher 2
            var rate3 = new TeacherRatings()
            {
                Id = Guid.NewGuid(),
                Rate = 1,
                StudentId = student3.Id,
                TeacherGradeLevelId = teacherAssigne2.Id,
                StudentBookingId = studentBook3.Id,
            };
            var rate4 = new TeacherRatings()
            {
                Id = Guid.NewGuid(),
                Rate = 1,
                StudentId = student4.Id,
                TeacherGradeLevelId = teacherAssigne2.Id,
                StudentBookingId = studentBook4.Id,
            };
            //teacher 3
            var rate5 = new TeacherRatings()
            {
                Id = Guid.NewGuid(),
                Rate = 5,
                StudentId = student5.Id,
                TeacherGradeLevelId = teacherAssigne3.Id,
                StudentBookingId = studentBook5.Id,
            };
            var rate6 = new TeacherRatings()
            {
                Id = Guid.NewGuid(),
                Rate = 4,
                StudentId = student6.Id,
                TeacherGradeLevelId = teacherAssigne3.Id,
                StudentBookingId = studentBook6.Id,
            };
        
        await context.Subjects.AddAsync(subject);
        await context.GradeLevels.AddAsync(grade);
        await context.Teachers.AddRangeAsync(new List<Teacher>() { teacher1,teacher2, teacher3});
        await context.Books.AddRangeAsync(new List<Book>() { book1,book2,book3});
        await context.Students.AddRangeAsync(new List<Student>() { student1, student2, student3, student4, student5, student6 });
        await context.StudentBookings.AddRangeAsync(new List<StudentBooking>() { 
                studentBook1,studentBook2,studentBook3,studentBook4,studentBook5,studentBook6});
            await context.TeacherSubjectAssignment.AddRangeAsync(new List<TeacherSubjectAssignment>() { teacherAssigne1 , teacherAssigne2, teacherAssigne3});
            await context.Rates.AddRangeAsync(new List<TeacherRatings>(){
                rate1,rate2,rate3,rate4,rate5,rate6});
           await context.SaveChangesAsync();

            var query = new GetTeacherRatingQuery(subject.Id, grade.Id);
            var handler = new GetTeacherRatingQueryHandler(context);
            var result = await handler.Handle(query, CancellationToken.None);

            result.Should().HaveCount(3);
            result[0].Rating.Should().Be(4.5m);
            result[0].TeacherGradeLevelId.Should().Be(teacherAssigne3.Id);
            result[0].TotalRates.Should().Be(2);

            result[2].Rating.Should().Be(1);
            result[2].TeacherGradeLevelId.Should().Be(teacherAssigne2.Id);
            result[2].TotalRates.Should().Be(2);

            result[1].Rating.Should().Be(2);
            result[1].TeacherGradeLevelId.Should().Be(teacherAssigne1.Id);
            result[1].TotalRates.Should().Be(2);


        }
    }
}
