using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

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
