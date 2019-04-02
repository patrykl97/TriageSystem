using Newtonsoft.Json;
using System.Collections.Generic;
using TriageSystem.Models;

namespace TriageSystem.ViewModels
{
    public class AddDescriptionViewModel
    {

        [JsonIgnore]
        public string Name { get; set; }

        public string SeeAlso { get; set; }
        public string Notes { get; set; }

        public List<Discriminator> Discriminators { get; set; }

        public bool Edit { get; set; }

    }
}
