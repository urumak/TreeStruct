using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using TreeStruct.Infrastructure;
using TreeStruct.Models;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using System.Security.Claims;

namespace TreeStruct.Controllers
{
    public class UserController : Controller
    {
        //tworzenie użytkownika
        public ActionResult Create()
        {
            //jeśli użytkownik jest zalogowany to nie ma dostępu do akcji
            if(User.Identity.IsAuthenticated)
            {
                return View("Error", new string[] { "Access Denied" });
            }
            return View();
        }

        //metoda post jest asynchroniczna
        [HttpPost]
        public async Task<ActionResult> Create(CreateModel model)
        {
            //jeśli dane wprowadzono prawidłowo
            if (ModelState.IsValid)
            {
                //tworzę nowego użytkownika, z danymi zapisanymi w modelu
                AppUser user = new AppUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };
                //próba utworzenia nowego użytkownika
                //UserManager to dokładnie HttpContext.GetOwinContext().GetUserManager<AppUserManager>()
                //właściwość znajduje się pod koniec pliku
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //jeśli dane są prawidłowe i tworzenie użytkownika zakończył się sukcesem
                    //to przekierowuję na stronę główną
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    //jeśli tworzenie użytkownika nie powiodło się
                    //podaj odpowiednie błędy
                    //metoda znajduje się pod koniec pliku
                    AddErrorsFromResult(result);
                }
            }
            //jeśli tworzenie użytkownika nie powiodło się lub dane są niezgodne z modelem to zostanie wykonana ta instrukcja
            //powrót do widoku w celu poprawnego wpisania danych
            //pojawi się informacja o tym, co zostało źle wpisane
            return View(model);
        }

        public ActionResult Login(string returnUrl)
        {
            //jeśli użytkownik jest zalogowany to nie może zalogować się ponownie
            if (User.Identity.IsAuthenticated)
            {
                return View("Error", new string[] { "Access Denied" });
            }
            //zapamiętuję poprzedni adres url
            ViewBag.returnUrl = returnUrl;
            return View();
        }


        [HttpPost]
        //zabezpieczenie przed atakiem CSRF
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel details, string returnUrl)
        {

            //jesli wpisano dane zgone z modelem
            if (ModelState.IsValid)
            {
                //wyszukuję odpowieniego użytkownika
                AppUser user = await UserManager.FindAsync(details.UserName, details.Password);
                //jeśli nie istnieje użytkownikmo takich danych, dodaję informację o tym że dane są nieprawidłowe
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid name or password.");
                }
                else
                {
                    //jeśli dane są poprawne idenyfikuję użytkownika
                    ClaimsIdentity identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                    AuthManager.SignOut();
                    AuthManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);
                    //na koniec przekierowuje na odpowiednią stronę
                    if (returnUrl != null && returnUrl != "")
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            ViewBag.returnUrl = returnUrl;
            return View(details);
        }

        [Authorize]
        public ActionResult Logout()
        {
            //wylogowanie użytkownika
            AuthManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //odpowiada za autentykację
        private IAuthenticationManager AuthManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        //dodawanie błędów do modelu
        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        //odpowiada za dane użytkowników
        public AppUserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); }
        }
    }
}