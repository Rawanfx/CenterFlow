using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Domain.Common;
using CenterFlow.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace CenterFlow.Infrastructure.Data
{
    public class AppDbContext:IdentityDbContext<ApplicationUser>,IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) :
            base(options) {
            
        }
        public DbSet<Teacher> Teachers => Set<Teacher>();
        public DbSet<Book> Books => Set<Book>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<StudentBooking> StudentBookings => Set<StudentBooking>();
        public DbSet<Subject> Subjects => Set<Subject>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<TeacherAvailability> TeacherAvailabilities => Set<TeacherAvailability>();

        public DbSet<GradeLevel> GradeLevels => Set<GradeLevel>();
        public DbSet<TeacherSubjectAssignment> TeacherSubjectAssignment => Set<TeacherSubjectAssignment>();
        public DbSet<TeacherRatings> Rates => Set<TeacherRatings>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<TeacherAvailability>()
                .HasQueryFilter(x => !x.IsDelete);

            builder.Entity<TeacherSubjectAssignment>()
                .HasQueryFilter(x => !x.IsActive);

            builder.Entity<TeacherRatings>()
                .HasIndex(x=>new { x.StudentId, x.TeacherGradeLevelId }).IsUnique();
        }
        public override async Task<int> SaveChangesAsync( CancellationToken cancellationToken = default)
        {
            var entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
               .Where(e => e.Entity.DomainEvents.Any())
               .Select(e => e.Entity)
               .ToList();
            var domainEvents = entitiesWithEvents
               .SelectMany(e => e.DomainEvents)
               .ToList();
            foreach (var entity in entitiesWithEvents)
                entity.ClearDomainEvents();

            var result = await base.SaveChangesAsync(cancellationToken);

           
            return result;
        }
    }
}
