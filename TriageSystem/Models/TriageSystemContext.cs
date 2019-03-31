using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using TriageSystem.ViewModels;

namespace TriageSystem.Models
{
    public class TriageSystemContext : IdentityDbContext
    {

        public TriageSystemContext() : base()
        {

        }

        public TriageSystemContext(DbContextOptions<TriageSystemContext> options) : base(options)
        {
            
        }

        //public OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseLazyLoadingProxies();
        //}

        public virtual DbSet<Hospital> Hospitals { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<PatientCheckIn> PatientCheckIns { get; set; }
        public virtual DbSet<PatientWaitingList> PatientWaitingList { get; set; }
        public virtual DbSet<PatientAdmitted> PatientAdmitted { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Hospital>().ToTable("Hospitals");
            modelBuilder.Entity<Staff>().ToTable("Staff");
            //modelBuilder.Entity<TriageSystemUser>().ToTable("Users").Property(p => p.Id).HasColumnName("UserId");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            //modelBuilder.Entity<IdentityRoleClaim>().ToTable("RoleClaims");
            //modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaims");
            //modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogins");
            //modelBuilder.Entity<IdentityUserRole>().ToTable("UserRoles");
            //modelBuilder.Entity<IdentityUserToken>().ToTable("UserTokens");

            //modelBuilder.Entity<ApplicationUser>().ToTable("Users");
            //modelBuilder.Entity<Patient>().ToTable("Patients");
            //modelBuilder.Entity<PatientCheckIn>().ToTable("PatientCheckIns");
            //modelBuilder.Entity<PatientWaitingList>().ToTable("PatientWaitingList");
        }
        
        

        

    }
}
