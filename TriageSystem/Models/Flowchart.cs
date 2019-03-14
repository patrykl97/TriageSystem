using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace TriageSystem.Models
{
    public class Flowchart : IEnumerable<Discriminator>
    {

        public List<Discriminator> Discriminators { get; set; }
        
        public List<string> SeeAlso { get; set; }

        public string Notes { get; set; }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<Discriminator>)Discriminators).GetEnumerator();
        }

        IEnumerator<Discriminator> IEnumerable<Discriminator>.GetEnumerator()
        {
            foreach (var item in Discriminators)
            {
                yield return item;
            }
        }

    }
}
