using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TriageSystem.Validations;

namespace TriageSystem.ViewModels
{
    public class PatientCheckInViewModel
    {
        public int PatientId { get; set; }
        //[Required]
        [PPSAttribute]
        public string PPS { get; set; }
        [Required]
        [Display(Name = "Full name")]
        public string Full_name { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        [Display(Name = "Date of birth")]
        public string Date_of_birth { get; set; }
        [Required]
        public string Nationality { get; set; }
        public string Address { get; set; }

        public string Infections { get; set; }
        [Required]
        public string Arrival { get; set; }
        [Display(Name = "Time checked in")]
        public DateTime Time_checked_in { get; set; }
        public int HospitalID { get; set; }
        public List<SelectListItem> Arrivals { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "Home", Text = "Home" },
            new SelectListItem { Value = "Ambulance", Text = "Ambulance" },
        };
    }
}
