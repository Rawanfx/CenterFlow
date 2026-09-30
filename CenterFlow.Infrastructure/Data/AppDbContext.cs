using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Domain.Common;
using CenterFlow.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace CenterFlow.Infrastructure.Data
{
    public class AppDbContext:IdentityDbContext<ApplicationUser>,IAppDbContext
    {
        private readonly IServiceProvider serviceProvider;
        public AppDbContext(DbContextOptions<AppDbContext> options,IServiceProvider serviceProvider) :
            base(options) {
            this.serviceProvider = serviceProvider;
        }
        public DbSet<Teacher> Teachers => Set<Teacher>();
        public DbSet<Book> Books => Set<Book>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<StudentBooking> StudentBookings => Set<StudentBooking>();
        public DbSet<Subject> Subjects => Set<Subject>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<TeacherAvailability> TeacherAvailabilities => Set<TeacherAvailability>();
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<TeacherAvailability>()
                .HasQueryFilter(x => !x.IsDelete);
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

            if (domainEvents.Any())
            {
                using var scope = serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                foreach (var domainEvent in domainEvents)
                    await mediator.Publish(domainEvent, cancellationToken);
            }

            return result;
        }
    }
}
