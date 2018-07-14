using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using TreeStruct.Infrastructure;
namespace Users
{
    public class IdentityConfig
    {
        public void Configuration(IAppBuilder app)
        {
            //konfiguracja identyfikacji
            app.CreatePerOwinContext<IdentityDatabaseContext>(IdentityDatabaseContext.Create);
            app.CreatePerOwinContext<AppUserManager>(AppUserManager.Create);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                //ścieżka do logowania
                LoginPath = new PathString("/User/Login"),
            });
        }
    }
}