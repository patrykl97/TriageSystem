using System;
using System.ComponentModel.DataAnnotations;

namespace TriageSystemAPI.Models
{
    public class User
    {
        public int UserID { get; set; }
        public int StaffID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }
        public bool Admin { get; set; }

        public Staff Staff { get; set; }
    }
}
