using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TriageSystem.Models;

namespace TriageSystem.Models
{
    public class DbInitializer
    {
        public static void Initialize(OnConfiguring context)
        {
            context.Database.EnsureCreated();

            CreateHospital(context);
            CreateStaff(context);
            CreatePatient(context);
        }

        private static void CreateHospital(OnConfiguring context)
        {
            if (context.Hospitals.Any())
            {
                return;   // DB has been seeded
            }

            var hospital = new Hospital { Name = "Regional Hospital", Location = "Limerick", Coordinates = "none" };

            context.Hospitals.Add(hospital);
            context.SaveChanges();
        }

        private static void CreateStaff(OnConfiguring context)
        {
            if (context.Staff.Any())
            {
                return;   // DB has been seeded
            }

            var hospital = context.Hospitals.First();

            var staff = new Staff { HospitalID = hospital.HospitalID, Full_name = "Patryk Leszczynski", Email = "e@mail.com", Gender = "M", Date_of_birth = "15/02/97", Department = "ED", Position = "Receptionist" };

            context.Staff.Add(staff);
            context.SaveChanges();
        }
        private static void CreatePatient(OnConfiguring context)
        {
            if (context.Patients.Any())
            {
                return;   // DB has been seeded
            }

            var patient = new Patient { PPS = "390849 F", Full_name = "Adam Smith", Address = "Limerick", Date_of_birth = "12/04/1990", Gender = "M", Nationality = "Irish"  };

            context.Patients.Add(patient);
            context.SaveChanges();
        }
    }
}
