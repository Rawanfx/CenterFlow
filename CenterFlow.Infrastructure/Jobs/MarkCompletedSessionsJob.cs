using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CenterFlow.Infrastructure.Jobs
{
    public class MarkCompletedSessionsJob : IMarkCompletedSessionsJob
    {
        private readonly IAppDbContext context;
        public MarkCompletedSessionsJob (IAppDbContext context)
        {
            this.context = context;
        }
        public async Task Execute()
        {
            var now = DateTime.UtcNow;
            var today = DateOnly.FromDateTime(now);
            var book = await context.Books
                .Where(x => (x.Date < today || (x.Date == today && x.To <= now.TimeOfDay))
                && x.Status != Domain.Enum.BookingStatus.Completed
                && x.Status!=Domain.Enum.BookingStatus.Cancelled
                ).ToListAsync();
            foreach (var i in book)
            {
                i.Status = Domain.Enum.BookingStatus.Completed;
            }
            await context.SaveChangesAsync();
        }
    }
}
