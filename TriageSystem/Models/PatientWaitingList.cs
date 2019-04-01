using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TriageSystem.Models
{
    public class PatientWaitingList
    {
       
        private string _PriorityString;
        private Priority _Priority;
        private int _Duration;
        private Flowchart _Flowchart;

        public int Id { get; set; }
        [Display(Name = "Patient ID")]
        public int PatientId { get; set; }
        public string Infections { get; set; }
        [Required]
        public string Arrival { get; set; }
        [Required]
        public string Condition { get; set; }
        public Priority Priority { get => _Priority; set => _Priority = value;}
        
        public string PriorityString
        {
            get => _PriorityString; 
            set
            {
                _PriorityString = value;
                Enum.TryParse(value, out _Priority);
            }

        }

        [Display(Name = "Time checked in")]
        public DateTime Time_checked_in { get; set; }
        [Display(Name = "Time triaged")]
        public DateTime Time_triaged { get; set; }


        [Display(Name = "Hospital ID")]
        public int HospitalID { get; set; }

        
        public int Duration
        {
            get =>_Duration;
            set
            {
                _Duration = value;
                Expiry_time = Time_triaged.AddMinutes(_Duration);
            }
        }


        
        [Display(Name = "Expiry time")]
        public DateTime? Expiry_time { get; set; }


        
        [Display(Name = "Flowchart ID")]
        public int FlowchartId { get; set; }
        [Display(Name = "Flowchart Name")]
        public string FlowchartName { get; private set; }


        
        public Flowchart Flowchart
        {
            get => _Flowchart;
            set
            {
                _Flowchart = value;
                FlowchartName = _Flowchart.Name;
            }
        }

        
        public IEnumerable<SelectListItem> Flowcharts { get; set; }

        
        [Display(Name = "Patient History")]
        public IEnumerable<PatientAdmitted> PatientHistory { get; set; }


        public virtual Patient Patient { get; set; }
        //public virtual Hospital Hospital { get; set; }



    }
}
