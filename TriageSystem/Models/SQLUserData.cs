using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TriageSystemAPI.Models;

namespace TriageSystem.Models
{
    public class SQLUserData
    {

        private TriageSystemContext _context { get; set; }
        public SQLUserData(TriageSystemContext context)
        {
            _context = context;
        }
        public void Add(User u)
        {
            _context.Add(u);
            _context.SaveChanges();
        }
        public User Get(int ID)
        {
            return _context.Users.FirstOrDefault(u => u.UserID == ID);
        }
        public IEnumerable<User> GetAll()
        {
            return _context.Users.ToList<User>();
        }
    }
}
