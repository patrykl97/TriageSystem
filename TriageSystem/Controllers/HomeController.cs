using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TriageSystem.Areas.Identity.Data;
using TriageSystem.Hubs;
using TriageSystem.Models;
using TriageSystem.ViewModels;

namespace TriageSystem.Controllers
{
    [Authorize] // User needs to be singed in to display this view
    public class HomeController : Controller
    {
        UserManager<TriageSystemUser> _userManager;
        private readonly OnConfiguring _context;
        private IHubContext<NotificationHub> HubContext { get; set; }

        public HomeController(UserManager<TriageSystemUser> userManager, OnConfiguring context, IHubContext<NotificationHub> hubContext)
        {
            _userManager = userManager;
            _context = context;
            HubContext = hubContext;
        }



        public ActionResult Index(bool refresh = false)
        {
            ViewData["Refresh"] = refresh;
            return View();
        }

        [HttpPost]
        public IActionResult ListView(bool refresh)
        {
            if (refresh)
            {
                System.Threading.Thread.Sleep(100); // sleep for 100ms
            }
            var user = _userManager.GetUserAsync(User).Result;
            user.Staff.Hospital.PatientCheckInList.OrderBy(t => t.Time_checked_in);
            var orderedList = user.Staff.Hospital.PatientWaitingList.OrderBy(p => (int)(p.Priority)).ThenBy(t => t.Time_checked_in).ToList(); // orders by priority, then by time checked in
            user.Staff.Hospital.PatientWaitingList = orderedList;
            return PartialView(user);
        }

        public IActionResult RegisterPatient()
        {
            var user = _userManager.GetUserAsync(User).Result;
            var patientData = new PatientCheckInViewModel { HospitalID = user.Staff.HospitalID };

            List<SelectListItem> list = GetPPSList();
            ViewBag.PPS = list; //selectList;
            return View(patientData);
        }

        private List<SelectListItem> GetPPSList()
        {
            var patientList = _context.Patients.ToList();
            var list = new List<SelectListItem>();

            foreach (var p in patientList)
            {
                list.Add(new SelectListItem { Text = p.PPS, Value = p.toString() });
            }
            list.Insert(0, new SelectListItem { Text = "Please Select...", Value = string.Empty });
            return list;
        }

        // TODO: prevent adding patients that are already in the patientList
        [HttpPost]
        public async Task<IActionResult> RegisterPatient( PatientCheckInViewModel patientData)
        {
            if (ModelState.IsValid)
            {
                Patient patient = null;
                try
                {
                    patientData.Time_checked_in = GetNow();
                    if (patientData.PPS != "" && patientData.PPS != null)
                    {
                        var patients = _context.Patients.Where(p => p.PPS == patientData.PPS);
                        if(patients.Count() > 0)
                        {
                            patient = patients.First();
                        }

                    }
                    else
                    {
                        var patients = _context.Patients.Where(p => p.Full_name == patientData.Full_name);
                        patients = patients.Where(p => p.Gender == patientData.Gender);
                        if (patientData.Date_of_birth != null)
                        {
                            patients = patients.Where(p => p.Date_of_birth == patientData.Date_of_birth);
                        }
                        if (patients.Count() > 0)
                        {
                            patient = patients.First();
                        }
                    }
                    if (patient == null)
                    {
                        patient = new Patient { PPS = patientData.PPS, Full_name = patientData.Full_name, Gender = patientData.Gender, Date_of_birth = patientData.Date_of_birth, Nationality = patientData.Nationality, Address = patientData.Address };
                        _context.Patients.Add(patient);
                        await _context.SaveChangesAsync();
                    }
                    var patientCheckIn = new PatientCheckIn { PatientId = patient.Id, PPS = patient.PPS, HospitalID = patientData.HospitalID, Arrival = patientData.Arrival, Infections = patientData.Infections, Time_checked_in = patientData.Time_checked_in };
                    _context.PatientCheckIns.Add(patientCheckIn);
                    await _context.SaveChangesAsync();
                    await HubContext.Clients.All.SendAsync("SendNotification", patientData.HospitalID.ToString());
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index)); 
            }
            List<SelectListItem> list = GetPPSList();
            ViewBag.PPS = list; //selectList;
            return View(patientData); 
        }

        public IActionResult SelectFlowcharts()
        {
            var user = _userManager.GetUserAsync(User).Result;
            if(user.Staff.Hospital.PatientCheckInList.Count == 0)
            {
                TempData["Error"] = "No patients awaiting triage assessment!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var patientCheckedIn = user.Staff.Hospital.PatientCheckInList.First();
                var patientData = new PatientWaitingList {PatientId = patientCheckedIn.PatientId, PPS = patientCheckedIn.PPS, Patient = patientCheckedIn.Patient, HospitalID = patientCheckedIn.HospitalID };
                List<Flowchart> flowcharts = GetFlowcharts();
                //ViewBag.FlowchartNames = flowchartNames.Select(f => new SelectListItem { Text = f, Value = f });

                var list = new List<SelectListItem>();
                int index = 0;
                foreach (var item in flowcharts)
                {
                    list.Add(new SelectListItem { Text = item.Name, Value = index.ToString() });
                    index++;
                }
                //ViewBag.FlowchartNames = list.AsEnumerable();
                //patientData.Flowcharts = list;
                ViewBag.Flowcharts = list;
                return View(patientData);
            }

        }

        //private List<string> GetFlowchartNames()
        //{
        //    string name, path;
        //    string[] filePaths = Directory.GetFiles(@"./Flowcharts");
        //    List<string> flowchartNames = new List<string>();
        //    for (int i = 0; i < filePaths.Length; ++i)
        //    {
        //        path = filePaths[i];
        //        name = Path.GetFileName(path);
        //        name = name.Replace("_", " ");
        //        name = name.Replace(".json", "");
        //        flowchartNames.Add(name);
        //    }
        //    return flowchartNames;
        //}

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

        private Flowchart GetSelectedFlowchart(int index)
        {
            string name, path;
            string[] filePaths = Directory.GetFiles(@"./Flowcharts");
            path = filePaths[index];
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
            return flowchart;
        }


        // TODO: refactor to use PatientId rather than pps
        public IActionResult TriageAssessment(PatientWaitingList patient)
        {
            var user = _userManager.GetUserAsync(User).Result;
            //var patientData = user.Staff.Hospital.PatientCheckInList.First(p => p.PatientId == patientId);
            var flowchart = GetSelectedFlowchart(patient.FlowchartId);
            patient.Flowchart = flowchart;
            //var patient = new PatientWaitingList { PPS = patientData.PPS, Condition = condition, HospitalID = patientData.HospitalID, Flowchart = flowcharts[0] };
            return View(patient);
        }

        [HttpPost]
        public async Task<IActionResult> GivePriority([FromBody] PatientWaitingList patientData) 
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //patientData.PPS = patientData.PPS.Replace("_", " ");
                    //var user = _userManager.GetUserAsync(User).Result;
                    //var patient = _context.PatientCheckIns.Where(p => p.PPS == patientData.PPS).First();
                    //patientData.HospitalID = patient.HospitalID;
                    patientData.Time_checked_in = GetNow();
                    //patientData.PatientId = patient.PatientId;
                    _context.PatientWaitingList.Add(patientData);
                    _context.PatientCheckIns.Remove(_context.PatientCheckIns.Where(p => p.PatientId == patientData.PatientId).First());
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return Json("Success");
            }
            return Json(getErrors());
        }


        //[HttpPost]
        //public JsonResult TriageAssessmentGenerateUrl([FromBody] string [] array)
        //{
        //    //string pps = array[0].Replace(" ", "_");
        //    int[] flowchart = array.Skip(1).Select(int.Parse).ToArray();
            
        //    string url = Url.Action("TriageAssessment", "Home", new { pps, flowchart });
        //    return Json(url);
        //}

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

        private static DateTime GetNow()
        {
            return DateTime.Now;
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        //public PartialViewResult ShowError(String sErrorMessage)
        //{
        //    return PartialView("_Error");
        //}

        private List<Flowchart> CreateFlowcharts()
        {
            List<Flowchart> flowcharts = new List<Flowchart>(); //GetFlowchartNames();
            var d = new List<Discriminator>();
            d.Add(new Discriminator { Name = "Airway compromise", Description = "", PriorityString = Priority.Red.ToString(), Priority = Priority.Red });
            d.Add(new Discriminator { Name = "Inadequate breathing", Description = "", PriorityString = Priority.Red.ToString(), Priority = Priority.Red });
            d.Add(new Discriminator { Name = "Shock", Description = "", PriorityString = Priority.Red.ToString(), Priority = Priority.Red });
            d.Add(new Discriminator { Name = "Vomiting blood", Description = "Vomited blood may be fresh (bright or dark red) or coffee ground in appearance", PriorityString = Priority.Orange.ToString(), Priority = Priority.Orange });
            d.Add(new Discriminator { Name = "Persistent vomiting", Description = "Vomiting that is continuous or that occurs without any respite between episodes", PriorityString = Priority.Yellow.ToString(), Priority = Priority.Yellow });
            d.Add(new Discriminator { Name = "Vomiting", Description = "Any emesis fulfils this criterion", PriorityString = Priority.Green.ToString(), Priority = Priority.Green });
            var list = new List<string>();
            list.Add("Diarrhoea and vomitin");         
            list.Add("Gl bleeding");
            list.Add("Pregnancy");
            var notes = "This is a presentation defined flow diagram. Abdominal pain is a common cause of presentation of surgical emergencies. " +
                "A number of general discriminators are used including Life threat and Pain. Specific discriminators are included in the ORANGE and YELLOW categories to ensure that the more severe pathologies are appropriately triaged. " +
                "In particular, discriminators are included to ensure that patients with moderate and severe GI bleeding and those with signs of retroperitoneal or diaphragmatic irritation are given sufficiently high categorisation.";
            var flowchart = new Flowchart { Discriminators = d, SeeAlso = list, Notes = notes};
            //var x = new JavaScriptSerializer().Serialize();
            string[] x = new string[4];
            string jsonObject = JsonConvert.SerializeObject(x);
            //flowchart.Json = jsonObject;
            flowcharts.Add(flowchart);
            return flowcharts;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
