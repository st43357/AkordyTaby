using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using DiplomovaPrace.Models;
using System.Text;
using System.Collections.Generic;
using System.Data.Entity;

namespace DiplomovaPrace.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        //počty příspěvků pro změny hodnosti uživatele na vyšší
        private const int MAX_ZACINAJICI_HUDEBNIK = 25;
        private const int MAX_POKROCILY_MUZIKANT = 50;
        private const int MAX_ZKUSENY_SKLADATEL = 90;
        private const int MAX_ZNAMA_CELEBRITA = 150;
        private const int MAX_HUDEBNI_BUH = 200;


   

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            userManager.PasswordHasher = new SQLPasswordHasher();
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //dopsáno
            UserManager.PasswordHasher = new SQLPasswordHasher();

            // Require the user to have a confirmed email before they can log on.
            var user = await UserManager.FindByNameAsync(model.Prezdivka);
            //var user = UserManager.Find(model.Prezdivka, model.Heslo);
            if (user != null)
            {
                if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Potvrzení emailu - opětovné poslání");
                    ViewBag.errorMessage = "Před prvním přihlášením musíte potvrdit email.";
                    return View("Error");
                }
            }


            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Prezdivka, model.Heslo, model.Zapamatovat, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:

                    //po úspěšném přihlášení uložím informace o uživateli se kterými budu dále pracovat
                    //Context vlastních tabulek
                    using (ModelContainer db = new ModelContainer())
                    {
                        var prihlasenyUzivatel = db.Uzivatele.Where(u => u.Prezdivka == model.Prezdivka).FirstOrDefault();

                        Session["uzivatelID"] = prihlasenyUzivatel.Id;
                        Session["prezdivka"] = prihlasenyUzivatel.Prezdivka;

                        //do _Layout navbar zobrazení miniatury
                        Session["avatar"] = prihlasenyUzivatel.Avatar;

                        //zjistím jestli uživatel nezasluhuje novou hodnost
                        int pocetPrispevku = db.Prispevky.Where(x => x.UzivatelId == prihlasenyUzivatel.Id).Count();
                        
                        if((pocetPrispevku > MAX_ZACINAJICI_HUDEBNIK) && (pocetPrispevku < MAX_POKROCILY_MUZIKANT)) {
                            prihlasenyUzivatel.Hodnost = TypHodnostUzivatele.PokrocilyMuzikant;
                        } else if((pocetPrispevku > MAX_POKROCILY_MUZIKANT) && (pocetPrispevku < MAX_ZKUSENY_SKLADATEL))
                        {
                            prihlasenyUzivatel.Hodnost = TypHodnostUzivatele.ZkusenySkladatel;
                        } else if ((pocetPrispevku > MAX_ZKUSENY_SKLADATEL) && (pocetPrispevku < MAX_ZNAMA_CELEBRITA))
                        {
                            prihlasenyUzivatel.Hodnost = TypHodnostUzivatele.ZnamaCelebrita;
                        } else if ((pocetPrispevku > MAX_ZNAMA_CELEBRITA) && (pocetPrispevku < MAX_HUDEBNI_BUH))
                        {
                            prihlasenyUzivatel.Hodnost = TypHodnostUzivatele.HudebniBuh;
                        }

                        var pocetNeprectenychZprav = db.Zpravy.Where(x => x.UzivatelKomuId == prihlasenyUzivatel.Id).Where(x => x.CasOdeslani > prihlasenyUzivatel.PosledniAktivita).ToList();
                        Session["neprecteneZpravy"] = pocetNeprectenychZprav.Count();
       
                        //upravím datum a čas poslední aktivity
                        //prihlasenyUzivatel.PosledniAktivita = DateTime.Now;

                        //pouze změna hodnoty - nastavím EntityState
                        db.Entry(prihlasenyUzivatel).State = System.Data.Entity.EntityState.Modified;


                        //uložím změny do databáze - poslední aktivita uživatele
                        db.SaveChanges();

       
                    }

                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.Zapamatovat });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");

                    //uložím informaci kterou zobrazím uživateli o neexistujícím uživateli
                    ViewBag.NeexistujiciUzivatel = "Neexistující kombinace přezdívky a hesla.";
                    return View(model);
            }
        }

        //GET /Account/FAQ
        [AllowAnonymous]
        public ActionResult FAQ()
        {
            return View();
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {

            //dopsáno
            UserManager.PasswordHasher = new SQLPasswordHasher();

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Prezdivka, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Heslo);
                if (result.Succeeded)
                {
                    Uzivatel novyUzivatel = new Uzivatel();
                    novyUzivatel.Avatar = model.ProfiloveFoto;                  
                    novyUzivatel.DatumRegistrace = DateTime.Now;
                    novyUzivatel.Email = model.Email;
                    novyUzivatel.Heslo = model.Heslo;
                    novyUzivatel.PosledniAktivita = DateTime.Now;
                    novyUzivatel.Prezdivka = model.Prezdivka;
                    novyUzivatel.Prijmeni = model.Prijmeni;
                    novyUzivatel.Role = TypRoleUzivatele.PrihlasenyUzivatel;
                    novyUzivatel.Hodnost = TypHodnostUzivatele.ZacinajiciHudebnik;
                    novyUzivatel.Jmeno = model.Jmeno;
                    novyUzivatel.Mesto = model.Mesto;
                    novyUzivatel.DatumNarozeni = Convert.ToDateTime(model.DatumNarození);
                    novyUzivatel.Avatar = "~/Images/anonym.jpg";

                    //vložím nového uživatele do své tabulky uživatelů
                    using (ModelContainer db = new ModelContainer())
                    {
                        db.Uzivatele.Add(novyUzivatel);
                        db.SaveChanges();
                    }

                    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Potvrďte Váš účet.");

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                   // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                   // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //await UserManager.SendEmailAsync(user.Id, "Potvrzení účtu", "Prosím potvrďte svůj účet kliknutím <a href=\"" + callbackUrl + "\">zde</a>.");

                    // Uncomment to debug locally 
                    TempData["ViewBagLink"] = callbackUrl;

                    ViewBag.Message = "Před prvním přihlášením musíte potvrdit email.";
                    return View("Info");

                   //return RedirectToAction("Index", "Home");
                }
                AddErrors(result);

                //uložím chybu která nastala při validaci modelu kteoru zobrazím ve view
                ViewBag.ChybaRegistrace = result.Errors.FirstOrDefault();
                
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Obnovení hesla", "Heslo obnovíte kliknutím <a href=\"" + callbackUrl + "\">zde</a>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //pokud se hesla neshodují
            if(model.Heslo != model.PotvrzeniHeslo)
            {
                ViewBag.ZmenaHeslaChyba = "Hesla se neshodují!";
            }

            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Heslo);
            if (result.Succeeded)
            {
                //pokud heslo je úspěšně změněno pak uložim jeho novou hdnotu i do své tabulky uživatelů
                using(ModelContainer db = new ModelContainer())
                {
                    Uzivatel editovanyUzivatel = db.Uzivatele.Where(u => u.Email == model.Email).FirstOrDefault();
                    editovanyUzivatel.Heslo = model.Heslo;
                    db.SaveChanges();
                }

                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);

            //zobrazím chybu uživateli
            ViewBag.ZmenaHeslaNesplnenePodminky = result.Errors.FirstOrDefault();

            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // POST: /Account/LogOff
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            using(ModelContainer db = new ModelContainer())
            {
                //vyhledám přihlášeného uživatele
                int idPrihlasenehoUzivatele = int.Parse(Session["uzivatelID"].ToString());
                Uzivatel prihlasenyUzivatel = db.Uzivatele.Where(x => x.Id == idPrihlasenehoUzivatele).First();

                //změním datum a čas poslední aktivity uživatele
                //prihlasenyUzivatel.PosledniAktivita = DateTime.Now;

                //pouze upravuji hodnotu - upravím EntityState
                db.Entry(prihlasenyUzivatel).State = System.Data.Entity.EntityState.Modified;

                //uložím změny do databáze
                db.SaveChanges();
            }

            //ukončím Session
            Session.Abandon();

            return RedirectToAction("Login");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion


/*
 * Once a user creates a new local account, they are emailed a confirmation link they are required to use before they can log on.
 * If the user accidentally deletes the confirmation email, or the email never arrives, they will need the confirmation link sent again.
 * The following code changes show how to enable this.
 */ 
public async Task<string> SendEmailConfirmationTokenAsync(string userID, string subject)
{
    string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
    var callbackUrl = Url.Action("ConfirmEmail", "Account",
       new { userId = userID, code = code }, protocol: Request.Url.Scheme);
    await UserManager.SendEmailAsync(userID, subject,
       "Prosím potvrďte email kliknutím <a href=\"" + callbackUrl + "\">zde</a>");

    return callbackUrl;
}

  /*
  * odešle email s vlastním textem na email administrátora stránky
  */
        public void SendEmail(String predmet, String text)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                //zjistím id uživatele s rolí admin
                String idOdesilatele = db.Roles.Where(x => x.Name == "admin").Select(x => x.Users.FirstOrDefault().UserId).FirstOrDefault();

                String emailOdesilateleZadosti = db.Users.Where(x => x.Id == idOdesilatele).Select(x => x.Email).FirstOrDefault();
                //připojím k textu kontakt na uživatele
                text += "/nEmail uživatele: " + emailOdesilateleZadosti;

                //odeslání emailu
                UserManager.SendEmailAsync(idOdesilatele, predmet, text);
            }
        }



    }




}




 