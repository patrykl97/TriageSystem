using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TriageSystem.Areas.Identity.Data;

namespace TriageSystemAPI.Models
{
    public class TriageSystemContext : IdentityDbContext<TriageSystemUser>
    {
        public TriageSystemContext() : base()
        {

        }

        public TriageSystemContext(DbContextOptions<TriageSystemContext> options) : base(options)
        {

        }

        //public virtual DbSet<Hospital> Hospitals { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }
        //public virtual DbSet<ApplicationUser> Users { get; set; }
        //public virtual DbSet<Patient> Patients { get; set; }
        //public virtual DbSet<PatientCheckIn> PatientCheckIns { get; set; }
        //public virtual DbSet<PatientWaitingList> PatientWaitingList { get; set; }

        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Hospital>().ToTable("Hospitals");
            modelBuilder.Entity<Staff>().ToTable("Staff");
            //modelBuilder.Entity<ApplicationUser>().ToTable("Users");
            //modelBuilder.Entity<Patient>().ToTable("Patients");
            //modelBuilder.Entity<PatientCheckIn>().ToTable("PatientCheckIns");
            //modelBuilder.Entity<PatientWaitingList>().ToTable("PatientWaitingList");
        }

    }
}
