using CenterFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CenterFlow.Application.Common.Interfaces
{
    public interface IAppDbContext
    {
         DbSet<Teacher> Teachers { get; }
         DbSet<Book> Books { get; }
         DbSet<Room> Rooms { get; }
         DbSet<Student> Students { get; }
         DbSet<StudentBooking> StudentBookings { get; }
         DbSet<Subject> Subjects { get; }
         DbSet<TeacherAvailability> TeacherAvailabilities { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
