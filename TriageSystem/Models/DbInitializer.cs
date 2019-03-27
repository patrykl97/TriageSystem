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
            //CreateLists(context);
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
            patient = new Patient { PPS = "310149 F", Full_name = "Gerry Warren", Address = "Clare", Date_of_birth = "12/05/1990", Gender = "M", Nationality = "Irish" };
            context.Patients.Add(patient);
            patient = new Patient { PPS = "344849 F", Full_name = "Gavin O'Connell", Address = "Limerick", Date_of_birth = "12/04/1995", Gender = "M", Nationality = "Irish" };
            context.Patients.Add(patient);
            patient = new Patient { PPS = "351049 R", Full_name = "Adam O'Reilly", Address = "Limerick", Date_of_birth = "17/06/1980", Gender = "M", Nationality = "Irish" };
            context.Patients.Add(patient);
            patient = new Patient { PPS = "390049 S", Full_name = "Patrick Smith", Address = "Limerick", Date_of_birth = "18/02/1993", Gender = "M", Nationality = "Irish" };
            context.Patients.Add(patient);
            patient = new Patient { PPS = "392249 S", Full_name = "Alan Smith", Address = "Limerick", Date_of_birth = "18/02/1993", Gender = "M", Nationality = "Irish" };
            context.Patients.Add(patient);
            patient = new Patient { PPS = "391149 S", Full_name = "David Smith", Address = "Limerick", Date_of_birth = "18/02/1993", Gender = "M", Nationality = "Irish" };
            context.Patients.Add(patient);
            patient = new Patient { PPS = "393349 S", Full_name = "Patrick Warren", Address = "Limerick", Date_of_birth = "18/02/1993", Gender = "M", Nationality = "Irish" };
            context.Patients.Add(patient);
            context.SaveChanges();
        }

       
    }
}
