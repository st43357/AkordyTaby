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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ModelContainer : DbContext
    {
        public ModelContainer()
            : base("name=ModelContainer")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Prispevek> Prispevky { get; set; }
        public virtual DbSet<HodnoceniPrispevku> HodnoceniPrispevku { get; set; }
        public virtual DbSet<Komentar> Komentare { get; set; }
        public virtual DbSet<Zprava> Zpravy { get; set; }
        public virtual DbSet<Zadost> Zadosti { get; set; }
        public virtual DbSet<Interpret> Interpreti { get; set; }
        public virtual DbSet<SpravceInterpreta> SpravciInterpretu { get; set; }
        public virtual DbSet<Clanek> Clanky { get; set; }
        public virtual DbSet<Uzivatel> Uzivatele { get; set; }
    }
}