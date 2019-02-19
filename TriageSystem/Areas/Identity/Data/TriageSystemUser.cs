using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TriageSystem.Models;

namespace TriageSystem.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the TriageSystemUser class
    public class TriageSystemUser : IdentityUser
    {
        [ForeignKey("Staff")]
        public int StaffID { get; set; }
        public string UserType { get; set; }
        public bool Admin { get; set; }

        public virtual Staff Staff { get; set; }
    }
}
