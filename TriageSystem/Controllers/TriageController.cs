using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TriageSystem.Models;

namespace TriageSystem.Controllers
{
    [Authorize] // User needs to be singed in to display this view
    public class TriageController : Controller
    {

        //private IHubContext<NotificationHub> HubContext { get; set; }
        private readonly HttpClient _client = new HttpClient();
        private readonly string apiUrl;

        public TriageController(IOptions<ApiSettings> apiSettings)
        {
            apiUrl = apiSettings.Value.ApiConnection;
        }

        public IActionResult SelectFlowcharts()
        {
            var hospitalID = getHospitalID();
            var response = GetCheckIns(hospitalID);
            if (response.IsSuccessStatusCode)
            {
                var list = JsonConvert.DeserializeObject<IEnumerable<PatientCheckIn>>(response.Content.ReadAsStringAsync().Result);
                if (list.Count() == 0)
                {
                    TempData["Error"] = "No patients awaiting triage assessment!";
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
                else
                {
                    var patientCheckedIn = list.Where(p => p.Time_checked_in == list.Min(t => t.Time_checked_in)).FirstOrDefault();
                    var patientData = new PatientWaitingList { PatientId = patientCheckedIn.PatientId, Patient = patientCheckedIn.Patient, HospitalID = patientCheckedIn.HospitalID, Arrival = patientCheckedIn.Arrival, Infections = patientCheckedIn.Infections, Time_checked_in = patientCheckedIn.Time_checked_in };
                    List<Flowchart> flowcharts = GetFlowcharts();

                    var l = new List<SelectListItem>();
                    int index = 0;
                    foreach (var item in flowcharts)
                    {
                        l.Add(new SelectListItem { Text = item.Name, Value = index.ToString() });
                        index++;
                    }
                    
                    ViewBag.Flowcharts = l;
                    response = GetPatientHistory(patientCheckedIn.PatientId);
                    if(response.IsSuccessStatusCode)
                    {
                        
                        var patientHistory = JsonConvert.DeserializeObject<IEnumerable<PatientAdmitted>>(response.Content.ReadAsStringAsync().Result);
                        patientData.PatientHistory = patientHistory;
                    }

                    return View(patientData);
                }
            }
            return NotFound();
        }


        // TODO: refactor to use PatientId rather than pps
        public IActionResult Assessment(PatientWaitingList patientData)
        {
            var hospitalID = getHospitalID();
            var response = GetPatientById(patientData.PatientId);
            if(response.IsSuccessStatusCode)
            {
                var patient = JsonConvert.DeserializeObject<Patient>(response.Content.ReadAsStringAsync().Result);
                var flowchart = GetSelectedFlowchart(patientData.FlowchartId);
                patientData.Patient = patient;
                patientData.Flowchart = flowchart;
                return View(patientData);

            }
            return NotFound();
        }


        //****************************************
        // TODO: call signalR, make sure it's called everywhere else where required
        //******************************************************
        [HttpPost]
        public IActionResult GivePriority([FromBody] PatientWaitingList patientData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    patientData.Time_triaged = GetNow();
                    var response = GetPatientCheckIn(patientData.PatientId);
                    if (response.IsSuccessStatusCode)
                    {
                        var checkIn = JsonConvert.DeserializeObject<PatientCheckIn>(response.Content.ReadAsStringAsync().Result);
                        response = RemoveCheckIn(checkIn.Id);
                        if(response.IsSuccessStatusCode)
                        {
                            response = AddWaiting(patientData);
                            if(response.IsSuccessStatusCode)
                            {
                                return Json("Success");

                            }
                            else
                            {
                                return Json("Patient already exists in the list");
                            }
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
            return Json(getErrors());
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

        private int getHospitalID()
        {
            var h = User.Claims.Where(u => u.Type == "HospitalID").FirstOrDefault().Value;
            int hospitalID = 0;
            Int32.TryParse(h, out hospitalID);
            return hospitalID;
        }

        private static DateTime GetNow()
        {
            return DateTime.Now;
        }
    

        private HttpResponseMessage GetPatientById(int id)
        {
            AddHeader();
            var response = _client.GetAsync(apiUrl + "Patients/" + id).Result;
            return response;
        }



        private HttpResponseMessage GetCheckIns(int id)
        {
            AddHeader();
            var response = _client.GetAsync(apiUrl + "PatientCheckIns/" + id).Result;
            return response;
        }

        private HttpResponseMessage GetPatientCheckIn(int id)
        {
            AddHeader();
            var response = _client.GetAsync(apiUrl + "PatientCheckIns/patient/" + id).Result;
            return response;
        }

        private HttpResponseMessage GetPatientHistory(int id)
        {
            AddHeader();
            var response = _client.GetAsync(apiUrl + "PatientAdmitted/patient/" + id).Result;
            return response;
        }

        private HttpResponseMessage AddWaiting(PatientWaitingList patient)
        {
            var x = JsonConvert.SerializeObject(patient);
            AddHeader();
            var response = _client.PostAsJsonAsync(apiUrl + "PatientWaitingLists", patient).Result;
            return response;
        }

        private HttpResponseMessage RemoveCheckIn(int id)
        {
            AddHeader();
            var response = _client.DeleteAsync(apiUrl + "PatientCheckIns/" + id).Result;
            return response;
        }

        private void AddHeader()
        {
            var token = HttpContext.User.Claims.Where(u => u.Type == "Token").FirstOrDefault().Value;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

    }
}