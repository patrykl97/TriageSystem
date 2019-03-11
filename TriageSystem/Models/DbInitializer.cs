﻿using System;
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

        private static void CreateLists(OnConfiguring context)
        {
            if (context.PatientCheckIns.Any())
            {
                return;   // DB has been seeded
            }

            DateTime myDate = DateTime.ParseExact("2019-02-20 19:14:30", "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
            var checkIn = new PatientCheckIn { PPS = "390849 F", HospitalID = 1, Condition = "Broken leg", Time_checked_in = myDate };
            context.PatientCheckIns.Add(checkIn);

            myDate = DateTime.ParseExact("2019-02-20 19:17:30", "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
            checkIn = new PatientCheckIn { PPS = "310149 F", HospitalID = 1, Condition = "Headache", Time_checked_in = myDate };
            context.PatientCheckIns.Add(checkIn);

            myDate = DateTime.ParseExact("2019-02-20 19:19:30", "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
            checkIn = new PatientCheckIn { PPS = "344849 F", HospitalID = 1, Condition = "Vomiting", Time_checked_in = myDate };
            context.PatientCheckIns.Add(checkIn);

            myDate = DateTime.ParseExact("2019-02-20 19:17:30", "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
            var waiting = new PatientWaitingList { PPS = "351049 R", HospitalID = 1, Condition = "Heart attack", Time_checked_in = myDate, Priority = Priority.Red };
            context.PatientWaitingList.Add(waiting);

            myDate = DateTime.ParseExact("2019-02-20 19:17:30", "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
            waiting = new PatientWaitingList { PPS = "390049 S", HospitalID = 1, Condition = "Sprained ankle", Time_checked_in = myDate, Priority = Priority.Green };
            context.PatientWaitingList.Add(waiting);

            myDate = DateTime.ParseExact("2019-02-20 19:19:30", "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
            waiting = new PatientWaitingList { PPS = "392249 S", HospitalID = 1, Condition = "Vomiting", Time_checked_in = myDate, Priority = Priority.Green };
            context.PatientWaitingList.Add(waiting);

            myDate = DateTime.ParseExact("2019-02-20 19:17:31", "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
            waiting = new PatientWaitingList { PPS = "391149 S", HospitalID = 1, Condition = "Internal bleeding", Time_checked_in = myDate, Priority = Priority.Red};
            context.PatientWaitingList.Add(waiting);

            myDate = DateTime.ParseExact("2019-02-20 19:16:59", "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
            waiting = new PatientWaitingList { PPS = "393349 S", HospitalID = 1, Condition = "Respitory problems", Time_checked_in = myDate, Priority = Priority.Red };
            context.PatientWaitingList.Add(waiting);

            context.SaveChanges();
        }
    }
}
