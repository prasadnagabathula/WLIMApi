using Microsoft.EntityFrameworkCore;
using MSS.WLIM.DataServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSS.WLIM.DataServices.Data
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {
        }
        public DbSet<Roles> WHTblRole { get; set; }
        public DbSet<Departments> WHTblDepartment { get; set; }
        public DbSet<Designations> WHTblDesignation { get; set; }
        public DbSet<Users> WHTblUser { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //----------3rd table_Employee------------------------------------
            modelBuilder.Entity<Users>()
            .HasOne(e => e.Designation)
            .WithMany(d => d.Users)
            .HasForeignKey(e => e.DesignationId);

            modelBuilder.Entity<Users>()
            .HasOne(e => e.Department)
            .WithMany(d => d.Users)
            .HasForeignKey(e => e.DepartmentId);

            modelBuilder.Entity<Users>()
            .HasOne(e => e.Roles)
            .WithMany(d => d.Users)
            .HasForeignKey(e => e.Role);

            modelBuilder.Entity<Users>()
            .HasOne(e => e.ReportingToUser)
            .WithMany(d => d.Subordinates)
            .HasForeignKey(e => e.ReportingTo);

        }
    }
}