using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriageSystem.Models
{
    public class UserSession
    {
        public int Id { get; set; }
        public int HospitalID { get; set; }
        public int StaffID { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
