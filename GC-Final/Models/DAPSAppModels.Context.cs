﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GC_Final.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BuildDisk> BuildDisks { get; set; }
        public virtual DbSet<BuildMonitor> BuildMonitors { get; set; }
        public virtual DbSet<BuildOD> BuildODs { get; set; }
        public virtual DbSet<BuildPCI> BuildPCIs { get; set; }
        public virtual DbSet<Build> Builds { get; set; }
        public virtual DbSet<BuildsPeripheral> BuildsPeripherals { get; set; }
        public virtual DbSet<BuildsRAM> BuildsRAMs { get; set; }
        public virtual DbSet<Case> Cases { get; set; }
        public virtual DbSet<CPU> CPUs { get; set; }
        public virtual DbSet<GPU> GPUs { get; set; }
        public virtual DbSet<HardDrive> HardDrives { get; set; }
        public virtual DbSet<Monitor> Monitors { get; set; }
        public virtual DbSet<Motherboard> Motherboards { get; set; }
        public virtual DbSet<OpticalDriver> OpticalDrivers { get; set; }
        public virtual DbSet<PCICard> PCICards { get; set; }
        public virtual DbSet<Peripheral> Peripherals { get; set; }
        public virtual DbSet<PSU> PSUs { get; set; }
        public virtual DbSet<RAM> RAMs { get; set; }
    }
}
