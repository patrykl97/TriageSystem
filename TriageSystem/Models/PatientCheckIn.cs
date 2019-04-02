using System;
using System.ComponentModel.DataAnnotations;

namespace TriageSystem.Models
{
    public class PatientCheckIn
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Patient ID")]
        public int PatientId { get; set; }

        //[PPSAttribute]
        //public string PPS { get; set; }
        public string Infections { get; set; }
        [Required]
        public string Arrival { get; set; }
        [Display(Name = "Time checked in")]
        public DateTime Time_checked_in { get; set; }
        [Display(Name = "Time triaged")]
        public DateTime? Time_triaged { get; set; }
        [Display(Name = "Hospital ID")]
        public int HospitalID { get; set; }

        public virtual Patient Patient { get; set; }
        //public virtual Hospital Hospital { get; set; }



    }
}

