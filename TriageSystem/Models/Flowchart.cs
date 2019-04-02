using Newtonsoft.Json;
using System.Collections.Generic;

namespace TriageSystem.Models
{
    public class Flowchart
    {
        [JsonIgnore]
        public string Name { get; set; }

        [JsonProperty("SeeAlso")]
        public List<string> SeeAlso { get; set; }

        [JsonProperty("Notes")]
        public string Notes { get; set; }

        public List<Discriminator> Discriminators { get; set; }

        

        //public IEnumerator GetEnumerator()
        //{
        //    return ((IEnumerable<Discriminator>)Discriminators).GetEnumerator();
        //}

        //IEnumerator<Discriminator> IEnumerable<Discriminator>.GetEnumerator()
        //{
        //    foreach (var item in Discriminators)
        //    {
        //        yield return item;
        //    }
        //}

    }
}
