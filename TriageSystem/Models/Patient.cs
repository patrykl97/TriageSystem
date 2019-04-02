using System.ComponentModel.DataAnnotations;

namespace TriageSystem.Models
{
    public class Patient
    {
        [Key] 
        public int Id { get; set; }
        public string PPS { get; set; }

        [Display(Name = "Full name")]
        public string Full_name { get; set; }
        public string Gender { get; set; }
        [Display(Name = "Date of birth")]
        public string Date_of_birth { get; set; }
        public string Nationality { get; set; }
        public string Address { get; set; }
        
        
        public string toString()
        {
            return PPS + ", " + Full_name + ", " + Gender + ", " + Date_of_birth + ", " + Nationality + ", " + Address;
        }
        
    }
}
