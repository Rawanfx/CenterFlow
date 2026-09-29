using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace CenterFlow.Infrastructure.Data
{
    public class AppDbContext:IdentityDbContext<ApplicationUser>,IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Teacher> Teachers => Set<Teacher>();
        public DbSet<Book> Books => Set<Book>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<StudentBooking> StudentBookings => Set<StudentBooking>();
        public DbSet<Subject> Subjects => Set<Subject>();
        public DbSet<TeacherAvailability> TeacherAvailabilities => Set<TeacherAvailability>();
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<TeacherAvailability>()
                .HasQueryFilter(x => !x.IsDelete);
        }
    }
}
