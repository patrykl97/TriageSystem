using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TriageSystem.Areas.Identity.Data;
using TriageSystem.Models;
using TriageSystem.ViewModels;

namespace TriageSystem.Controllers
{
    [Authorize] // User needs to be singed in to display this view
    public class FlowchartController : Controller
    {
        //UserManager<TriageSystemUser> _userManager;
        //private readonly TriageSystemContext _context;

        public FlowchartController()
        {
            //_userManager = userManager;
            //_context = context;
        }

        public IActionResult Index()
        {
            return View(GetFlowcharts());
        }

        public IActionResult Create()
        {
            CreateFlowchartViewModel flowchart = new CreateFlowchartViewModel();
            return View(flowchart);
        }

        public IActionResult Delete(string name)
        {
            string fileLocation = @".\Flowcharts\" + name + ".json";
            System.IO.File.Delete(fileLocation);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

                if(flowchart.SeeAlso != null)
                {
                    var x = flowchart.SeeAlso.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    model.SeeAlso = x;
                }

                model.Notes = flowchart.Notes;
                return View(model);
            }
            TempData["Error"] = getErrors();
            return RedirectToAction("Index", "Home");

        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddFlowchart(AddDescriptionViewModel model)
        {
            var flowchart = ViewModelToFlowchart(model);
            string x = JsonConvert.SerializeObject(flowchart);
            string docPath = @"./Flowcharts";
            string fileName = flowchart.Name.Replace(" ", "_");
            fileName = fileName + ".json";
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, fileName))) 
            {
                outputFile.WriteLine(x);
            }
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        private Flowchart ViewModelToFlowchart(AddDescriptionViewModel model)
        {
            var flowchart = new Flowchart { Name = model.Name, Discriminators = model.Discriminators, SeeAlso = model.SeeAlso, Notes = model.Notes };
            return flowchart;
        }

        private List<Flowchart> GetFlowcharts()
        {
            string name, path;
            string[] filePaths = Directory.GetFiles(@"./Flowcharts");
            List<Flowchart> flowcharts = new List<Flowchart>();
            for (int i = 0; i < filePaths.Length; ++i)
            {
                path = filePaths[i];
                name = Path.GetFileName(path);
                name = name.Replace("_", " ");
                name = name.Replace(".json", "");

                string text;
                var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    text = streamReader.ReadToEnd();
                }
                var flowchart = JsonConvert.DeserializeObject<Flowchart>(text);
                flowchart.Name = name;
                var d = new Discriminator { Priority = Priority.Green };
                flowcharts.Add(flowchart);
            }
            return flowcharts;
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