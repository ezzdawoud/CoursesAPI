using Microsoft.AspNetCore.Identity;

namespace Courses.Models
{
    public class Users:IdentityUser
    {   
        public string? UsersPictrues {  get; set; }
    }
}
