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
        [Required]
        public string Full_name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string Date_of_birth { get; set; }
        [Required]
        public string Position { get; set; }
        [Required]
        public string Department { get; set; }

        public virtual Hospital Hospital { get; set; }
    }
}
