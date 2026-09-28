using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Domain.Entities;
using CenterFlow.Infrastructure.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CenterFlow.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtSetting jwtSetting;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        public JwtService (IOptions<JwtSetting> options
            ,RoleManager<IdentityRole> roleManager
            , UserManager<ApplicationUser> userManager)
        {
            jwtSetting = options.Value;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        public async Task<string> GenerateJwtTonen(ApplicationUser user)
        {
            var role = await roleManager.user
            var claims = new List<Claim>()
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email!),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.Key));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: jwtSetting.Issuer,
                audience: jwtSetting.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtSetting.Expire),
                signingCredentials: cred
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
