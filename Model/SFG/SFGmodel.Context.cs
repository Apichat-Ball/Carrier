﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Carrier.Model.SFG
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SFGEntities : DbContext
    {
        public SFGEntities()
            : base("name=SFGEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<SAP_VBRK_NEWSAP> SAP_VBRK_NEWSAP { get; set; }
        public virtual DbSet<SAP_VBRP_NEWSAP> SAP_VBRP_NEWSAP { get; set; }
        public virtual DbSet<vSAP_Site> vSAP_Site { get; set; }
        public virtual DbSet<vSAP_Site2> vSAP_Site2 { get; set; }
    }
}
