using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriageSystem.Models
{
    public class Priorities
    {
        public List<List<string>> Red { get; set; }
        public List<List<string>> Orange { get; set; }
        public List<List<string>> Yellow { get; set; }
        public List<List<string>> Green { get; set; }
    }
}
