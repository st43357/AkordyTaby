using DiplomovaPrace.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Net;

namespace DiplomovaPrace.Controllers
{

 
    
    [Authorize]
    public class ZadostController : Controller
    {
        // GET: Zadost - zobrazí podrobnosti žádosti s daným ID + možnost upravit stav žádosti
        public ActionResult KonkretniZadost(int id)
        {

            using (ModelContainer db = new ModelContainer())
            {
                Zadost vyhledavanaZadost = db.Zadosti.Include(x => x.Interpret).Include(x => x.ZadostOd).Include(x => x.Prispevek).Where(x => x.Id == id).First();

                //uložím ID do Session pro pozdější použití
                Session["idKonkretniZadost"] = vyhledavanaZadost.Id;

                //zjistím aktuální počet nepřečtených zpráv
                int idPrihlaseneho = int.Parse(Session["uzivatelID"].ToString());
                Uzivatel prihlasenyUzivatel = db.Uzivatele.Where(x => x.Id == idPrihlaseneho).First();
                var pocetNeprectenychZprav = db.Zpravy.Where(x => x.UzivatelKomuId == prihlasenyUzivatel.Id).Where(x => x.CasOdeslani > prihlasenyUzivatel.PosledniAktivita).ToList();
                Session["neprecteneZpravy"] = pocetNeprectenychZprav.Count();

                vyhledavanaZadost.Komentar = WebUtility.HtmlDecode(vyhledavanaZadost.Komentar);

                return View(vyhledavanaZadost);
            }
        }

        [HttpPost]
        public ActionResult KonkretniZadost(String novyStavZadosti)
        {

            using (ModelContainer db = new ModelContainer())
            {

                //získám ID žádosti
                int idZadosti = int.Parse(Session["idKonkretniZadost"].ToString());

                //vyhledám žádost kterou upravuji
                Zadost upravovanaZadost = db.Zadosti.Include(x => x.Interpret).Include(x => x.ZadostOd).Include(x => x.Prispevek).Where(x => x.Id == idZadosti).First();

                //změním stav žádosti
                upravovanaZadost.StavZadosti = ZjistiStavZadosti(novyStavZadosti);

                //změním stav objektu
                db.Entry(upravovanaZadost).State = System.Data.Entity.EntityState.Modified;

                upravovanaZadost.Komentar = WebUtility.HtmlEncode(upravovanaZadost.Komentar);


                //NÁSLEDKY ZMĚNY STAVU ŽÁDOSTI dle typu žádosti a novém stavu
                if (upravovanaZadost.TypZadosti == TypZadosti.ZadostPisen)
                {

                    if (upravovanaZadost.StavZadosti == StavZadosti.Schvalena)
                    {
                        //změním Schvaleno u Prispevek na true
                        Prispevek zmenaViditelnosti = db.Prispevky.Where(x => x.Id == upravovanaZadost.PrispevekId).First();

                        zmenaViditelnosti.Schvalen = true;

                        db.Entry(zmenaViditelnosti).State = EntityState.Modified;
                    }
                    else //StavZadosti.Zamitnuta
                    {
                        //změním Schvaleno u Prispevek na false
                        Prispevek zmenaViditelnosti = db.Prispevky.Where(x => x.Id == upravovanaZadost.PrispevekId).First();

                        zmenaViditelnosti.Schvalen = false;

                        db.Entry(zmenaViditelnosti).State = EntityState.Modified;
                    }

                }
                else //TypZadosti.ZadostRedaktor
                {

                    int idPrihlasenehoUzivatele = int.Parse(Session["uzivatelID"].ToString());

                    if (upravovanaZadost.StavZadosti == StavZadosti.Schvalena)
                    {
                        //přidám uživateli roli v databázové tabulce Uzivatele
                        Uzivatel pridanaRole = db.Uzivatele.Where(x => x.Id == upravovanaZadost.ZadostOd.Id).First();

                        pridanaRole.Role = TypRoleUzivatele.Redaktor;

                        db.Entry(pridanaRole).State = EntityState.Modified;

                        //přidám uživateli roli v databázové tabulce AspUserRoles
                        RoleManager<IdentityRole> spravceRoli = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new IdentityDbContext()));
                        UserManager<ApplicationUser> spravceUzivatelu = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                        ApplicationUser uzivatel = spravceUzivatelu.FindByName(pridanaRole.Prezdivka);
                        spravceUzivatelu.AddToRole(uzivatel.Id, "redaktor");

                    }  else {//pokud není žádost schválena - nic se nemění, pouze pokud již byla schválena předtím tak odeberu roli redaktora

                        //odeberu uživateli roli v databázové tabulce Uzivatele
                        Uzivatel pridanaRole = db.Uzivatele.Where(x => x.Id == upravovanaZadost.ZadostOd.Id).First();

                        pridanaRole.Role = TypRoleUzivatele.PrihlasenyUzivatel;

                        db.Entry(pridanaRole).State = EntityState.Modified;

                        //přidám uživateli roli v databázové tabulce AspUserRoles
                        RoleManager<IdentityRole> spravceRoli = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new IdentityDbContext()));
                        UserManager<ApplicationUser> spravceUzivatelu = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                        ApplicationUser uzivatel = spravceUzivatelu.FindByName(pridanaRole.Prezdivka);
                        spravceUzivatelu.RemoveFromRole(uzivatel.Id, "redaktor");
                    }
                }

                //uložím změny
                db.SaveChanges();

                //return View(upravovanaZadost);
                return RedirectToAction("KonkretniZadost", new { id = idZadosti } );
            }
        }

        //Validace nastavena na false - i při [AllowHtml] stále upozornění na možný útok kvůli HTML vstupu
        [HttpPost, ValidateInput(false)]
        public ActionResult PridatKomentar(String textKomentare)
        {
            using (ModelContainer db = new ModelContainer())
            {
                //získám ID žádosti
                int idZadosti = int.Parse(Session["idKonkretniZadost"].ToString());

                //vyhledám žádost kterou upravuji
                Zadost upravovanaZadost = db.Zadosti.Include(x => x.Interpret).Include(x => x.ZadostOd).Include(x => x.Prispevek).Where(x => x.Id == idZadosti).First();

                //změnim komentář u žádosti
                upravovanaZadost.Komentar = textKomentare;

                upravovanaZadost.Komentar = WebUtility.HtmlEncode(upravovanaZadost.Komentar);

                //změním stav objektu
                db.Entry(upravovanaZadost).State = System.Data.Entity.EntityState.Modified;

                //uložím změny
                db.SaveChanges();

                //return View(upravovanaZadost);
                return RedirectToAction("KonkretniZadost", new { id = idZadosti });

            }
        }

        //zjistí stav žádosti dle zadaného parametru
        private StavZadosti ZjistiStavZadosti(String stav)
        {

            switch(stav)
            {
                case "schvalena": return StavZadosti.Schvalena;
                case "cekajici": return StavZadosti.Cekajici;
                case "zamitnuta": return StavZadosti.Zamitnuta;
            }

            //nenastane
            return StavZadosti.Cekajici;
        }

    }

}