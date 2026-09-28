using CenterFlow.Application.Common.Exceptions;
using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Application.Common.Models;
using CenterFlow.Application.Features.Auth.Common;
using CenterFlow.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CenterFlow.Application.Features.Auth.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Response<LoginResult>>
    {
        private readonly IJwtService jwtService;
        private readonly UserManager<ApplicationUser> userManager;
        public LoginCommandHandler(IJwtService jwtService,UserManager<ApplicationUser> userManager)
        {
            this.jwtService = jwtService;
            this.userManager = userManager;
        }
        public async Task<Response<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null || await userManager.CheckPasswordAsync(user, request.Password))
                throw new IdentityException(new List<string>{ "Error in email or password" });
            string token = await jwtService.GenerateJwtTonen(user);
            // string refreshToken = 
            return new Response<LoginResult>()
            {
                Data = new LoginResult()
                {
                    Token = token
                },Success=true
            };
        }
    }
}
