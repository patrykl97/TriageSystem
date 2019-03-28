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
        public int PatientId { get; set; }
        public string Infections { get; set; }
        public string Arrival { get; set; }
        [Required]
        public string Condition { get; set; }
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
        public DateTime Time_checked_in { get; set; }
        [Required]
        public DateTime Time_triaged { get; set; }

        public DateTime Time_admitted { get; set; }
        public DateTime? Time_released { get; set; }

        [ForeignKey("Hospital")]
        public int HospitalID { get; set; }


        public string FlowchartName { get; set; }



    }
}
