using System;
using System.ComponentModel.DataAnnotations;

namespace TriageSystemAPI.Models
{
    public class PatientCheckIn
    {
        public int PPS { get; set; }
        public string Condition { get; set; }
        public DateTime Time_checked_in { get; set; }
        public int Hospital_id { get; set; }
    }
}
