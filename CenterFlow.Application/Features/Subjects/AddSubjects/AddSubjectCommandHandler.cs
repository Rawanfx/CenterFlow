using CenterFlow.Application.Common.Exceptions;
using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CenterFlow.Application.Features.Subjects.AddSubjects
{
    public class AddSubjectCommandHandler : IRequestHandler<AddSubjectCommand, Response<Guid>>
    {
        private readonly IAppDbContext context;
        private readonly ICurrentUserService userService;
        public AddSubjectCommandHandler (IAppDbContext context, ICurrentUserService userService)
        {
            this.context = context;
            this.userService = userService;
        }
        public async Task<Response<Guid>> Handle(AddSubjectCommand request, CancellationToken cancellationToken)
        {
            var admin = await context.CenterAdmins.SingleAsync(x => x.Id == userService.UserId);

            if (await context.Subjects.AnyAsync(x => x.Name.Trim() == request.Name.Trim() && x.IsActive))
                throw new ConflictException("This subject is already excit");
            var subject = new Domain.Entities.Subject
            {
                Name = request.Name,
                CreatedBy = admin.Id,
                CreatedAt = DateTime.UtcNow,
                Id = Guid.NewGuid(),
            };
            await context.Subjects.AddAsync(subject);
            await context.SaveChangesAsync(cancellationToken);
            return new Response<Guid>() { Success =true,
                Message = "Subject added successfully",
                Data = subject.Id
            };
        }
    }
}
