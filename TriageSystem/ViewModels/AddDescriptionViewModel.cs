using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TriageSystem.Models;

namespace TriageSystem.ViewModels
{
    public class AddDescriptionViewModel
    {
        //public List<string> DiscriminatorsString { get; set; }

        //public List<string> Descriptions { get; set; } 
        public List<string> SeeAlso { get; set; }
        public string Notes { get; set; }

        public List<Discriminator> Discriminators { get; set; }

    }
}
