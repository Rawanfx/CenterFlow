
using CenterFlow.Application.Common.Exceptions;
using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Application.Common.Models;
using CenterFlow.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CenterFlow.Application.Features.GradeLevel.AddGradeLevel
{
    public class AddGradeLevelCommandHandler : IRequestHandler<AddGradeLevelCommand, Response<Guid>>
    {
        private readonly IAppDbContext context;
        private readonly ICurrentUserService userService;
        public AddGradeLevelCommandHandler(IAppDbContext context, ICurrentUserService userService   )
        {
            this.context = context;
            this.userService = userService;
        }
        public async Task<Response<Guid>> Handle(AddGradeLevelCommand request, CancellationToken cancellationToken)
        {
            var admin = context.CenterAdmins.SingleAsync(x => x.Id == userService.UserId);
            if (await context.GradeLevels.AnyAsync(x => x.Name.Trim() == request.Name.Trim() && x.IsActive))
                throw new ConflictException("This grade level is already excit");
            var gradeLevel = new CenterFlow.Domain.Entities.GradeLevel()
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                IsActive =true,
                Name = request.Name,
            };
            context.GradeLevels.Add(gradeLevel);
           await context.SaveChangesAsync(cancellationToken);
            return new Response<Guid>() { Success = true, Data = gradeLevel.Id };
        }
    }
}
