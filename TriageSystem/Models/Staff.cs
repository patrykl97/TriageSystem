using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TriageSystem.Models
{
    public class Staff
    {
        public int StaffID { get; set; }
        [ForeignKey("Hospital")]
        public int HospitalID { get; set; }
        public string Full_name { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Date_of_birth { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }

        public virtual Hospital Hospital { get; set; }
    }
}
