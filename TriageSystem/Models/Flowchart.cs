using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriageSystem.Models
{
    public class Flowchart
    {

        public List<List<List<string>>> Priorities { get; set; }

        public List<string> SeeAlso { get; set; }

        public string Notes { get; set; }

        public List<List<string>> Red => Priorities[0]; // index 0: red priority

        public List<List<string>> Orange => Priorities[1]; // index 1: orange priority

        public List<List<string>> Yellow => Priorities[2]; // index 2: yellow priority

        public List<List<string>> Green => Priorities[3]; // index 3: green priority

    }
}
