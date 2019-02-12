using System;
using System.ComponentModel.DataAnnotations;

namespace TriageSystem.Models
{
    public class PatientWaitingList
    {
        public int PPS { get; set; }
        public string Condition { get; set; }
        public int Priority { get; set; }
        public DateTime Time_checked_in { get; set; }
        public int Hospital_id { get; set; }


    }
}
