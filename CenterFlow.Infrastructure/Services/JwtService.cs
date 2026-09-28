using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Domain.Entities;
using CenterFlow.Infrastructure.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CenterFlow.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtSetting jwtSetting;
        private readonly UserManager<ApplicationUser> userManager;
        public JwtService (IOptions<JwtSetting> options
            , UserManager<ApplicationUser> userManager)
        {
            jwtSetting = options.Value;
            this.userManager = userManager;
        }
        public async Task<string> GenerateJwtTonen(ApplicationUser user)
        {
            var userRole = await userManager.GetRolesAsync(user);
            
            var claims = new List<Claim>()
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email!),
            };
            claims.Add(new Claim(ClaimTypes.Role, userRole.FirstOrDefault()));
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
        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
