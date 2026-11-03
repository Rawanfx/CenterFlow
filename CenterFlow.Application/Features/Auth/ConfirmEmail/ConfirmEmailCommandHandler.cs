using CenterFlow.Application.Common.Exceptions;
using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Application.Common.Models;
using CenterFlow.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CenterFlow.Application.Features.Auth.ConfirmEmail
{
    public class ConfirmEmailCommandHandler: IRequestHandler<ConfirmEmailCommand, Response<string>>
    {
        private readonly IEmailService emailService;
        private readonly UserManager<ApplicationUser> userManager; 
        public ConfirmEmailCommandHandler(IEmailService emailService,UserManager<ApplicationUser> userManager)
        {
            this.emailService = emailService;
            this.userManager = userManager;
        }
        public async Task<Response<string>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new NotFoundException("User not found");
            var decodedToken = Uri.UnescapeDataString(request.Token);
            var result = await userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
                throw new IdentityException(result.Errors.Select(e => e.Description).ToList());
            return new Response<string>
            {
                Success = true,
                Message = "Email Confirmed"
            };
        }
    
    }
}
