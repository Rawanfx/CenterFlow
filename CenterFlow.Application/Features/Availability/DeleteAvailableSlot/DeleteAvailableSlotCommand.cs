using CenterFlow.Application.Common.Models;
using MediatR;

namespace CenterFlow.Application.Features.Availability.DeleteAvailableSlot;

public record DeleteAvailableSlotCommand(Guid AvailableId) : IRequest<Unit>;
