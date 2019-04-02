using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TriageSystem.Models;
using TriageSystem.ViewModels;

namespace TriageSystem.Controllers
{
    [Authorize] // User needs to be singed in to display this view
    public class FlowchartController : Controller
    {

        public IActionResult Index()
        {
            return View(GetFlowcharts());
        }

        public IActionResult Create()
        {
            CreateFlowchartViewModel flowchart = new CreateFlowchartViewModel();
            return View(flowchart);
        }


        public IActionResult Edit(string name)
        {
            string text = ReadFile(name);
            var flowchart = JsonConvert.DeserializeObject<Flowchart>(text);
            CreateFlowchartViewModel model = new CreateFlowchartViewModel
            {
                Name = name
            };
            //model.Discriminators = flowchart.Discriminators;
            foreach (var d in flowchart.Discriminators)
            {
                switch(d.Priority)
                {
                    case Priority.Red:
                        model.Red += d.Name + "\n";
                        break;
                    case Priority.Orange:
                        model.Orange += d.Name + "\n";
                        break;
                    case Priority.Yellow:
                        model.Yellow += d.Name + "\n";
                        break;
                    case Priority.Green:
                        model.Green += d.Name + "\n";
                        break;
                }
            }
            foreach(var item in flowchart.SeeAlso)
            {
                model.SeeAlso += item + "\n";
            }
            model.Notes = flowchart.Notes;
            model.Edit = true;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddDescription(CreateFlowchartViewModel flowchart)
        {
            if (ModelState.IsValid)
            {
                var model = InitiliseDescription(flowchart);
                return View(model);
            }
            TempData["Error"] = getErrors();
            return RedirectToAction(nameof(Index));

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
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Delete(string name)
        {
            string fileLocation = @".\Flowcharts\" + name + ".json";
            System.IO.File.Delete(fileLocation);
            return RedirectToAction(nameof(Index));
        }


        private static string ReadFile(string name)
        {
            name = name.Replace(" ", "_");
            string path = @".\Flowcharts\" + name + ".json";
            string text;
            var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                text = streamReader.ReadToEnd();
            }

            return text;
        }

        private static AddDescriptionViewModel InitiliseDescription(CreateFlowchartViewModel model)
        {
            Flowchart flowchart = new Flowchart();
            var Edit = model.Edit;
            if(Edit)
            {
                string name = model.Name;
                string text = ReadFile(name);
                flowchart = JsonConvert.DeserializeObject<Flowchart>(text);
            }


            List<List<String>> list = new List<List<string>>();
            list.Add(model.Red.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList());
            list.Add(model.Orange.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList());
            list.Add(model.Yellow.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList());
            list.Add(model.Green.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList());

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
                    var discriminator = new Discriminator { Name = d, Priority = c, PriorityString = c.ToString() };
                    if(Edit)
                    {
                        foreach(var disc in flowchart.Discriminators)
                        {
                            if (disc.Name == d && disc.Priority == c)
                            {
                                discriminator.Description = disc.Description;
                                break;
                            }

                        }
                    }
                    discriminators.Add(discriminator);
                }
                counter++;
            }

            AddDescriptionViewModel newModel = new AddDescriptionViewModel()
            {
                Name = model.Name,
                Discriminators = discriminators
            };


                //if (model.SeeAlso != null)
                //{
                    //var x = model.SeeAlso.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    newModel.SeeAlso = model.SeeAlso;
                //}

                newModel.Notes = model.Notes;

            return newModel;
        }




        private Flowchart ViewModelToFlowchart(AddDescriptionViewModel model)
        {
            var list = model.SeeAlso.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var flowchart = new Flowchart { Name = model.Name, Discriminators = model.Discriminators, SeeAlso = list, Notes = model.Notes };
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
                var d = new Discriminator { /*Priority = Priority.Green*/ };
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