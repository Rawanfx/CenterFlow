using CenterFlow.Application.Common.Exceptions;
using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CenterFlow.Application.Features.Availability.DeleteAvailableSlot
{
    public class DeleteAvailableSlotCommandHandler : IRequestHandler<DeleteAvailableSlotCommand, Unit>
    {
        private readonly IAppDbContext context;
        private readonly ICurrentUserService userService;
        public DeleteAvailableSlotCommandHandler(IAppDbContext context,ICurrentUserService userService)
        {
            this.userService = userService;
            this.context = context;
        }
        public async Task<Unit> Handle(DeleteAvailableSlotCommand request, CancellationToken cancellationToken)
        {
            var availableSlot = await context.TeacherAvailabilities
                .FirstOrDefaultAsync(x => x.TeacherId == Guid.Parse(userService.UserId)
                && x.Id == request.AvailableId
               ,cancellationToken);
            if (availableSlot == null)
                throw new NotFoundException("Availability slot not found.");
            availableSlot.IsDelete = true;
            await context.SaveChangesAsync();
            return Unit.Value;
        }
    }
}
