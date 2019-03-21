using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TriageSystem.ViewModels
{
    public class PatientCheckInViewModel
    {
        public int Id { get; set; }
        [Required]
        public string PPS { get; set; }
        public string Infections { get; set; }
        [Required]
        public string Arrival { get; set; }
        public DateTime Time_checked_in { get; set; } 
        public int HospitalID { get; set; }
        public virtual List<SelectListItem> Arrivals { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "Home", Text = "Home" },
            new SelectListItem { Value = "Ambulance", Text = "Ambulance" },
        };
    }
}
