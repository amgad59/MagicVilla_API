using Microsoft.AspNetCore.Identity;

namespace MagicVilla_VillaAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string name { get; set; }
    }
}
