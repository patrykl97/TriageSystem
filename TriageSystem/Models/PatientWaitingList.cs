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

        public int Id { get; set; }
        [ForeignKey("Patient")]
        public int PatientId { get; set; }
        public string PPS { get; set; }
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
        [NotMapped]
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
        [ForeignKey("Hospital")]
        public int HospitalID { get; set; }

        [NotMapped]
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


        [NotMapped]
        public DateTime Expiry_time { get; set; }


        [NotMapped]
        public int FlowchartId { get; set; }


        [NotMapped]
        public Flowchart Flowchart { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem> Flowcharts { get; set; }


        public virtual Patient Patient { get; set; }
        public virtual Hospital Hospital { get; set; }


     
    }
}
