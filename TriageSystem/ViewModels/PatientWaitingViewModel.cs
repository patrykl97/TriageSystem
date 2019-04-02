using System;
using System.ComponentModel.DataAnnotations;
using TriageSystem.Models;

namespace TriageSystem.ViewModels
{
    public class PatientWaitingViewModel
    {

        private string _PriorityString;
        private Priority _Priority;
        private int _Duration;

        [Display(Name = "Patient ID")]
        public int PatientId { get; set; }
        public string PPS { get; set; }
        [Display(Name = "Full name")]
        public string Full_name { get; set; }
        public string Gender { get; set; }
        [Display(Name = "Date of birth")]
        public string Date_of_birth { get; set; }
        public string Nationality { get; set; }
        public string Address { get; set; }
        public string Infections { get; set; }
        public string Arrival { get; set; }
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
        [Display(Name = "Time checked in")]
        public DateTime Time_checked_in { get; set; }
        [Display(Name = "Hospital ID")]
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

        [Display(Name = "Expiry time")]
        public DateTime? Expiry_time { get; set; }
    }
}
