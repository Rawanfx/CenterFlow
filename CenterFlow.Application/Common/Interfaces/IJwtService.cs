using CenterFlow.Domain.Entities;

namespace CenterFlow.Application.Common.Interfaces
{
    public interface IJwtService
    {
        Task<string> GenerateJwtTonen(ApplicationUser user);
    }
}
