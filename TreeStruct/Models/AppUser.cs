using Microsoft.AspNet.Identity.EntityFramework;

namespace TreeStruct.Models
{
    //klasa użytkownika dziedziczy po IdentityUser z ASP.NET Identity
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}