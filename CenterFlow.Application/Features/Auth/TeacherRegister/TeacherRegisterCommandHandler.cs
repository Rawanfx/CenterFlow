using CenterFlow.Application.Common.Exceptions;
using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Application.Common.Models;
using CenterFlow.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CenterFlow.Application.Features.Auth.TeacherRegister
{
    public class TeacherRegisterCommandHandler : IRequestHandler<TeacherRegisterCommand, Response<string>>
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IAppDbContext context;
        public TeacherRegisterCommandHandler(UserManager<ApplicationUser> userManager
            ,IAppDbContext context
            )
        {
            this.context = context;
            
            this.userManager = userManager;
        }
        public async Task<Response<string>> Handle(TeacherRegisterCommand request, CancellationToken cancellationToken)
        {
            var found = await userManager.FindByEmailAsync(request.Email);
            if (found!=null)
                throw new IdentityException(new List<string>() { "this email has been registerd" });
            var subject = await context.Subjects.FirstOrDefaultAsync(x => x.Id == request.SubjectId);
            if (subject == null)
                throw new NotFoundException("Subject not found");
            Teacher teacher = new Teacher()
            {
                Email = request.Email,
                PhoneNumber = request.Phone,
                FullName = request.FullName,
                UserName = request.Email,
                SubjectId = request.SubjectId,
            };
          var result=  await userManager.CreateAsync(teacher, request.Password);
            if (!result.Succeeded)
            {
                throw new IdentityException(result.Errors.Select(x => x.Description).ToList());
            }
            var roleResult = await userManager.AddToRoleAsync(teacher, "Teacher");
            if (!roleResult.Succeeded)
            {
                await userManager.DeleteAsync(teacher);
                throw new IdentityException(result.Errors.Select(x => x.Description).ToList());
            }
            return new Response<string>() { Success = true, Message = "Registed Successfully" };
        }
    }
}
