﻿using System;
using System.ComponentModel.DataAnnotations;

namespace TriageSystem.Models
{
    public class Patient
    {
        [Key] 
        public string PPS { get; set; }
        public string Full_name { get; set; }
        public string Gender { get; set; }
        public string Date_of_birth { get; set; }
        public string Nationality { get; set; }
        public string Address { get; set; }
        
        
    }
}
