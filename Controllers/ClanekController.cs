using DiplomovaPrace.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Reflection;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Net;

namespace DiplomovaPrace.Controllers
{

    [Authorize]
    public class ClanekController : Controller
    {

        private const int POCET_CLANKU = 7;
        private const int POCET_KOMENTARU = 10;

        private const int MAX_POCET_VYHLEDANYCH_CLANKU = 5;

        //vypíše všechny články na požadované stránce
        public ActionResult Index(int cisloStranky = 1)
        {
            //uložím si hodnotu aktuální stránky pro výpis ve view
            Session["aktualniStranka"] = cisloStranky;

            //seznam který naplním články z databáze dle čísla stránky
            var seznamClanku = new List<Clanek>();
            
            using (ModelContainer db = new ModelContainer())
            {
                //přeskočím články které jsou na předcházejích stránkách pomocí Skip
                seznamClanku = db.Clanky.Include(x => x.Autor).OrderByDescending(x => x.DatumVytvoreni).Skip((cisloStranky-1)* POCET_CLANKU).Take(POCET_CLANKU).ToList();

                //zjistím maximální počet stránek dle počtu článků
                double maxPocetStranek = db.Clanky.Count() / POCET_CLANKU;
               
                //uložím maximální počet stránek do Session, pomocí metody Ceiling zaokrouhlím nahoru
                Session["maxPocetStranek"] = Math.Ceiling(maxPocetStranek);

                //zjistím nový počet nepřečtených zpráv
                int idPrihlaseneho = int.Parse(Session["uzivatelID"].ToString());
                Uzivatel prihlasenyUzivatel = db.Uzivatele.Where(x => x.Id == idPrihlaseneho).First();
                var pocetNeprectenychZprav = db.Zpravy.Where(x => x.UzivatelKomuId == prihlasenyUzivatel.Id).Where(x => x.CasOdeslani > prihlasenyUzivatel.PosledniAktivita).ToList();
                Session["neprecteneZpravy"] = pocetNeprectenychZprav.Count();

                //vrátím seznam do View
                return View(seznamClanku);
            }
        }

        // GET: Clanek/ClankyKategorie/kategorie
        [HttpGet]
        [ClanekControllerAtributy("kategorie")]
        public ActionResult ClankyKategorie(String kategorie)
        {

            //uložím kategorii do Session kvůli stránkování
            Session["kategorie"] = kategorie;

            //při změně kategorie nastavím aktuální stránku na 1
            Session["aktualniStranka"] = 1;

            var seznamClanku = new List<Clanek>();

            //naplním seznam články z kategorie dle parametru
            using (ModelContainer db = new ModelContainer())
            {
                //získám seřazený seznam článků dle kategorie, není potřeba Skip -> pouze první strana článků (změna kategorie)
                seznamClanku = db.Clanky.Include(x => x.Autor).OrderByDescending(x => x.DatumVytvoreni).Where(x => x.Kategorie.ToString().Equals(kategorie)).Take(POCET_CLANKU).ToList();

                //zjistím maximální počet stránek dle počtu článků
                double maxPocetStranek = db.Clanky.Where(x => x.Kategorie.ToString().Equals(kategorie)).Count() / POCET_CLANKU;

                //uložím maximální počet stránek do Session, pomocí metody Ceiling zaokrouhlím nahoru
                Session["maxPocetStranek"] = Math.Ceiling(maxPocetStranek);

                return View(seznamClanku);
            }
        }

        // GET: Clanek/ClankyKategorie/cisloStranky
        [HttpGet]
        [ClanekControllerAtributy("cisloStranky")]
        public ActionResult ClankyKategorie(int cisloStranky = 1)
        {

            //uložím si hodnotu aktuální stránky pro výpis ve view
            Session["aktualniStranka"] = cisloStranky;

            var seznamClanku = new List<Clanek>();

            //naplním seznam články z kategorie dle parametru
            using (ModelContainer db = new ModelContainer())
            {

                String kategorie = Session["kategorie"].ToString();

                //přeskočím články které jsou na předcházejích stránkách pomocí Skip ale vyberu články pouze v kategorii
                seznamClanku = db.Clanky.Include(x => x.Autor).OrderByDescending(x => x.DatumVytvoreni).Where(x => x.Kategorie.ToString().Equals(kategorie)).Skip((cisloStranky-1) * POCET_CLANKU).Take(POCET_CLANKU).ToList();

                //zjistím maximální počet stránek dle počtu článků
                double maxPocetStranek = db.Clanky.Count() / POCET_CLANKU;

                //uložím maximální počet stránek do Session, pomocí metody Ceiling zaokrouhlím nahoru
                Session["maxPocetStranek"] = Math.Ceiling(maxPocetStranek);

                return View(seznamClanku);
            }
        }


        // GET: Clanek/KonkrektniClanek
        [HttpGet]
        public ActionResult KonkretniClanek(int id)
        {
            
            using(ModelContainer db = new ModelContainer())
            {
                //vyhledám článek v databázi dle ID článku
                Clanek clanek = db.Clanky.Include(x => x.Autor).Where(x => x.Id == id).FirstOrDefault();
                
                return View(clanek);
            }
        }

        //zobrazení komentářů na první stránce k článku
        [HttpGet]
        [ClanekControllerAtributy("idClanku")]
        public ActionResult ZobrazitKomentare(int idClanku)
        {

            //pro pozdější POST ZobrazitKomentare
            Session["idClanku"] = idClanku;

            using(ModelContainer db = new ModelContainer())
            {

                //naplním seznam komentářů k danému článku
                var seznamKomentaru = db.Komentare.Include(x => x.UzivatelOd).Where(x => x.ClanekId == idClanku).OrderByDescending(x => x.DatumPridani).Take(POCET_KOMENTARU).ToList();

                //zjistím počte komentářů které vypíšu ve view
                Session["pocetKomentaru"] = db.Komentare.Include(x => x.UzivatelOd).Where(x => x.ClanekId == idClanku).Count();

                //zjistím kolik bude stránek s komentáři
                int pocetKomentaru = db.Komentare.Include(x => x.UzivatelOd).Where(x => x.ClanekId == idClanku).Count();
                double maxStranek = pocetKomentaru / POCET_KOMENTARU;
                Session["pocetStranekKomentaru"] = Math.Ceiling(maxStranek);

                //nastavím jako aktuální stránku první
                Session["aktualniStrankaKomentaru"] = 1;

                return View(seznamKomentaru);
            }           
        }

        //získání komentářů na zadané stránce
        [HttpGet]
        [ClanekControllerAtributy("cisloStranky")]
        public ActionResult ZobrazitKomentare(short cisloStranky = 1) //short kvůli odlišené datového typu od verze s idClanku, nelze mít dva stejné předpisy
        {

            using (ModelContainer db = new ModelContainer())
            {
                //získám id článku
                int idClanku = int.Parse(Session["idClanku"].ToString());

                //vezmu správné komentáře pro danou stránku
                var seznamKomentaru = db.Komentare.Include(x => x.UzivatelOd).Where(x => x.ClanekId == idClanku).OrderByDescending(x => x.DatumPridani).Skip(POCET_KOMENTARU * (cisloStranky-1)).Take(POCET_KOMENTARU).ToList();

                //zjistím počet komentářů které vypíšu ve view
                Session["pocetKomentaru"] = db.Komentare.Include(x => x.UzivatelOd).Where(x => x.ClanekId == idClanku).Count();

                //zjistím kolik bude stránek s komentáři
                int pocetKomentaru = db.Komentare.Include(x => x.UzivatelOd).Where(x => x.ClanekId == idClanku).Count();
                double maxStranek = pocetKomentaru / POCET_KOMENTARU;
                Session["pocetStranekKomentaru"] = Math.Ceiling(maxStranek);

                //nastavím jako aktuální stránku první
                Session["aktualniStrankaKomentaru"] = cisloStranky;

                return View(seznamKomentaru);
            }
        }

        //pro vložení komentáře
        [HttpPost]
        public ActionResult ZobrazitKomentare(String komentar)
        {

            using (ModelContainer db = new ModelContainer())
            {


                int idClanku = int.Parse(Session["idClanku"].ToString());
                Komentar vkladanyKomentar = new Komentar();
                vkladanyKomentar.ClanekId = idClanku;
                vkladanyKomentar.TypKomentare = TypKomentare.KomentarPrispevek;
                vkladanyKomentar.Text = komentar;
                vkladanyKomentar.UzivatelKomuId = 1; //nastavím na admina protože nejde o komentář na profil ale příspěvku
                vkladanyKomentar.UzivatelOdId = int.Parse(Session["uzivatelID"].ToString());
                vkladanyKomentar.DatumPridani = DateTime.Now;

                //vložím nový komentář
                db.Komentare.Add(vkladanyKomentar);

                //uložím změny
                db.SaveChanges();

                //naplním seznam komentářů k danému článku
                var seznamKomentaru = db.Komentare.Include(x => x.UzivatelOd).Where(x => x.ClanekId == idClanku).OrderByDescending(x => x.DatumPridani).ToList();

                //zjistím počte komentářů které vypíšu ve view
                Session["pocetKomentaru"] = db.Komentare.Include(x => x.UzivatelOd).Where(x => x.ClanekId == idClanku).Count();

                return View(seznamKomentaru);
            }
        
        }


        //vytvoření článku
        public ActionResult Create()
        {

            Clanek clanek = new Clanek();

            //nastavím vychozí hodnotu EnumDropDownListFor ve view
            clanek.Kategorie = KategorieClanku.Koncerty;

            return View(clanek);
        }

        //vytvoření článku
        [HttpPost]
        public ActionResult Create(Clanek clanek)
        {

                using (ModelContainer db = new ModelContainer())
                {

                    //doplní hodnoty které uživatel nemá možnost vyplnit
                    clanek.UzivatelId = int.Parse(Session["uzivatelID"].ToString());
                    clanek.DatumVytvoreni = DateTime.Now;
                    clanek.Titulek = System.Net.WebUtility.HtmlEncode(clanek.Titulek);
                    clanek.Popisek = System.Net.WebUtility.HtmlEncode(clanek.Popisek);
                    clanek.Obsah = System.Net.WebUtility.HtmlEncode(clanek.Obsah);

                //vložím článek
                db.Clanky.Add(clanek);

                    //uložím změny
                    db.SaveChanges();

                return RedirectToAction("Index");
                }      
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {

            using (ModelContainer db = new ModelContainer())
            {

                //vyhledám příspěvek dle ID
                Clanek editovanyPrispevek = db.Clanky.Where(x => x.Id == id).First();

                editovanyPrispevek.Obsah = System.Net.WebUtility.HtmlDecode(editovanyPrispevek.Obsah);
                editovanyPrispevek.Titulek = System.Net.WebUtility.HtmlDecode(editovanyPrispevek.Titulek);
                editovanyPrispevek.Popisek = System.Net.WebUtility.HtmlDecode(editovanyPrispevek.Popisek);

                //vrátím do view
                return View(editovanyPrispevek);
            }

        }

        [HttpPost]
        public ActionResult Edit(Clanek clanek)
        {

            using(ModelContainer db = new ModelContainer())
            {

                //vyhledám článek dle jeho ID
                Clanek editovanyClanek = db.Clanky.Where(x => x.Id == clanek.Id).First();

                //přepíšu hodnoty článku
                editovanyClanek.Kategorie = clanek.Kategorie;
                editovanyClanek.Obsah = clanek.Obsah;
                editovanyClanek.Popisek = clanek.Popisek;
                editovanyClanek.Titulek = clanek.Titulek;

                editovanyClanek.Titulek = System.Net.WebUtility.HtmlEncode(clanek.Titulek);
                editovanyClanek.Popisek = System.Net.WebUtility.HtmlEncode(clanek.Popisek);
                editovanyClanek.Obsah = System.Net.WebUtility.HtmlEncode(clanek.Obsah);

                //pouze upravuji hodnoty proto musím nastavit EntityState, více zde https://docs.microsoft.com/cs-cz/ef/ef6/saving/change-tracking/entity-state
                db.Entry(editovanyClanek).State = EntityState.Modified;

                //uložím změny
                db.SaveChanges();

                //vrátím view které mi vrátí rozcestník na seznam článků nebo na vlastní příspěvky
                return View("PostEdit");
            }

        }


        [HttpGet]
        public ActionResult PostEdit()
        {
            return View();
        }


        //parametrem je ID článku který má být smazán
        [HttpGet]
        public ActionResult Delete(int id)
        {

            using(ModelContainer db = new ModelContainer())
            {

                //vyhledám odebíraný článek
                Clanek odebiranyClanek = db.Clanky.Where(x => x.Id == id).First();


                //vyhledám veškeré komentáře k článku které musím smazat kvůli referenci (cizí klíč)
                db.Komentare.RemoveRange(db.Komentare.Where(x => x.ClanekId == id));

                //odeberu článek
                db.Clanky.Remove(odebiranyClanek);

                //uložím změny
                db.SaveChanges();

                //přesměruju na správné view
                return RedirectToAction("VlastniPrispevky", "Uzivatel", new { kategorie = "clanky" });

                }           
        }

        //view pro vyplnění žádosti o roli redaktora
        [HttpGet]
        public ActionResult ZadostRedaktor()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ZadostRedaktor(Zadost novaZadost)
        {

            using(ModelContainer db = new ModelContainer())
            {

                int idUzivatele = int.Parse(Session["uzivatelID"].ToString());

                //vyhledám aktuálně přihlášeného uživatele
                Uzivatel prihlasenyUzivatel = db.Uzivatele.Where(x => x.Id == idUzivatele).First();

                String adminPrezdivka;

                //vyhledám administrátora kterému pošlu žádost o roli redaktora
                using (IdentityDbContext aspContext = new IdentityDbContext()) {

                    String adminRoleId = aspContext.Roles.Where(x => x.Name == "admin").Select(x => x.Id).First();
                    adminPrezdivka = aspContext.Users.Include(x => x.Roles).Where(x => x.Roles.Any(y => y.RoleId == adminRoleId)).Select(x => x.UserName).First();
                        
                }
                Uzivatel admin = db.Uzivatele.Where(x => x.Prezdivka == adminPrezdivka).First();
               
                novaZadost.StavZadosti = StavZadosti.Cekajici;
                novaZadost.TypZadosti = TypZadosti.ZadostRedaktor;
                novaZadost.ZadostOd = prihlasenyUzivatel;
                novaZadost.ZadostKomu = admin;

                //přebytečné povinné atributy které nepoužiji ale musí být v tabulce
                novaZadost.Interpret = db.Interpreti.Take(1).First();
                novaZadost.Prispevek = db.Prispevky.Take(1).First();

                db.Entry(novaZadost).State = EntityState.Added;

                //vložím novou žádost
                db.Zadosti.Add(novaZadost);

                //uložit změny
                db.SaveChanges();

                //vrátím view s potvrzením o odeslání žádosti
                return View("PotvrzeniZadosti");
            }
        }


        //view s potvrzením o odeslání žádosti na roli redaktora a možným přesměrováním na FAQ
        [HttpGet]
        public ActionResult PotvrzeniZadosti()
        {
            return View();
        }


        //slouží k vyhledání článku pomocí titulku
        [HttpPost]
        public ActionResult VyhledaniClanku(String titulek)
        {

            using (ModelContainer db = new ModelContainer())
            {

                //vyhledám MAX_VYHLEDANYCH_UZIVAETLU kteří začínají na zadanou přezdívku - zruším case sensitivitu pomocí ToUpper
                var vyhledaneClanky = db.Clanky.Where(x => x.Titulek.ToUpper().Contains(titulek.ToUpper())).Take(MAX_POCET_VYHLEDANYCH_CLANKU).ToList();

                //Dekóduji titulek článku pro zobrazení uživateli
                object clanekVratit = vyhledaneClanky.Select(x => new { x.Id, Titulek = WebUtility.HtmlDecode(x.Titulek) });

                //vrátím model naplněný daty
                return Json(clanekVratit, JsonRequestBehavior.AllowGet);
            }
        }

    }


    //slouží k rozlišení parametrů u ClankyKategorie (int, String) -> nyní možné přidat [ClanekControllerAtributy("id")]
    public class ClanekControllerAtributy : ActionMethodSelectorAttribute
    {
        public ClanekControllerAtributy(string valueName)
        {
            ValueName = valueName;
        }
        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            return (controllerContext.HttpContext.Request[ValueName] != null);
        }

        public string ValueName { get; private set; }
    }


}
