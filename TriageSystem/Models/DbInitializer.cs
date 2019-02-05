using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TriageSystemAPI.Models;

namespace TriageSystem.Models
{
    public class DbInitializer
    {
        public static void Initialize(TriageSystemContext context)
        {
            context.Database.EnsureCreated();

            // Look for any staff.
            if (context.Staff.Any())
            {
                return;   // DB has been seeded
            }

            var staff = new Staff { Hospital_id = 1, Full_name = "Patryk Leszczynski", Email = "e@mail.com", Gender = "M", Date_of_birth = "15/02/97", Department = "ED", Position = "Receptionist" };

            context.Staff.Add(staff);
            context.SaveChanges();
        }
    }
}
