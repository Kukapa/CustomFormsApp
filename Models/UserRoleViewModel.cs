using Microsoft.AspNetCore.Identity;

namespace CustomFormsApp.Models
{
    public class UserRoleViewModel
    {
        public IdentityUser User { get; set; }
        public IList<string> Roles { get; set; }
    }
}
