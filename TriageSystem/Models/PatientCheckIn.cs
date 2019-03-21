using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TriageSystem.Models
{
    public class PatientCheckIn
    {
        public int Id { get; set; }
        [Required]
        [ForeignKey("Patient")]
        public string PPS { get; set; }
        [Required]
        //public string Condition { get; set; }
        public DateTime Time_checked_in { get; set; }
        [ForeignKey("Hospital")]
        public int HospitalID { get; set; }

        public virtual Patient Patient { get; set; }
        public virtual Hospital Hospital { get; set; }
    }
}
