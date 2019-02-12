//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using TriageSystemAPI.Models;

//namespace TriageSystem.Models
//{
//    public class SQLUserData
//    {

//        private TriageSystemContext _context { get; set; }
//        public SQLUserData(TriageSystemContext context)
//        {
//            _context = context;
//        }
//        public void Add(ApplicationUser u)
//        {
//            _context.Add(u);
//            _context.SaveChanges();
//        }
//        public ApplicationUser Get(int ID)
//        {
//            return _context.Users.FirstOrDefault(u => u.UserID == ID);
//        }
//        public IEnumerable<ApplicationUser> GetAll()
//        {
//            return _context.Users.ToList<ApplicationUser>();
//        }
//    }
//}
