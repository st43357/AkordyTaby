using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DiplomovaPrace.Models
{
    public class UzivatelViewModels
    {

        //data potřebná pro zobrazení profilu - Index view - UzivatelController
        public class ProfilViewModel
        {
            public Uzivatel prihlasenyUzivatel;

            public IEnumerable<Prispevek> nejnovejsiPrispevky;

            public IEnumerable<Clanek> nejnovejsiClanky;
        }

        //data potřebná pro zobrazení vlastních příspěvků - VlastniPrispevky view - UzivatelController
        public class VlastniPrispevkyViewModel
        {
            public IEnumerable<Prispevek> prispevky;

            public IEnumerable<Clanek> clanky;
        }


        //data potřebná pro zobrazení všech žádostí které se týkají přihlášeného uživatele (VaseZadosti view)
        public class VazeZadostiViewModel
        {

            //seznam žádostí které může potvrdit přihlášený uživatel - nové příspěvky pokud je správcem interpreta
            public IEnumerable<Zadost> cekajiciNaPotvrzeniPrihlaseneho;

            //seznam žádostí které může potvrdit přihlášený uživatel - nové příspěvky pokud je správcem interpreta - které již byly vyhodnoceny (historie)
            public IEnumerable<Zadost> cekajiciNaPotvrzeniPrihlasenehoVyresene;

            //seznam žádostí které čekají na potvrzení jinými uživateli - čekání na schválění mého příspěvku, schválení žádosti o redaktora
            public IEnumerable<Zadost> cekajiciNaPotvrzeniOstatnich;

            //seznam žádostí které čekají na potvrzení jinými uživateli - čekání na schválění mého příspěvku, schválení žádosti o redaktora - které již byly vyhodnoceny (historie)
            public IEnumerable<Zadost> cekajiciNaPotvrzeniOstatnichVyresene;
        }


        //data pro výpis správců konkrétního interpreta (PridatSpravce view)
        public class PridatSpravceViewModel
        {
            //seznam správců konkrétního interpreta
            public IEnumerable<SpravceInterpreta> spravciInterpreta;

            public int pocetPrispevkuInterpretaCelkem;

            public int pocetPrispevkuInterpretaSchvalenych;

            public Interpret konkretniInterpret;
        }


    }
}