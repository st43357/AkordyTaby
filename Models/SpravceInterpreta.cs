//------------------------------------------------------------------------------
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

    public partial class SpravceInterpreta
    {

        [DisplayName("ID")]
        public int Id { get; set; }

        [DisplayName("Interpret")]
        public int InterpretId { get; set; }

        [DisplayName("U�ivatel")]
        public int UzivatelId { get; set; }


        [DisplayName("Interpret")]
        public virtual Interpret Interpret { get; set; }

        [DisplayName("U�ivatel")]
        public virtual Uzivatel Uzivatel { get; set; }
    }
}