﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ERP_V2.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class PURSysEntities : DbContext
    {
        public PURSysEntities()
            : base("name=PURSysEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BA01A> BA01A { get; set; }
        public virtual DbSet<BA01B> BA01B { get; set; }
        public virtual DbSet<BA02A> BA02A { get; set; }
        public virtual DbSet<BA02B> BA02B { get; set; }
        public virtual DbSet<BA03A> BA03A { get; set; }
        public virtual DbSet<FN01> FN01 { get; set; }
        public virtual DbSet<BA04A> BA04A { get; set; }
        public virtual DbSet<TR01A> TR01A { get; set; }
        public virtual DbSet<TR01B> TR01B { get; set; }
        public virtual DbSet<TR01C> TR01C { get; set; }
    }
}