using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CenterFlow.Application.Common.Exceptions;

namespace CenterFlow.Application.Features.Availability.GetFreeRooms
{
    public class GetFreeRoomQueryHandler : IRequestHandler<GetFreeRoomQuery, Response<List<FreeRoomDto>>>
    {
        private readonly IAppDbContext context;
        public readonly ICurrentUserService userService;
        public GetFreeRoomQueryHandler(IAppDbContext context,ICurrentUserService userService)
        {
            this.context = context;
            this.userService = userService;
        }
        public async Task<Response<List<FreeRoomDto>>> Handle(GetFreeRoomQuery request, CancellationToken cancellationToken)
        {
            var teacher = context.Teachers.FirstOrDefaultAsync(x => x.Id == userService.UserId);
            if (teacher == null)
                throw new NotFoundException("Teacher not found");
            var bookedRooms =await context.Books
                .Where(x => x.Date == request.date && x.From < request.To && x.To > request.From)
                .Select(x => x.RoomId)
                .ToListAsync(cancellationToken);
            var freeRooms = await context.Rooms
                .Where(r => !bookedRooms.Contains(r.Id))
                .Select(r => new FreeRoomDto
                {
                    RoomId = r.Id,
                    RoomName = r.Name,
                    Capacity = r.Capacity,
                    From = request.From,
                    To = request.To
                })
                .ToListAsync(cancellationToken);
            return new Response<List<FreeRoomDto>>()
            {
                Data = freeRooms,
                Success =true,
                
            };
        }
    }
}
