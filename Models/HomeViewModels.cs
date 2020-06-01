using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DiplomovaPrace.Models
{
    public class HomeViewModels
    {

        public class HomeIndexViewModel
        {
            [Display(Name = "Interpreti")]
            public List<Interpret> NejnovejsiInterpreti;

            [Display(Name = "Příspěvky")]
            public List<Prispevek> NejnovejsiPrispevky;

            [Display(Name = "Články")]
            public List<Clanek> NejnovejsiClanky;

            //počet žádostí které čekají na vyhodnocení přihlášením uživatelem
            [Display(Name = "Nevyřízené žádosti")]
            public int pocetNevyrizenychZadosti;
        }

        public class PodporaKontaktViewModel
        {
            //atributy vytvořeny kvůli vlastnostem
            private String predmet;
            private String text;

            //vytvořeny i get a set metody kvůli bindingu v HomeController -> PodporaKontakt view,
            [Required]
            [Display(Name = "Předmět")]
            public String Predmet { get { return predmet; }  set { predmet = value; } }

            [Required]
            [Display(Name = "Text")]
            public String Text { get { return text; } set { text = value; } }
        }


    }
}