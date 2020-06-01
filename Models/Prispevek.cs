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

        [DisplayName("Název písnì")]
        [Required(ErrorMessage = "Název písnì musí být vyplnìný.")]
        public string NazevPisne { get; set; }

        [DisplayName("Obsah")]
        [Required(ErrorMessage = "Obsah nesmí být prázdný.")]
        [AllowHtml]
        public string Obsah { get; set; }

        [DisplayName("Datum pøidání")]
        public System.DateTime DatumPridani { get; set; }

        [DisplayName("Typ pøíspìvku")]
        public TypPrispevek TypPrispevku { get; set; }

        [DisplayName("Autor")]
        public int UzivatelId { get; set; }

        [DisplayName("Interpret")]
        public int InterpretId { get; set; }

        [DisplayName("Schváleno")]
        public bool Schvalen { get; set; }

        [DisplayName("Autor")]
        public virtual Uzivatel Autor { get; set; }

        [DisplayName("Interpret")]
        public virtual Interpret Interpret { get; set; }
    }
}