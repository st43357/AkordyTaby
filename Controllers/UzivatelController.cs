using DiplomovaPrace.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using static DiplomovaPrace.Models.HomeViewModels;
using static DiplomovaPrace.Models.UzivatelViewModels;

namespace DiplomovaPrace.Controllers
{


    [Authorize]
    public class UzivatelController : Controller
    {

        //velikost souboru v MB
        private const int MAX_VELIKOST_SOUBORU = 3;
        private const int MAX_POCET_PRISPEVKU = 5;
        private const int MAX_POCET_CLANKU = 5;

        //počet uživatelů který zobrazím ve vyhledávání uživatelů
        private const int MAX_VYHLEDANYCH_UZIVATELU = 5;


        // GET: Uzivatel - zobrazí profil uživatele - přihlášeného nebo vyhledaného
        public ActionResult Index(String prezdivkaUzivatele)
        {
            using(ModelContainer db = new ModelContainer())
            {

                ProfilViewModel model = new ProfilViewModel();

                //zjistím ID a vyhledám uživatele který je aktuálně přihlášen
                int idPrihlaseneho = int.Parse(Session["uzivatelID"].ToString());
                Uzivatel aktualnePrihlaseny = db.Uzivatele.Where(x => x.Id == idPrihlaseneho).First();


                //pokud potřebuji zobrazit svůj profil
                if(aktualnePrihlaseny.Prezdivka == prezdivkaUzivatele)
                {
                    //získám hodnotu Enum - jiné způsoby nefungovali
                    Session["hodnostUzivatele"] = ZiskejDisplayNameTypHodnostUzivatele(aktualnePrihlaseny.Hodnost);

                    //získám MAX_POCET_PRISPEVKU nejnovějších příspěvků uživatele
                    var nejnovejsiPrispevky = db.Prispevky.Include(x => x.Interpret).Where(x => x.UzivatelId == idPrihlaseneho).OrderByDescending(x => x.DatumPridani).Take(MAX_POCET_PRISPEVKU).ToList();

                    //získám MAX_POCET_CLANKU nejnovějších článku uživatele
                    var nejnovejsiClanky = db.Clanky.Where(x => x.UzivatelId == idPrihlaseneho).OrderByDescending(x => x.DatumVytvoreni).Take(MAX_POCET_CLANKU).ToList();

                    //naplnění hodnot do modelu
                    model.prihlasenyUzivatel = aktualnePrihlaseny;
                    model.nejnovejsiPrispevky = nejnovejsiPrispevky.AsEnumerable();
                    model.nejnovejsiClanky = nejnovejsiClanky.AsEnumerable();
                } else //pokud potřebují zobrazit profil jiného uživatele
                {

                    Uzivatel vyhledavanyUzivatel = db.Uzivatele.Where(x => x.Prezdivka == prezdivkaUzivatele).First();
                    //získám hodnotu Enum - jiné způsoby nefungovali
                    Session["hodnostUzivatele"] = ZiskejDisplayNameTypHodnostUzivatele(vyhledavanyUzivatel.Hodnost);

                    //získám MAX_POCET_PRISPEVKU nejnovějších příspěvků uživatele
                    var nejnovejsiPrispevky = db.Prispevky.Include(x => x.Interpret).Where(x => x.UzivatelId == vyhledavanyUzivatel.Id).OrderByDescending(x => x.DatumPridani).Take(MAX_POCET_PRISPEVKU).ToList();

                    //získám MAX_POCET_CLANKU nejnovějších článku uživatele
                    var nejnovejsiClanky = db.Clanky.Where(x => x.UzivatelId == vyhledavanyUzivatel.Id).OrderByDescending(x => x.DatumVytvoreni).Take(MAX_POCET_CLANKU).ToList();

                    //naplnění hodnot do modelu
                    model.prihlasenyUzivatel = vyhledavanyUzivatel;
                    model.nejnovejsiPrispevky = nejnovejsiPrispevky.AsEnumerable();
                    model.nejnovejsiClanky = nejnovejsiClanky.AsEnumerable();
                }

                //zjistím aktuální počet nepřečtených zpráv
                var pocetNeprectenychZprav = db.Zpravy.Where(x => x.UzivatelKomuId == aktualnePrihlaseny.Id).Where(x => x.CasOdeslani > aktualnePrihlaseny.PosledniAktivita).ToList();
                Session["neprecteneZpravy"] = pocetNeprectenychZprav.Count();

                //vrátím model naplněný daty
                return View(model);
            }
       
        }

        //nahrání nové profilové fotky
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {

            using(ModelContainer db = new ModelContainer())
            {
                //ověření typu souboru
                if (!JeValidniTyp(file.ContentType)) 
                {
                    TempData["ChybaNahraniSouboru"] = "Je možné nahrát pouze JPG, JPEG, PNG.";
                    return RedirectToAction("Index");
                }
                //ověření zda je něco nahráváno
                else if (file.ContentLength == 0)
                {
                    TempData["ChybaNahraniSouboru"] = "Nelze nahrát NIC.";
                    return RedirectToAction("Index");
                //ověření zda není překročena maximální velikost
                } else if(!JeValidniVelikost(file.ContentLength))
                {
                    TempData["ChybaNahraniSouboru"] = "Velikost souboru překročila povolenou velikost "+ MAX_VELIKOST_SOUBORU+"MB.";
                    return RedirectToAction("Index");
                }
                //vše prošlo - nahraji soubor
                else 
                {
            
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("/Images/upload"), fileName);


                    int idPrihlasenyUzivatel = int.Parse(Session["uzivatelID"].ToString());
                    Uzivatel prihlasenyUzivatel = db.Uzivatele.Where(x => x.Id == idPrihlasenyUzivatel).First();
          

                    //uložím nový soubor
                    file.SaveAs(path);

                    //přejmenuji nové nahraný soubor do potřebné podoby - přejmenuji dle názvu uživatele
                    String novyNazev = prihlasenyUzivatel.Prezdivka + Path.GetExtension(path);
                    
                    //smažu uživateli starou profilovou fotku
                    if (System.IO.File.Exists(@"C:\Users\krajt\Desktop\DiplomPrace\DiplomovaPrace" + prihlasenyUzivatel.Avatar))
                    {
                        System.IO.File.Delete(@"C:\Users\krajt\Desktop\DiplomPrace\DiplomovaPrace" + prihlasenyUzivatel.Avatar);
                    }

                    //přejmenuji novou profilovou fotku - zajištění jedinečnosti názvu fotky
                    System.IO.File.Move(Path.Combine(Server.MapPath("/Images/upload"), fileName), Path.Combine(Server.MapPath("/Images/upload"), novyNazev));
          
                    //upravím adresu k profilové fotce uživatele
                    prihlasenyUzivatel.Avatar = "/Images/upload/" + prihlasenyUzivatel.Prezdivka + Path.GetExtension(path);

                    //uložím změny
                    db.SaveChanges();

                    //přesměruji na Index view [httpget] tohoto controlleru
                    return RedirectToAction("Index");
                }                           
            }  
        }

        //načte data potřebná pro zobrazení vlastních příspěvků - kategorie příspěvky, články
        public ActionResult VlastniPrispevky(String kategorie = "prispevky")
        {

            //model který bude vrácen do view
            VlastniPrispevkyViewModel model = new VlastniPrispevkyViewModel();

            //získám ID přihlášeného uživatele
            int idPrihlasenyUzivatel = int.Parse(Session["uzivatelID"].ToString());

            using (ModelContainer db = new ModelContainer())
            {

                //naplním model daty dle kategorie
                if(kategorie == "prispevky")
                {
                    Session["kategorieVlastniPrispevky"] = "prispevky";
                    model.prispevky = db.Prispevky.Include(x => x.Interpret).Where(x => x.UzivatelId == idPrihlasenyUzivatel).OrderByDescending(x => x.DatumPridani).ToList();
                } else // kategorie == "clanky"
                {
                    Session["kategorieVlastniPrispevky"] = "clanky";
                    model.clanky = db.Clanky.Where(x => x.UzivatelId == idPrihlasenyUzivatel).OrderByDescending(x => x.DatumVytvoreni).ToList();
                }

                //vrátím model naplněný daty
                return View(model);
            }
           
        }

        //zobrazí žádosti dle typu - žádosti čekající na potvrzení přihlášeným uživatelem nebo žádosti uživatele čekající na potvrzení ostatními
        public ActionResult VaseZadosti(String typyZadosti = "cekajiciNaPrihlaseneho")
        {

            using(ModelContainer db = new ModelContainer())
            {
                //získám ID přihlášeného uživatele
                int idPrihlasenyUzivatel = int.Parse(Session["uzivatelID"].ToString());

                //model který bude vrácen do view
                VazeZadostiViewModel model = new VazeZadostiViewModel();

                //žádosti které mohu potvrdit
                if (typyZadosti == "cekajiciNaPrihlaseneho") {

                //označím typ zadosti podle kterého poznám co mám ve view vypsat
                @TempData["typyZadosti"] = "cekajiciNaPrihlaseneho";

                    //žádosti čekající na potvrzení přihlášeným uživatelem
                    //vyfiltruji žádosti zaslané přihlášenému uživateli - stav čekající
                    model.cekajiciNaPotvrzeniPrihlaseneho = db.Zadosti.Include(x => x.ZadostOd).Include(x => x.Interpret).Include(x => x.Prispevek).Where(x => x.UzivatelKomuId == idPrihlasenyUzivatel).Where(x => x.StavZadosti == StavZadosti.Cekajici).ToList().OrderByDescending(x => x.Prispevek.DatumPridani);

                    //dotaz sloužící ke zjištění již vyřešených žádostí přihlášeného uživatele - stav schválené nebo zamítnuté žádosti
                    model.cekajiciNaPotvrzeniPrihlasenehoVyresene = db.Zadosti.Include(x => x.ZadostOd).Include(x => x.Interpret).Include(x => x.Prispevek).Where(x => x.UzivatelKomuId == idPrihlasenyUzivatel).Where(x => x.StavZadosti == StavZadosti.Schvalena || x.StavZadosti == StavZadosti.Zamitnuta).ToList().OrderByDescending(x => x.Prispevek.DatumPridani);
                }
                else //typyZadosti == "cekajiciNaPotvrzeniOstatnimi"
                {

                    //označím typ zadosti podle kterého poznám co mám ve view vypsat
                    @TempData["typyZadosti"] = "cekajiciNaPotvrzeniOstatnimi";

                    //žádosti uživatele čekající na schválení ostatními uživateli - stav čekající
                    model.cekajiciNaPotvrzeniOstatnich = db.Zadosti.Include(x => x.ZadostKomu).Include(x => x.Interpret).Include(x => x.Prispevek).Where(x => x.UzivatelOdId == idPrihlasenyUzivatel).Where(x => x.StavZadosti == StavZadosti.Cekajici).ToList().OrderByDescending(x => x.Prispevek.DatumPridani);

                    //dotaz sloužící ke zjištění již vyřešených žádostí odeslaných přihlášeným uživatelem - stav schválené nebo zamítnuté žádosti
                    model.cekajiciNaPotvrzeniOstatnichVyresene = db.Zadosti.Include(x => x.ZadostKomu).Include(x => x.Interpret).Include(x => x.Prispevek).Where(x => x.UzivatelOdId == idPrihlasenyUzivatel).Where(x => x.StavZadosti == StavZadosti.Schvalena || x.StavZadosti == StavZadosti.Zamitnuta).ToList().OrderByDescending(x => x.Prispevek.DatumPridani);

                }

                //vrátím model naplněný daty
                return View(model);
            }
        }



        private bool JeValidniVelikost(int velikostSouboru)
        {
            return ((velikostSouboru / 1024) / 1024 < MAX_VELIKOST_SOUBORU); //soubor musí mít velikost pod MAX_VELIKOST_SOUBORU MB
        }

        //oveření typu nahraného souboru
        private bool JeValidniTyp(String contentType)
        {
            return contentType.Equals("image/png") || contentType.Equals("image/gif") || contentType.Equals("image/jpg") || contentType.Equals("image/jpeg");
        }


        //získání DisplayName u Enum TypHodnostUzivatele
        private String ZiskejDisplayNameTypHodnostUzivatele(TypHodnostUzivatele hodnost)
        {
            switch(hodnost)
            {
                case TypHodnostUzivatele.ZacinajiciHudebnik: return "Začínající hudebník";
                case TypHodnostUzivatele.PokrocilyMuzikant: return "Pokročilý muzikant";
                case TypHodnostUzivatele.ZkusenySkladatel: return "Zkušený skladatel";
                case TypHodnostUzivatele.ZnamaCelebrita: return "Známá celebrita";
                case TypHodnostUzivatele.HudebniBuh: return "Hudební bůh";
            }

            //nenastane - uživatel má vždy určitou hodnost
            return null;
        }


        //v _Layout slouží k vyhledání uživatele pomocí přezdívky
        [HttpPost]
        public ActionResult VyhledaniUzivatele(String prezdivka)
        {
            
            using (ModelContainer db = new ModelContainer())
            {

                //vyhledám MAX_VYHLEDANYCH_UZIVAETLU kteří začínají na zadanou přezdívku - zruším case sensitivitu pomocí ToUpper
                var vyhledaniUzivatele = db.Uzivatele.Where(x => x.Prezdivka.ToUpper().Contains(prezdivka.ToUpper())).Take(MAX_VYHLEDANYCH_UZIVATELU).ToList();

                object vratitUzivatele = vyhledaniUzivatele.Select(x => new { x.Id, Prezdivka = x.Prezdivka });

                //vrátím model naplněný daty
                return Json(vratitUzivatele, JsonRequestBehavior.AllowGet);
            }   
        }

        //vyhledá veškeré interprety které přihlášený uživatel spravuje
        [HttpGet]
        public ActionResult VasiInterpreti()
        {

           using(ModelContainer db = new ModelContainer())
            {

                int idPrihlasenyUzivatel = int.Parse(Session["uzivatelID"].ToString());

                var seznam = db.SpravciInterpretu.Include(x => x.Interpret).Include(x => x.Interpret).Where(x => x.UzivatelId == idPrihlasenyUzivatel).ToList();

                return View(seznam);
            }
        }

        //zobrazí view pro přidání správce k interpretovi
        [HttpGet]
        public ActionResult PridatSpravce(int id)
        {

            PridatSpravceViewModel model = new PridatSpravceViewModel();

            using (ModelContainer db = new ModelContainer())
            {
                //zjistím jakého interpreta upravuji
               model.konkretniInterpret = db.Interpreti.Where(x => x.Id == id).First();

                //zjistím kolik má daný interpret příspěvků
                model.pocetPrispevkuInterpretaCelkem = db.Prispevky.Where(x => x.InterpretId == id).Count();

                //uložím do Session aktuálního ID interpreta ke kterému potřebují přiřadit nového správce
                Session["idInterpretaPridatSpravce"] = id;

                //zjistím kolik má daný interpret schválených příspěvků
                model.pocetPrispevkuInterpretaSchvalenych = db.Prispevky.Where(x => x.InterpretId == id).Where(x => x.Schvalen == true).Count();

                //zjistím veškeré správce daného interpreta
                model.spravciInterpreta = db.SpravciInterpretu.Include(x => x.Uzivatel).Include(x => x.Interpret).Where(x => x.InterpretId == id).ToList();

               return View(model);
            }
        }

        //přijímá jako parametr přezdívku uživatele který se stane správcem daného interpreta
        [HttpGet]
        public ActionResult PridatSpravcePrezdivkou(String prezdivka)
        {

            using(ModelContainer db = new ModelContainer())
            {

                int idInterpreta = int.Parse(Session["idInterpretaPridatSpravce"].ToString());

                //oveření zda uživatel již není správcem interpreta
                if (db.SpravciInterpretu.Include(x => x.Uzivatel).Any(x => x.InterpretId == idInterpreta && x.Uzivatel.Prezdivka == prezdivka))
                {
                    Session["jizJeSpravce"] = "Vámi vybraný uživatel již má roli správce interpreta.";
                    return RedirectToAction("PridatSpravce", new { id = idInterpreta });
                }

                //naplním nového správce potřebnými daty
                SpravceInterpreta novySpravce = new SpravceInterpreta();
                novySpravce.Uzivatel = db.Uzivatele.Where(x => x.Prezdivka == prezdivka).First();
                novySpravce.Interpret = db.Interpreti.Where(x => x.Id == idInterpreta).First();

                //přidám nového správce
                db.SpravciInterpretu.Add(novySpravce);

                //uložím změny
                db.SaveChanges();

                return RedirectToAction("PridatSpravce", new { id = idInterpreta });

            }

        }

        //odeberu uživatele se zadaným Id z role správce konkrétního interpreta
        public ActionResult OdebratSpravce(int id)
        {

            using(ModelContainer db = new ModelContainer())
            {

                //zjistím id interpreta
                int idInterpreta = int.Parse(Session["idInterpretaPridatSpravce"].ToString());

                //odeberu správce interpreta
                db.SpravciInterpretu.Remove(db.SpravciInterpretu.Where(x => x.UzivatelId == id).Where(x => x.InterpretId == idInterpreta).First());

                //uložím změny
                db.SaveChanges();

                //přesměruji na potřebnou stránku
                return RedirectToAction("PridatSpravce", new { id = idInterpreta });
            }
        }

    }


 


}