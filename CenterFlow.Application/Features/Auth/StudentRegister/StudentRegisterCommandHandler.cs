using CenterFlow.Application.Common.Exceptions;
using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Application.Common.Models;
using CenterFlow.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CenterFlow.Application.Features.Auth.StudentRegister
{
    public class StudentRegisterCommandHandler : IRequestHandler<StudentRegisterCommand, Response<string>>
    {
        private readonly UserManager<ApplicationUser> userManager;
        public StudentRegisterCommandHandler(UserManager<ApplicationUser> userManager
            )
        {
            this.userManager = userManager;
        }
        public async Task<Response<string>> Handle(StudentRegisterCommand request, CancellationToken cancellationToken)
        {
            var found = await userManager.FindByEmailAsync(request.Email);
            if (found != null)
                throw new IdentityException(new List<string>() { "this email has been registerd" });
            var student = new Student()
            {
                Email = request.Email,
                UserName = request.Email,
                FullName = request.FullName,
                GradeLevel = request.Level
            };

            var result = await userManager.CreateAsync(student, request.Password);
            if (!result.Succeeded)
                throw new IdentityException(result.Errors.Select(x => x.Description).ToList());
            var roleResult = await userManager.AddToRoleAsync(student, "Student");
            if (!roleResult.Succeeded)
            {
                await userManager.DeleteAsync(student);
                throw new IdentityException(roleResult.Errors.Select(x => x.Description).ToList());
            }
            return new Response<string>()
            {
                Success = true,
                Message = "Registed Successfully"
            };
        }
    }
}
