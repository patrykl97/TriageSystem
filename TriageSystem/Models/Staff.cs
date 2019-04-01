using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TriageSystem.Models
{
    public class Staff
    {

        [Display(Name = "Staff ID")]
        public int StaffID { get; set; }
        [Display(Name = "Hospital ID")]
        public int HospitalID { get; set; }
        [Required]
        [Display(Name = "Full name")]
        public string Full_name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        [Display(Name = "Date of Birth")]
        public string Date_of_birth { get; set; }
        [Required]
        public string Position { get; set; }
        [Required]
        public string Department { get; set; }

        public virtual Hospital Hospital { get; set; }
    }
}
