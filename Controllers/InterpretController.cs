
using DiplomovaPrace.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static DiplomovaPrace.Models.InterpretViewModels;
using System.Data.Entity;
using Microsoft.Ajax.Utilities;

namespace DiplomovaPrace.Controllers
{

    [Authorize]
    public class InterpretController : Controller
    {
        //počet interpretu který bude vrácen do View pomocí ViewModelu
        private static int VRACENO_INTERPRETU = 15;
        //počet příspěvku které budou vráceny do View pomocí ViewModelu
        private static int VRACENO_PRISPEVKU = 3;


        // GET: Interpret/Index - parametr slouží pro vyhledávání v tabulce interpretů, ve výchozím stavu vyhledává mezi všemi interprety
        public ActionResult Index(String nazevInterpreta = "")
        {
            //získám databázový context
            using(ModelContainer db = new ModelContainer())
            {

                //uložím hodnotu kolik příspěvků vypíšu ke každému interpretovi
                ViewBag.PocetPrispevku = VRACENO_PRISPEVKU;

                InterpretiSkladbyViewModel model = new InterpretiSkladbyViewModel();
                
                //pokud není zadán parametr - nevyhledávám konkrétního interpreta
                if(nazevInterpreta.Equals("")) {
                    model.seznamInterpretu = db.Interpreti.ToList().Take(VRACENO_INTERPRETU);
                }
                else //vyhledávám interpreta který ve svém názvu obsahuje hodnotu nazevInterpreta
                {
                    model.seznamInterpretu = db.Interpreti.Where(x => x.Nazev.ToUpper().Contains(nazevInterpreta.ToUpper())).Take(VRACENO_INTERPRETU).ToList();
                }

                //vytvořeny Listy protože do IEnumerable nelze vkládat
                List<Prispevek> seznamPrispevku = new List<Prispevek>();
                List<Uzivatel> seznamSpravcu = new List<Uzivatel>();

                /*
                 *Zjistím zda je menší počet interpretů v databází nebo konstanta která určuje kolik interpretů má být vráceno 
                 * pokud by hodnota konstanty byla větší než počet interpretů v databází = error
                 */
                int minimum = Math.Min(model.seznamInterpretu.Count(), VRACENO_INTERPRETU);

                //vyhledám ke každému intepretovi 3 příspevky 
                for (int i = 0; i < minimum; i++)
                {
                                   
                    //získám ID aktuálního interpreta
                    Interpret aktualniInterpret = model.seznamInterpretu.ElementAt(i);
                    int idInterpreta = db.Interpreti.Where(x => x.Nazev == aktualniInterpret.Nazev).Select(x => x.Id).First();

                    //získám ID uživatele který je správcem aktuálního interpreta
                    String prezdivka = db.SpravciInterpretu.Where(x => x.InterpretId == idInterpreta).Select(x => x.Uzivatel.Prezdivka).First();

                    //získám VRACENO_PRISPEVKU příspěvku od každého interpreta 
                    var prispevkyInterpreta = db.Prispevky.Where(x => x.InterpretId == idInterpreta).Where(x => x.Schvalen == true).Take(VRACENO_PRISPEVKU).ToList();

                    //zjištuji postupně zda interpret má dostatek příspěvků
                    //
                    for (int j = 0; j < VRACENO_PRISPEVKU; j++)
                    {                       
                        try {
                            seznamPrispevku.Add(prispevkyInterpreta.ElementAt(j));
                        }
                        //pokud interpret nemá dost písní -> vložím prázdný prvek kvůli správnému výpisu ve view
                        catch (Exception)
                        {
                            //pokud nějaký prvek neexistuje vložím prázdný prvek a pokračuji dál
                            seznamPrispevku.Add(new Prispevek());
                            continue;
                        }
                    }

                    //vložím uživatelovu přezdívku do seznamu
                    seznamSpravcu.Add(new Uzivatel { Prezdivka = prezdivka });

                }

                //zjistím aktuální počet nepřečtených zpráv
                int idPrihlaseneho = int.Parse(Session["uzivatelID"].ToString());
                Uzivatel prihlasenyUzivatel = db.Uzivatele.Where(x => x.Id == idPrihlaseneho).First();
                var pocetNeprectenychZprav = db.Zpravy.Where(x => x.UzivatelKomuId == prihlasenyUzivatel.Id).Where(x => x.CasOdeslani > prihlasenyUzivatel.PosledniAktivita).ToList();
                Session["neprecteneZpravy"] = pocetNeprectenychZprav.Count();


                //převedení Listů na IEnumerable
                model.seznamPrispevku = seznamPrispevku.AsEnumerable();
                model.seznamSpravcu = seznamSpravcu.AsEnumerable();

                //return View(model);
                return PartialView("Index", model);

            }
        }

        // GET: Interpret/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Interpret/Create
        [HttpPost]
        public ActionResult Create([Bind(Include = "Nazev")] Interpret vkladanyInterpret)
        {
            //pokud není uživatel přihlášen pak ho přesměruji na přihlášení
            if(Session["uzivatelID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            //získám ID uživatele ze session
            int idUzivatele = (int) Session["uzivatelID"];

            String nazevInterpreta = vkladanyInterpret.Nazev;

            using(ModelContainer db = new ModelContainer())
            {
                //vyhledám jestli interpret s tímto názvem již existuje
                int pocetVyskytu = db.Interpreti.Where(x => x.Nazev == nazevInterpreta).Count();

                //pokud interpret neexistuje -> vložím do databáze
                if(pocetVyskytu == 0)
                {
                    vkladanyInterpret.DatumVytvoreni = DateTime.Now;
                    db.Interpreti.Add(vkladanyInterpret);

                    //uživatele který interpreta vytvořil jmenuji správcem interpreta
                    SpravceInterpreta spravce = new SpravceInterpreta();
                    spravce.Interpret = vkladanyInterpret;
                    spravce.UzivatelId = idUzivatele;
                    db.SpravciInterpretu.Add(spravce);

                    //uložím změny
                    db.SaveChanges();
                } else
                {
                    ViewBag.InterpretVytvoreniChyba = "Interpret se zadaným názvem již existuje!";
                }
            }

            return RedirectToAction("CreatePotvrzeni", "Interpret");
        }


        // GET: Interpret/CreatePotvrzeni
        public ActionResult CreatePotvrzeni()
        {
            return View();
        }


        //vypíše názvy všech příspěvku interpreta - od každého příspěvku pouze jeden název
        public ActionResult KonkretniInterpret(int id)
        {

            using (ModelContainer db = new ModelContainer())
            {

                //uložím název interpreta pro zobrazení ve View
                Session["interpretNazev"] = db.Interpreti.First(x => x.Id == id).Nazev;

                //vrátím pouze jedninečné hodnoty názvem příspěvku
                List<Prispevek> prispevkyInterpreta = db.Prispevky.Include(x => x.Autor).Where(x => x.InterpretId == id).Where(x => x.Schvalen == true).DistinctBy(x => x.NazevPisne).OrderBy(x => x.NazevPisne).ToList();

                return View(prispevkyInterpreta);
            }

        }


        //slouží k výpisu příspěvků které patří pod interpreta s názve příspěvku, pomocí názvu můžeme zobrazit veškeré příspěvky s tímto názvem (př.: všechny písně s názvem Dole v dole)
        public ActionResult SeznamPrispevkuNazev(String nazevPrispevku)
        {

            using(ModelContainer db = new ModelContainer())
            {

                //uložím název písně pro zobrazení ve View
                Session["pisenNazev"] = nazevPrispevku;

                //použít nullable int u těch co nemají hodnocení
                PrispevkyInterpretaNazev model = new PrispevkyInterpretaNazev();
                List<int?> prumerneHodnoceni = new List<int?>();

                //uložím příspěvky dle názvu (například veškeřé příspěvky s názvem Dole v dole), include autora kvůli přezdívce
                List<Prispevek> seznamPrispevku = db.Prispevky.Include(x => x.Autor).Where(x => x.NazevPisne == nazevPrispevku).Where(x => x.Schvalen == true).ToList();

                //pro každý příspěvek zjistím průměrné hodnocení
                for (int i = 0; i < seznamPrispevku.Count; i++)
                {
                    //zjistím ID příspěvku
                    int idPrispevku = seznamPrispevku.ElementAt(i).Id;

                    //pokud má příspěvek nějaké hodnocení
                    if (db.HodnoceniPrispevku.Count(x => x.PrispevekId == idPrispevku) > 0)
                    {
                        //zjistím průměrné hodnocení příspěvku, pouze celočíselné hodnocení, Average nefungoval tak po staru :)
                        int pocetHodnot = db.HodnoceniPrispevku.Where(x => x.PrispevekId == idPrispevku).Count();
                        List<int> seznamHodnot = db.HodnoceniPrispevku.Where(x => x.PrispevekId == idPrispevku).Select(x => x.Hodnoceni).ToList();
                        int sumaHodnot = 0;
                        foreach (var hodnoceni in seznamHodnot)
                        {
                            sumaHodnot += hodnoceni;
                        }

                        //uložím průměrnou hodnotu do seznamu
                        int prumer = (int)sumaHodnot/pocetHodnot;
                        prumerneHodnoceni.Add(prumer);
                    } else
                    {
                        //pokud nemá hodnocení vložím 0
                        prumerneHodnoceni.Add(0);
                    }
                }

                //přidělím hodnoty modelu
                model.seznamPrispevku = seznamPrispevku.AsEnumerable();
                model.prumerneHodnoceniPrispevku = prumerneHodnoceni.AsEnumerable();
                
                //vrátím model do view
                return View(model);
            }        
        }

        //odeslání hodnocení do databáze
        [HttpPost]
        public void SeznamPrispevkuNazev(int hodnoceni)
        {

            int uzivatelId = int.Parse(Session["uzivatelID"].ToString());
            int prispevekId = int.Parse(Session["konkretniPrispevekId"].ToString());

            HodnoceniPrispevku hodnoceniUzivatele = new HodnoceniPrispevku();

            using (ModelContainer db = new ModelContainer())
            {

                //pokud uživatel daný přispěvek doposud nehodnotil vložím nový záznam hodnocení
                if(db.HodnoceniPrispevku.Where(x => x.UzivatelId == uzivatelId).Where(x => x.PrispevekId == prispevekId).Count() == 0)
                {
                    hodnoceniUzivatele.PrispevekId = prispevekId;
                    hodnoceniUzivatele.UzivatelId = uzivatelId;
                    hodnoceniUzivatele.Hodnoceni = hodnoceni;

                    db.HodnoceniPrispevku.Add(hodnoceniUzivatele);
                    db.SaveChanges();
                }
                else // pokud uživatel již přispěvek hodnotil - upravím hodnotu hodnocení
                {
                    HodnoceniPrispevku editovaneHodnoceni = db.HodnoceniPrispevku.Where(x => x.UzivatelId == uzivatelId).Where(x => x.PrispevekId == prispevekId).FirstOrDefault();

                    editovaneHodnoceni.Hodnoceni = hodnoceni;
                    db.SaveChanges();
                }              
            }         
        }



        //pro filtrování hodnot v tabulce interpretů
        [HttpPost]
        public ActionResult VyhledaniIntepreta(String nazevInterpreta)
        {

                //zavolám Index s předaným parametrem
                return RedirectToAction("Index", new { nazevInterpreta = nazevInterpreta } );
            }
        }


    }
