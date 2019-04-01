using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TriageSystem.Models
{
    public class PatientAdmitted
    {

        private string _PriorityString;
        private Priority _Priority;

        public int Id { get; set; }
        [ForeignKey("Patient")]
        [Display(Name = "Patient ID")]
        public int PatientId { get; set; }
        public string Infections { get; set; }
        public string Arrival { get; set; }
        [Required]
        public string Condition { get; set; }
        [Display(Name = "Final Condition")]
        public string FinalCondition { get; set; }
        public Priority Priority
        {
            get => _Priority;
            set => _Priority = value;
        }
        [NotMapped]
        public string PriorityString
        {
            get => _PriorityString;
            set
            {
                _PriorityString = value;
                Enum.TryParse(value, out _Priority);
            }

        }

        [Required]
        [Display(Name = "Time checked in")]
        public DateTime Time_checked_in { get; set; }
        [Required]
        [Display(Name = "Time triaged")]
        public DateTime Time_triaged { get; set; }
        [Display(Name = "Time admitted")]
        public DateTime? Time_admitted { get; set; }
        [Display(Name = "Time releassed")]
        public DateTime? Time_released { get; set; }

        [ForeignKey("Hospital")]
        [Display(Name = "Hospital ID")]
        public int HospitalID { get; set; }

        [Display(Name = "Flowchart Name")]
        public string FlowchartName { get; set; }



    }
}
