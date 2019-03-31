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
        //[ForeignKey("Patient")]
        public int PatientId { get; set; }
        public string Infections { get; set; }
        [Required]
        public string Arrival { get; set; }
        [Required]
        public string Condition { get; set; }
        public Priority Priority { get => _Priority; set => _Priority = value;}
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

        public DateTime Time_checked_in { get; set; }
        //[NotMapped]
        //public DateTime Time_checked_in { get; set; }
        public DateTime Time_triaged { get; set; }


        //[ForeignKey("Hospital")]
        public int HospitalID { get; set; }

        [NotMapped]
        public int Duration
        {
            get =>_Duration;
            set
            {
                _Duration = value;
                Expiry_time = Time_triaged.AddMinutes(_Duration);
            }
        }


        [NotMapped]
        public DateTime Expiry_time { get; set; }


        [NotMapped]
        public int FlowchartId { get; set; }


        public string FlowchartName { get; private set; }


        [NotMapped]
        public Flowchart Flowchart
        {
            get => _Flowchart;
            set
            {
                _Flowchart = value;
                FlowchartName = _Flowchart.Name;
            }
        }

        [NotMapped]
        public IEnumerable<SelectListItem> Flowcharts { get; set; }

        [NotMapped]
        public IEnumerable<PatientAdmitted> PatientHistory { get; set; }


        public virtual Patient Patient { get; set; }
        //public virtual Hospital Hospital { get; set; }



    }
}
