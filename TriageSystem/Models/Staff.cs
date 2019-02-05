using System;
using System.ComponentModel.DataAnnotations;

namespace TriageSystemAPI.Models
{
    public class Staff
    {
        public int StaffID { get; set; }
        public int Hospital_id { get; set; }
        public string Full_name { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Date_of_birth { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
    }
}
