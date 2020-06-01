using DiplomovaPrace.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using static DiplomovaPrace.Models.PrispevekViewModels;

namespace DiplomovaPrace.Controllers
{

    [Authorize]
    public class PrispevekController : Controller
    {

        private const int POCET_PRISPEVKU = 100;

        // GET: Prispevek
        public ActionResult Index()
        {

            using(ModelContainer db = new ModelContainer())
            {

                //získám z databáze POCET_PRISPEVKU nejnovějších příspěvků které vypíši
                List<Prispevek> seznamPrispevku = new List<Prispevek>();
                seznamPrispevku = db.Prispevky.Include(x => x.Autor).Include(x => x.Interpret).Where(x => x.Schvalen == true).OrderByDescending(x => x.DatumPridani).Take(POCET_PRISPEVKU).ToList();


                //zjistím aktuální počet nepřečtených zpráv
                int idPrihlaseneho = int.Parse(Session["uzivatelID"].ToString());
                Uzivatel prihlasenyUzivatel = db.Uzivatele.Where(x => x.Id == idPrihlaseneho).First();
                var pocetNeprectenychZprav = db.Zpravy.Where(x => x.UzivatelKomuId == prihlasenyUzivatel.Id).Where(x => x.CasOdeslani > prihlasenyUzivatel.PosledniAktivita).ToList();
                Session["neprecteneZpravy"] = pocetNeprectenychZprav.Count();


                return View(seznamPrispevku);
            }        
        }

        // GET: Prispevek/KonkretniPrispevek
        public ActionResult KonkretniPrispevek(int id)
        {

            using(ModelContainer db = new ModelContainer())
            {
                var konkretniPrispevek = db.Prispevky.Include(x => x.Autor).Include(x => x.Interpret).First(x => x.Id == id);

                Session["konkretniPrispevekId"] = id;

                return View(konkretniPrispevek);
            }
          
        }

        //vrátí view pro editaci příspěvku
        [HttpGet]
        public ActionResult Edit(int id)
        {

            using (ModelContainer db = new ModelContainer())
            {
                Prispevek editovanyPrispevek = db.Prispevky.Where(x => x.Id == id).First();

                editovanyPrispevek.Obsah = System.Net.WebUtility.HtmlDecode(editovanyPrispevek.Obsah);

                return View(editovanyPrispevek);
            }     
        }

        //upravíme hodnoty objektu
        [HttpPost]
        public ActionResult Edit(Prispevek prispevek)
        {

            using(ModelContainer db = new ModelContainer())
            {

                Prispevek stareHodnoty = db.Prispevky.Where(x => x.Id == prispevek.Id).First();

                //přepíšu hodnoty
                stareHodnoty.NazevPisne = prispevek.NazevPisne;
                stareHodnoty.Obsah = prispevek.Obsah;
                stareHodnoty.TypPrispevku = prispevek.TypPrispevku;

                //nastavím stav - změna hodnot
                db.Entry(stareHodnoty).State = EntityState.Modified;

                //uložím změny
                db.SaveChanges();

                return View("PostEdit");
            }          
        }


        [HttpGet]
        public ActionResult PostEdit()
        {
            return View();
        }


        [HttpGet]
        public ActionResult Delete(int id)
        {

            using(ModelContainer db = new ModelContainer())
            {

                //vyhledám odebíraný článek
                Prispevek odebiranyPrispevek= db.Prispevky.Where(x => x.Id == id).First();

                //odeberu veškeré hodnocení příspěvku - kvůli integritě s cizím klíčem
                db.HodnoceniPrispevku.RemoveRange(db.HodnoceniPrispevku.Where(x => x.PrispevekId == id));

                //odeberu článek
                db.Prispevky.Remove(odebiranyPrispevek);

                //odeberu žádost spojenou s příspěvkem
                db.Zadosti.RemoveRange(db.Zadosti.Where(x => x.PrispevekId == id)); 

                //uložím změny
                db.SaveChanges();

                //přesměruju na správné view
                return RedirectToAction("VlastniPrispevky", "Uzivatel", new { kategorie = "prispevky" });
            }

        }


        [HttpGet]
        public ActionResult NovyPrispevek()
        {

            Prispevek prispevek = new Prispevek();

            prispevek.TypPrispevku = TypPrispevek.Akordy;
            return View(prispevek);
        }

        [HttpPost]
        public ActionResult NovyPrispevek(Prispevek prispevek)
        {

            using(ModelContainer db = new ModelContainer())
            {

                int idUzivatel = int.Parse(Session["uzivatelID"].ToString());

                //vyhledám potřebné informace - aktuálně přihlášený uživatel a interpreta ke kterému potřebuji vložit příspěvek
                Uzivatel aktualnePrihlaseny = db.Uzivatele.Where(x => x.Id == idUzivatel).First();

                String nazevInterpreta = Session["interpretNazev"].ToString();
                Interpret aktualniInterpret = db.Interpreti.Where(x => x.Nazev == nazevInterpreta).First();

                //doplnim vyhledané informace
                prispevek.DatumPridani = DateTime.Now;
                prispevek.Interpret = aktualniInterpret;
                prispevek.Autor = aktualnePrihlaseny;
                prispevek.Schvalen = false;

                //vyhledám uživatele který je správcem interpreta
                int idSpravce = db.SpravciInterpretu.Where(x => x.Interpret.Nazev == nazevInterpreta).Select(x => x.UzivatelId).First();
                Uzivatel spravceInterpreta = db.Uzivatele.Where(x => x.Id == idSpravce).First();

                prispevek.Obsah = System.Net.WebUtility.HtmlEncode(prispevek.Obsah);

                //vytvořím žádost
                Zadost novaZadost = new Zadost();
                novaZadost.Interpret = aktualniInterpret;
                novaZadost.Prispevek = prispevek;
                novaZadost.StavZadosti = StavZadosti.Cekajici;
                novaZadost.ZadostOd = aktualnePrihlaseny;
                novaZadost.ZadostKomu = spravceInterpreta;
                novaZadost.TypZadosti = TypZadosti.ZadostPisen;



                //pokud uživatel který vkládá příspěvek je zároveň správcem daného interpreta pak se nemusí čekat na schválení žádosti a příspěvek bude přidán
                if(spravceInterpreta.Prezdivka.Equals(aktualnePrihlaseny.Prezdivka))
                {
                    novaZadost.StavZadosti = StavZadosti.Schvalena;
                    prispevek.Schvalen = true;
                }


                //vložím nový příspěvek
                db.Prispevky.Add(prispevek);

                //vložím novou žádost
                db.Zadosti.Add(novaZadost);

                //uložím změny
                db.SaveChanges();

                //vrátit view které zobrazí odeslání žádosti o příspěvku a vysvětlení co dál
                return View("NovyPrispevekPotvrzeni");
            }

        }

        //zobrazí potvrzení o odeslání žádosti o vložení příspěvku
        [HttpGet]
        public ActionResult NovyPrispevekPotvrzeni()
        {
            return View();
        }


        //vrátí tvar akordu jako PartialView
        [HttpGet]
        public ActionResult AkordDetail(String akord)
        {

            AkordViewModel model = new AkordViewModel();

            //úpravy parametrů akordu aby souhlasili s webovou stránkou ze které jsou obrázky převzaty
            //nahradím znak # za cis
            akord = akord.Replace("#", "is");

            //model.akordAdresa = "/Images/akordy/" + akord + ".png";
            //model.akordAdresa = "<a href='https://akordiky.cz/'"+akord+"><img height='118' width='138' src='https://akordiky.cz/img/chords/svg/"+ akord + "_1.svg'></a>";
            model.akordAdresa = akord;

            return PartialView("~/Views/Prispevek/_AkordDetail.cshtml", model);
        }



    }
}