﻿using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace TriageSystem.Models

{

    public class User
    {
        public int Id { get; set; }
        public int HospitalID { get; set; }
        public int StaffID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
