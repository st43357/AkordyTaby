﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DiplomovaPrace.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public partial class Uzivatel
    {

        [DisplayName("ID")]
        public int Id { get; set; }

        [DisplayName("Přezdívka")]
        [Required(ErrorMessage = "Přezdívka nemùže být prázdná.")]
        public string Prezdivka { get; set; }

        [DisplayName("Heslo")]
        [Required(ErrorMessage = "Heslo nemùže být prázdné.")]
        public string Heslo { get; set; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "Email nemùže být prázdný.")]
        public string Email { get; set; }

        [DisplayName("Jméno")]
        [Required(ErrorMessage = "Jméno musí být vyplněné.")]
        public string Jmeno { get; set; }

        [DisplayName("Příjmení")]
        [Required(ErrorMessage = "Příjmení musí být vyplněné.")]
        public string Prijmeni { get; set; }

        [DisplayName("Město")]
        [Required(ErrorMessage = "Město musí být vyplněné.")]
        public string Mesto { get; set; }

        [DisplayName("Poslední aktivita")]
        public System.DateTime PosledniAktivita { get; set; }

        [DisplayName("Avatar")]
        public string Avatar { get; set; }

        [DisplayName("Datum narození")]
        [Required(ErrorMessage = "Datum narození musí být vyplněné.")]
        public System.DateTime DatumNarozeni { get; set; }

        [DisplayName("Datum registrace")]
        public System.DateTime DatumRegistrace { get; set; }

        [DisplayName("Role")]
        public TypRoleUzivatele Role { get; set; }

        [DisplayName("Hodnost")]
        public TypHodnostUzivatele Hodnost { get; set; }
    }
}