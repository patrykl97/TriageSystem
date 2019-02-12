using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TriageSystem.Models
{
    public class Hospital
    {
        public int Hospital_id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Coordinates { get; set; }
        public IEnumerable<Staff> StaffList { get; set; }
    }
}
