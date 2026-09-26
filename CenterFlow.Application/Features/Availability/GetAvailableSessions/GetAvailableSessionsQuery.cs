using CenterFlow.Application.Common.Models;
using MediatR;

namespace CenterFlow.Application.Features.Availability.GetAvailableSlots;

public record GetAvailableSessionsQuery(Guid TeacherId,DateOnly date):IRequest<Response<List<GetAvailableSessionsDto>>>;
public class GetAvailableSessionsDto
{
    public Guid BookId { get; set; }
    public string RoomName { get; set; }
    public TimeSpan From { get; set; }
    public TimeSpan To { get; set; }
    public int AvaliableSeats { get; set; }

}

