using DiplomovaPrace.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static DiplomovaPrace.Models.HomeViewModels;

namespace DiplomovaPrace.Controllers
{

    [Authorize]
    public class HomeController : Controller
    {

        //počet záznamů které zobrazím v každé kategorii nejnovějších (interpreti, příspěvky, články)
        private static int POCET_ZAZNAMU = 5;

        /* GET /Home/Index
         *Vrací základní přehled novinek a zároveň rozcestník mezi interpreti, příspěvky a články.
         */
        public ActionResult Index()
        {

            HomeIndexViewModel model = new HomeIndexViewModel();

            using(ModelContainer db = new ModelContainer())
            {          
            model.NejnovejsiInterpreti = db.Interpreti.OrderByDescending(x => x.DatumVytvoreni).Take(POCET_ZAZNAMU).ToList();
            model.NejnovejsiClanky = db.Clanky.OrderByDescending(x => x.DatumVytvoreni).Take(POCET_ZAZNAMU).ToList();
            model.NejnovejsiPrispevky = db.Prispevky.Where(x => x.Schvalen == true).OrderByDescending(x => x.DatumPridani).Take(POCET_ZAZNAMU).ToList();

            //zjistím zda existují případně kolik nevyřízené žádosti - pokud ano oznámím ve View přihlášenému uživateli
            int idPrihlaseneho = int.Parse(Session["uzivatelID"].ToString());
            int pocetNevyrizenychZadosti = db.Zadosti.Where(x => x.ZadostKomu.Id == idPrihlaseneho).Where(x => x.StavZadosti == StavZadosti.Cekajici).Count();
            model.pocetNevyrizenychZadosti = pocetNevyrizenychZadosti;

            return View(model);
            }
        }

        // GET /Home/PodporaKontakt
        public ActionResult PodporaKontakt()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PodporaKontakt([Bind(Include = "Predmet, Text")] PodporaKontaktViewModel model)
        {
            //získám instanci AccountControlleru
            var controller = DependencyResolver.Current.GetService<AccountController>();
            controller.ControllerContext = new ControllerContext(this.Request.RequestContext, controller);

            //zavolám metodu AccountControlleru pro odeslání emailu
            controller.SendEmail(model.Predmet, model.Text);

            return View("PodporaKontaktPotvrzeni");
        }

        public ActionResult PodporaKontaktPotvrzeni()
        {
            return View();
        }


    }


}