using CenterFlow.Application.Common.Exceptions;
using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Application.Common.Models;
using CenterFlow.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CenterFlow.Application.Features.Availability.AddAvailableSessionsForTeacher
{
    public class AddAvailableSessionCommandHandler : IRequestHandler<AddAvailableSessionCommand, Response<Guid>>
    {
        private readonly ICurrentUserService userService;
        private readonly IAppDbContext context;
        public AddAvailableSessionCommandHandler(ICurrentUserService userService,
           IAppDbContext context)
        {
            this.context = context;
            this.userService = userService;
        }
        public async Task<Response<Guid>> Handle(AddAvailableSessionCommand request, CancellationToken cancellationToken)
        {
            var teacher = await context.Teachers.FirstOrDefaultAsync(x => x.Id == userService.UserId,cancellationToken);
            if (teacher == null)
                throw new ForbiddenException("Only teachers can manage availability.");

            var available = await context.TeacherAvailabilities
                .AnyAsync(x => x.TeacherId == Guid.Parse(teacher.Id)
                && x.DayOfWeek == request.Day
               && x.From < request.To &&
                !x.IsDelete&&
                x.To > request.From,cancellationToken);

            if (available)
                throw new ConflictException("This availability overlaps with an existing one.");

            var teacherAvailabe = new TeacherAvailability()
            {
                DayOfWeek = request.Day,
                From = request.From,
                To = request.To,
                TeacherId = Guid.Parse(teacher.Id),
                Id = Guid.NewGuid()
                
            };
            await context.TeacherAvailabilities.AddAsync(teacherAvailabe,cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return new Response<Guid>()
            {
                Data = teacherAvailabe.Id,
                Success = true,
                Message = "Availability added successfully."
            };
        }
    }
}
