using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
            if (ModelState.IsValid)
            {
                List<List<String>> list = new List<List<string>>();
                list.Add(flowchart.Red.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList());
                list.Add(flowchart.Orange.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList());
                list.Add(flowchart.Yellow.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList());
                list.Add(flowchart.Green.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList());

                List<Discriminator> discriminators = new List<Discriminator>();

                int counter = 0;
                foreach (var colour in list)
                {
                    foreach (var d in colour)
                    {
                        Priority c;
                        switch (counter)
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
                        discriminators.Add(new Discriminator { Name = d, Priority = c, PriorityString = c.ToString() });
                    }
                    counter++;
                }

                AddDescriptionViewModel model = new AddDescriptionViewModel()
                {
                    Name = flowchart.Name,
                    Discriminators = discriminators
                };
                model.SeeAlso = flowchart.SeeAlso.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
                model.Notes = flowchart.Notes;


                return View(model);
            }
            TempData["Error"] = getErrors();
            return RedirectToAction("Index", "Home");

        }
        
        [HttpPost]
        public void AddFlowchart(AddDescriptionViewModel model)
        {
            var flowchart = ViewModelToFlowchart(model);
            string x = JsonConvert.SerializeObject(flowchart);
            string docPath = @"./Flowcharts";
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, flowchart.Name + ".json"))) 
            {
                outputFile.WriteLine(x);
            }
            var m = JsonConvert.DeserializeObject<Flowchart>(x);
            var safafads = 0;       
     
        }

        private Flowchart ViewModelToFlowchart(AddDescriptionViewModel model)
        {
            var flowchart = new Flowchart { Name = model.Name, Discriminators = model.Discriminators, SeeAlso = model.SeeAlso, Notes = model.Notes };
            return flowchart;
        }

        private string getErrors()
        {
            string errors = "";
            foreach (var value in ModelState.Values)
            {
                foreach (var error in value.Errors)
                {
                    errors += error.ErrorMessage + " ";
                }
            }
            return errors;
        }

    }
}