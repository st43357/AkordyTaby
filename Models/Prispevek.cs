namespace DiplomovaPrace.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public partial class Prispevek
    {

        [DisplayName("ID")]
        public int Id { get; set; }

        [DisplayName("N�zev p�sn�")]
        [Required(ErrorMessage = "N�zev p�sn� mus� b�t vypln�n�.")]
        public string NazevPisne { get; set; }

        [DisplayName("Obsah")]
        [Required(ErrorMessage = "Obsah nesm� b�t pr�zdn�.")]
        [AllowHtml]
        public string Obsah { get; set; }

        [DisplayName("Datum p�id�n�")]
        public System.DateTime DatumPridani { get; set; }

        [DisplayName("Typ p��sp�vku")]
        public TypPrispevek TypPrispevku { get; set; }

        [DisplayName("Autor")]
        public int UzivatelId { get; set; }

        [DisplayName("Interpret")]
        public int InterpretId { get; set; }

        [DisplayName("Schv�leno")]
        public bool Schvalen { get; set; }

        [DisplayName("Autor")]
        public virtual Uzivatel Autor { get; set; }

        [DisplayName("Interpret")]
        public virtual Interpret Interpret { get; set; }
    }
}