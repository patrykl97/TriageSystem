using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TriageSystem.Models
{
    public class PatientWaitingList
    {
        public int Id { get; set; }
        [ForeignKey("Patient")]
        public string PPS { get; set; }
        public string Condition { get; set; }
        public Priority Priority { get; set; }
        public DateTime Time_checked_in { get; set; }
        [ForeignKey("Hospital")]
        public int HospitalID { get; set; }


        public virtual Patient Patient { get; set; }
        public virtual Hospital Hospital { get; set; }

    }
}
