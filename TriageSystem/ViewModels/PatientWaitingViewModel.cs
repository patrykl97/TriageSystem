using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TriageSystem.Models;

namespace TriageSystem.ViewModels
{
    public class PatientWaitingViewModel
    {

        private string _PriorityString;
        private Priority _Priority;
        private int _Duration;

        public int PatientId { get; set; }
        public string PPS { get; set; }
        public string Full_name { get; set; }
        public string Gender { get; set; }
        public string Date_of_birth { get; set; }
        public string Nationality { get; set; }
        public string Address { get; set; }
        //[Required]
        public string Condition { get; set; }
        public Priority Priority
        {
            get
            {
                return _Priority;
            }
            set
            {
                _Priority = value;
            }
        }
        public string PriorityString
        {
            get
            {
                return _PriorityString;
            }
            set
            {
                _PriorityString = value;
                Enum.TryParse(value, out _Priority);
            }

        }
        public DateTime Time_checked_in { get; set; }
        public int HospitalID { get; set; }
        public int Duration
        {
            get
            {
                return _Duration;
            }
            set
            {
                _Duration = value;
                Expiry_time = Time_checked_in.AddMinutes(_Duration);
            }
        }
        public DateTime Expiry_time { get; set; }
    }
}
