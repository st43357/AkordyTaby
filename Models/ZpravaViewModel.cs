using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DiplomovaPrace.Models
{
    public class ZpravaViewModel
    {

        public class VypisZpravIndex
        {

            //obsahuje veškeré uživatele se kterými si uživatel psal
            public IEnumerable<Uzivatel> seznamUzivatelu;

            //obsahuje zprávy konkrétního kontaktu se kterým si uživatel psal
            public IEnumerable<Zprava> seznamZprav;

            //adresa na profilové foto uživatele od kterého přišla zpráva
            public String avatarOd;

            //adresa na profilové foto uživatelé kterému přišla zpáva
            public String avatarKomu;

            //ID uživatele který odeslal zpravu
            public int idOdesilatele;
        }
    }
}