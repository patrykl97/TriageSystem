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
