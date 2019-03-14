using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TriageSystem.Areas.Identity.Data;
using TriageSystem.Models;
using TriageSystem.ViewModels;

namespace TriageSystem.Controllers
{
    public class FlowchartController : Controller
    {
        UserManager<TriageSystemUser> _userManager;
        private readonly OnConfiguring _context;

        public FlowchartController(UserManager<TriageSystemUser> userManager, OnConfiguring context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            CreateFlowchartViewModel flowchart = new CreateFlowchartViewModel();
            return View(flowchart);
        }

        [HttpPost]
        public IActionResult AddDescription(CreateFlowchartViewModel flowchart)
        {
            List<List<String>> list = new List<List<string>>();
            list.Add(flowchart.Red.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList());
            list.Add(flowchart.Orange.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList());
            list.Add(flowchart.Yellow.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList());
            list.Add(flowchart.Green.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList());

            List<Discriminator> discriminators = new List<Discriminator>();

            int counter = 0;
            foreach(var colour in list)
            {
                foreach(var d in colour)
                {
                    Priority c;
                    switch(counter)
                    {
                        case 0:
                            c = Priority.Red;
                            break;
                        case 1:
                            c = Priority.Orange;
                            break;
                        case 2:
                            c = Priority.Yellow;
                            break;
                        case 3:
                            c = Priority.Green;
                            break;
                        default:
                            c = Priority.Blue;
                            break;
                    }
                    discriminators.Add(new Discriminator { Name = d, Priority = c, PriorityString = c.ToString()});
                }
            }

            AddDescriptionViewModel model = new AddDescriptionViewModel()
            {
                Discriminators = discriminators
            };

            return View(model);
        }
    }
}