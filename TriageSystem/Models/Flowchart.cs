using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriageSystem.Models
{
    public class Flowchart
    {
        List<Discriminator> Discriminators { get; set; }

        public Priorities Priorities { get; set; }

        public List<string> SeeAlso { get; set; }

        public string Notes { get; set; }



    }
}
