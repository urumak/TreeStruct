using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using TreeStruct.Models;

namespace TreeStruct.Infrastructure
{
    //klasa odpowiedzilna za dane użytkowników
    public class AppUserManager : UserManager<AppUser>
    {
        //konstruktor wywołuje również bazowy z takimi samymi parametrzmi
        public AppUserManager(IUserStore<AppUser> store) : base(store) { }

        //tworzenie instancji
        public static AppUserManager Create(
            IdentityFactoryOptions<AppUserManager> options,
            IOwinContext context)
        {
            //dostęp do bazy danych
            IdentityDatabaseContext db = context.Get<IdentityDatabaseContext>();
            //stworzenie obiektu z odpowiednimi danymi
            AppUserManager manager = new AppUserManager(new UserStore<AppUser>(db));

            //walidacja danych dla hasła użytkownika
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireDigit = true,
                RequireUppercase = true,
            };

            //walidacja danych dla nazwy użytkownika
            manager.UserValidator = new UserValidator<AppUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = true,
                RequireUniqueEmail = true
            };

            //zwrócenie instancji klasy
            return manager;
        }
    }
}