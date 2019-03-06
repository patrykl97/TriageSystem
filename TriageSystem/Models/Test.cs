        using System;
        using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TriageSystem.Models
    {
        public class Test
        {

            public string PPS { get; set; }
            public string Condition { get; set; }
            public int HospitalID { get; set; }

        }
    }

