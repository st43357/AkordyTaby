using DiplomovaPrace.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using static DiplomovaPrace.Models.ZpravaViewModel;
using System.Web.Routing;

namespace DiplomovaPrace.Controllers
{

    [Authorize]
    public class ZpravaController : Controller
    {

        // GET: Zprava
        //idUzivatele = 0 - pokud není zadané idUzivatele pak vezmu ze seznamu uživatelů zprávy od užitele který napsal nejnovější zprávu
        //nejlépe přepsat
        public ActionResult Index(int idUzivatele = 0)
        {

            using (ModelContainer db = new ModelContainer())
            {

                //model použiju při výpisu zpráv a kontaktů v něm uložených
                VypisZpravIndex model = new VypisZpravIndex();

                int uzivatelId = int.Parse(Session["uzivatelID"].ToString());


                //získám seznam uživatelů se kterými je přihlášený uživatel v kontaktu - kteří mu poslali zprávu, seřezeno od nejnovější zprávy, řazení složité kvůli správně funkčnosti
                var seznamIdUzivateluVKontaktu = db.Zpravy.Where(x => x.UzivatelKomuId == uzivatelId).OrderByDescending(x => x.CasOdeslani).Select(x => new { x.UzivatelOdId, x.CasOdeslani }).Distinct().ToList();
              
                //po řazení se z Listu stane IEnumerable, neřadím pomocí OrderByDescending v LINQ - nefunkční pro typ DateTime
                var serazenySeznamUzivatelu = seznamIdUzivateluVKontaktu.OrderByDescending(x => x.CasOdeslani);

                //vytvořím si seznam který bude obsahovat pouze ID který naplním, abych mohl aplikovat Distinct - jinak by se nic nezměnilo kvůli časům zpráv které jedinečné nejsou
                var pouzeId = new List<int>();
                for (int i = 0; i < serazenySeznamUzivatelu.Count(); i++)
                {
                    pouzeId.Add(serazenySeznamUzivatelu.ElementAt(i).UzivatelOdId);
                }

                //získám jedinečné hodnoty s ID uživatel se kterými je přihlášený uživatel v kontaktu
                var jedinecniUzivatele = pouzeId.Distinct();

                //seznam instancí uživatelů
                List<Uzivatel> seznamUzivateluVKontaktu = new List<Uzivatel>();

                //získám uživatele s danými ID
                for (int i = 0; i < jedinecniUzivatele.Count(); i++)
                {
                    int aktualniId = jedinecniUzivatele.ElementAt(i);
                    Uzivatel uzivatel = db.Uzivatele.Where(x => x.Id == aktualniId).First();
                    seznamUzivateluVKontaktu.Add(uzivatel);
                }

                //získám ID kontaktu který poslal nejnovější zprávu
                int aktualniKontakt;
                if (idUzivatele == 0)
                {
                    aktualniKontakt = jedinecniUzivatele.ElementAt(0);
                    Session["aktualniKontakt"] = aktualniKontakt;
                } else
                {
                    aktualniKontakt = db.Uzivatele.Where(x => x.Id == idUzivatele).First().Id;
                    Session["aktualniKontakt"] = aktualniKontakt;
                }
                //zjistím adresu obrázku uživatele u kterého zobrazují zprávu a přihlášeného uživatele
                model.avatarOd = db.Uzivatele.Where(x => x.Id == uzivatelId).Select(x => x.Avatar).First();
                model.avatarKomu = db.Uzivatele.Where(x => x.Id == aktualniKontakt).Select(x => x.Avatar).First();

                //získám seznam nejnovějších zpráv odeslaných uživatelem přihlášenému uživateli
                List<Zprava> seznamZpravOd = db.Zpravy.Where(x => x.UzivatelKomuId == uzivatelId).Where(x => x.UzivatelOdId == aktualniKontakt).OrderBy(x => x.CasOdeslani).ToList();
                List<Zprava> seznamZpravKomu = db.Zpravy.Where(x => x.UzivatelOdId == uzivatelId).Where(x => x.UzivatelKomuId == aktualniKontakt).OrderBy(x => x.CasOdeslani).ToList();

                //spojím seznamy - tím získám podobu konverzace v časově seřazeném pořadí
                IEnumerable<Zprava> seznamVsechZprav = seznamZpravOd.Concat(seznamZpravKomu);
                //seřadím zprávy
                seznamVsechZprav = seznamVsechZprav.OrderBy(x => x.CasOdeslani);

                //uložím hodnotu do viewmodelu
                model.seznamUzivatelu = seznamUzivateluVKontaktu.AsEnumerable();
                model.seznamZprav = seznamVsechZprav;

                //zjistím aktuálně přihlášeného uživatele kterému změním čas poslední akvitity které porovnám s časem zprávy
                Uzivatel aktualnePrihlaseni = db.Uzivatele.Where(x => x.Id == uzivatelId).First();
                aktualnePrihlaseni.PosledniAktivita = DateTime.Now;
                db.Entry(aktualnePrihlaseni).State = EntityState.Modified;

                //pokud uživatel zobrazí zprávy tak nejsou žádné nepřečtené
                Session["neprecteneZpravy"] = 0;

                db.SaveChanges();

                //vrátím model
                return View("Index", model);
            }        
        }

        [HttpPost]
        public ActionResult VlozitKomentar(String textZpravy)
        {

            using(ModelContainer db = new ModelContainer())
            {

                Zprava novaZprava = new Zprava();

                //naplním hodnoty
                novaZprava.Text = textZpravy;
                novaZprava.UzivatelKomuId = int.Parse(Session["aktualniKontakt"].ToString());
                novaZprava.UzivatelOdId = int.Parse(Session["uzivatelID"].ToString());
                novaZprava.CasOdeslani = DateTime.Now;

                //vložím novou zprávu a uložím změny
                db.Zpravy.Add(novaZprava);
                db.SaveChanges();

                //předám parametr ID uživatele kterému zprávu posílám aby se model mohl naplnit správnými daty
                return RedirectToAction("Index", new RouteValueDictionary(new { controller = "Zprava", action = "Index", idUzivatele = novaZprava.UzivatelKomuId }));
            }          
        }

        [HttpPost]
        public ActionResult PridatKontakt(String prezdivkaUzivatele) 
        {

            using(ModelContainer db = new ModelContainer())
            {
                //existuje hledány uživatel?
                bool existujeUzivatel = db.Uzivatele.Any(x => x.Prezdivka == prezdivkaUzivatele);

                if(existujeUzivatel)
                {
                    //pokud uživatel existuje
                    Uzivatel hledanyUzivatel = db.Uzivatele.Where(x => x.Prezdivka == prezdivkaUzivatele).First();

                    //zašlu první oznamovací zprávu - tím se mi zobrazí v kontaktech
                    Zprava prvniZprava = new Zprava();
                    prvniZprava.Text = "Pozdravte uživatele "+ hledanyUzivatel.Prezdivka+"!";
                    prvniZprava.UzivatelKomuId = int.Parse(Session["uzivatelID"].ToString());
                    prvniZprava.UzivatelOdId = hledanyUzivatel.Id;
                    prvniZprava.CasOdeslani = DateTime.Now;

                    //vložím první zprávu 
                    db.Zpravy.Add(prvniZprava);

                    //uložím změny
                    db.SaveChanges();

                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                else //uživatel s danou přezdívkou neexistuje
                {

                    //vrátím informaci o neúspěšné operaci přidání kontaktu
                    return Json(new { success = false, responseText = "Uživatel se zadanou přezdívkou neexistuje!" }, JsonRequestBehavior.AllowGet);

                }
                //return View("Index", new RouteValueDictionary(new { controller = "Zprava", action = "Index", idUzivatele = int.Parse(Session["aktualniKontakt"].ToString()) }));


            }

        }

    }
}