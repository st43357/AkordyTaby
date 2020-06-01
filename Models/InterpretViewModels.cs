using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DiplomovaPrace.Models
{
    public class InterpretViewModels
    {

        public class InterpretiSkladbyViewModel
        {
            [Display(Name = "Interpreti")]
            public IEnumerable<Interpret> seznamInterpretu;

            [Display(Name = "Příspěvky")]
            public IEnumerable<Prispevek> seznamPrispevku;

            [Display(Name = "Správce interpreta")]
            public IEnumerable<Uzivatel> seznamSpravcu;
        }

        //zobrazí veškeré příspěvky interpreta s vybraným názvem - view SeznamPrispevkuNazev
        public class PrispevkyInterpretaNazev
        {
            [Display(Name = "Příspěvky")]
            public IEnumerable<Prispevek> seznamPrispevku;

            [Display(Name = "Průměrné hodnocení")]
            public IEnumerable<int?> prumerneHodnoceniPrispevku;
        }

    }

}