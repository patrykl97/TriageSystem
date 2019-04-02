using System.Collections.Generic;

namespace TriageSystem.Models
{
    public class Hospital
    {
        public int HospitalID { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Coordinates { get; set; }

        //public virtual ICollection<Staff> StaffList { get; set; }
        public virtual ICollection<PatientCheckIn> PatientCheckInList { get; set; }
        public virtual ICollection<PatientWaitingList> PatientWaitingList { get; set; }
    }
}
