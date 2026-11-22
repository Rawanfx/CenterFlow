using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CenterFlow.Application.Common.Exceptions;
using CenterFlow.Domain.Enum;

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
            var teacher =await context.Teachers.FirstOrDefaultAsync(x => x.Id == userService.UserId);
            if (teacher == null)
                throw new NotFoundException("Teacher not found");
            var bookedRooms =await context.Books
                .Where(x => x.Date == request.date 
                && x.From < request.To && x.To > request.From
                && x.Status == BookingStatus.Confirmed)
                .Select(x => x.RoomId)
                .ToListAsync(cancellationToken);
            var freeRooms =  context.Rooms
                .Where(r => !bookedRooms.Contains(r.Id));
            if (request.MinCapacity.HasValue)
                freeRooms=freeRooms.Where(x=> x.Capacity >= request.MinCapacity.Value);
            var freeRoomsList = await freeRooms
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
                Data = freeRoomsList,
                Success =true,
                
            };
        }
    }
}
