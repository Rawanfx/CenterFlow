using CenterFlow.Application.Common.Models;
using MediatR;

namespace CenterFlow.Application.Features.Availability.GetFreeRooms;

    public record GetFreeRoomQuery(DateOnly date,TimeSpan From ,TimeSpan To) : IRequest<Response<List<FreeRoomDto>>>;
    public class FreeRoomDto
    {
        public Guid RoomId { get; set; }
        public string RoomName { get; set; }
        public int Capacity { get; set; }
        public TimeSpan From { get;set; }
        public TimeSpan To { get;set; }
    }
