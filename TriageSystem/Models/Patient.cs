using System;
using System.ComponentModel.DataAnnotations;

namespace TriageSystemAPI.Models
{
    public class Patient
    {
        public int PPS { get; set; }
        public string Full_name { get; set; }
        public string Gender { get; set; }
        public string Date_of_birth { get; set; }
        public string Nationality { get; set; }
        public string Address { get; set; }
    }
}
