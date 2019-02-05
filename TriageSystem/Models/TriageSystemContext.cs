using Microsoft.EntityFrameworkCore;

namespace TriageSystemAPI.Models
{
    public class TriageSystemContext : DbContext
    {
        public TriageSystemContext() : base()
        {

        }

        public TriageSystemContext(DbContextOptions<TriageSystemContext> options) : base(options)
        {

        }

        //public virtual DbSet<Hospital> Hospitals { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }
        public virtual DbSet<User> Users { get; set; }
        //public virtual DbSet<Patient> Patients { get; set; }
        //public virtual DbSet<PatientCheckIn> PatientCheckIns { get; set; }
        //public virtual DbSet<PatientWaitingList> PatientWaitingList { get; set; }

        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Hospital>().ToTable("Hospitals");
            modelBuilder.Entity<Staff>().ToTable("Staff");
            modelBuilder.Entity<User>().ToTable("Users");
            //modelBuilder.Entity<Patient>().ToTable("Patients");
            //modelBuilder.Entity<PatientCheckIn>().ToTable("PatientCheckIns");
            //modelBuilder.Entity<PatientWaitingList>().ToTable("PatientWaitingList");
        }

    }
}
