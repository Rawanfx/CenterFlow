using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Domain.Entities;

namespace CenterFlow.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        public Task<string> GenerateJwtTonen(ApplicationUser user)
        {
            throw new NotImplementedException();
        }
    }
}
