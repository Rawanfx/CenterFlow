
using Microsoft.AspNetCore.Identity;

namespace CenterFlow.Domain.Entities
{
    public class ApplicationUser:IdentityUser
    {
        public string FullName { get; set; }
    }
}
