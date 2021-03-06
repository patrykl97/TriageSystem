﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TriageSystem.Hubs;
using TriageSystem.Models;
using TriageSystem.ViewModels;

namespace TriageSystem.Controllers
{
    [Authorize] // User needs to be singed in to display this view
    public class PatientController : Controller
    {
        //UserManager<IdentityUser> _userManager;
        //private TriageSystemContext _context;
        private IHubContext<NotificationHub> HubContext { get; set; }
        private readonly HttpClient _client = new HttpClient();
        //private readonly IOptions<ApiSettings> _apiSettings; 
        private readonly string apiUrl;

        public PatientController(IHubContext<NotificationHub> hubContext, IOptions<ApiSettings> apiSettings )
        {
            //_userManager = userManager;
            //_context = context;
            HubContext = hubContext;
            apiUrl = apiSettings.Value.ApiConnection;

        }

        public IActionResult Register()
        {
            //var user = _userManager.GetUserAsync(User).Result;
            var h = User.Claims.Where(u => u.Type == "HospitalID").FirstOrDefault().Value;
            int hospitalID = 0;
            Int32.TryParse(h, out hospitalID);
            var patientData = new PatientCheckInViewModel { HospitalID = hospitalID };

            List<SelectListItem> list = GetPPSList();
            ViewBag.PPS = list; //selectList;
            return View(patientData);
        }

        private List<SelectListItem> GetPPSList()
        {
            var response = GetAll();
            if (response.IsSuccessStatusCode)
            {
                var patientList = JsonConvert.DeserializeObject<List<Patient>>(response.Content.ReadAsStringAsync().Result);
                var list = new List<SelectListItem>();

                foreach (var p in patientList)
                {
                    list.Add(new SelectListItem { Text = p.PPS, Value = p.toString() });
                }
                list.Insert(0, new SelectListItem { Text = "Please Select...", Value = string.Empty });
                return list;
            }
            return null;
        }

        // TODO: prevent adding patients that are already in the patientList
        // TODO: if PPS != null and all other properties exist in the db, update the PPS
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(PatientCheckInViewModel patientData)
        {
            if (ModelState.IsValid)
            {
                Patient patient = null;
               


                try
                {
                    patientData.Time_checked_in = GetNow();
                    if (patientData.PPS != "" && patientData.PPS != null)
                    {
                        var response = GetByPps(patientData.PPS);
                        if (response.IsSuccessStatusCode)
                        {
                            patient = JsonConvert.DeserializeObject<Patient>(response.Content.ReadAsStringAsync().Result);
                        }

                    }
                    else
                    {
                        var response = GetAll();
                        if (response.IsSuccessStatusCode)
                        {
                            var patients = JsonConvert.DeserializeObject<IEnumerable<Patient>>(response.Content.ReadAsStringAsync().Result);

                            patients = patients.Where(p => p.Full_name == patientData.Full_name);
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
                    }
                    if (patient == null)
                    {
                        patient = new Patient { PPS = patientData.PPS, Full_name = patientData.Full_name, Gender = patientData.Gender, Date_of_birth = patientData.Date_of_birth, Nationality = patientData.Nationality, Address = patientData.Address };
                        var response = Add(patient);
                        if(response.IsSuccessStatusCode)
                        {
                            var ee = response.Content;
                            var bb = response.Content.ReadAsStringAsync().Result;
                            patient = JsonConvert.DeserializeObject<Patient>(response.Content.ReadAsStringAsync().Result);

                            var ii = 0;
                        }
                   
                    }
                    var patientCheckIn = new PatientCheckIn { PatientId = patient.Id, HospitalID = patientData.HospitalID, Arrival = patientData.Arrival, Infections = patientData.Infections, Time_checked_in = patientData.Time_checked_in };
                    var r = AddCheckIn(patientCheckIn);
                    if(r.IsSuccessStatusCode)
                    {
                        await HubContext.Clients.All.SendAsync("ReceiveNotification", patientData.HospitalID);

                    }
                    else
                    {
                        ViewData["Error"] = r.Content.ReadAsStringAsync().Result;
                        return View(patientData);
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            List<SelectListItem> list = GetPPSList();
            ViewBag.PPS = list; //selectList;
            return View(patientData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CheckedIn(int id)
        {
            //var user = _userManager.GetUserAsync(User).Result;
            var response = GetCheckIn(id);
            if(response.IsSuccessStatusCode)
            {
                var patientCheckedIn = JsonConvert.DeserializeObject<PatientCheckIn>(response.Content.ReadAsStringAsync().Result);
                var p = patientCheckedIn.Patient;
                var patientViewModel = new PatientCheckInViewModel
                {
                    PatientId = id,
                    PPS = p.PPS,
                    Full_name = p.Full_name,
                    Gender = p.Gender,
                    Date_of_birth = p.Date_of_birth,
                    Nationality = p.Nationality,
                    Address = p.Address,
                    Infections = patientCheckedIn.Infections,
                    Arrival = patientCheckedIn.Arrival,
                    Time_checked_in = patientCheckedIn.Time_checked_in,
                    HospitalID = patientCheckedIn.HospitalID
                };

                return View(patientViewModel);
            }
            return NotFound();        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Actions(int id)
        {
            //var user = _userManager.GetUserAsync(User).Result;
            var response = GetWaitingPatient(id);
            if (response.IsSuccessStatusCode)
            {
                var patientData = JsonConvert.DeserializeObject<PatientWaitingList>(response.Content.ReadAsStringAsync().Result);

                //var patientData = user.Staff.Hospital.PatientWaitingList.Where(m => m.PatientId == id).FirstOrDefault();  //_context.PatientWaitingList.Where(m => m.PatientId == id).FirstOrDefault();
                var p = patientData.Patient;

                var patientWaitingViewModel = new PatientWaitingViewModel
                {
                    PatientId = id,
                    PPS = p.PPS,
                    Full_name = p.Full_name,
                    Gender = p.Gender,
                    Date_of_birth = p.Date_of_birth,
                    Nationality = p.Nationality,
                    Address = p.Address,
                    Condition = patientData.Condition,
                    Priority = patientData.Priority,
                    Expiry_time = patientData.Expiry_time,
                    Infections = patientData.Infections,
                    Arrival = patientData.Arrival,
                    Time_checked_in = patientData.Time_checked_in,
                    HospitalID = patientData.HospitalID
                };


                return View(patientWaitingViewModel);
            }
            return NotFound();
        }

        // ***********************************************
        // TODO: refactor these 2 methods to reuse code
        //       
        // ***********************************************
        [HttpPost]
        public async Task<IActionResult> PostAjax(int id)
        {
            if (id > 0)
            {
                try
                {
                    //var user = _userManager.GetUserAsync(User).Result;
                    var response = GetWaitingPatient(id);
                    if (response.IsSuccessStatusCode)
                    {
                        var patient = JsonConvert.DeserializeObject<PatientWaitingList>(response.Content.ReadAsStringAsync().Result);
                        var patientData = new PatientCheckIn { PatientId = patient.PatientId, Arrival = patient.Arrival, HospitalID = patient.HospitalID, Infections = patient.Infections, Time_checked_in = patient.Time_checked_in, Time_triaged = patient.Time_triaged };
                        var r = RemoveWaiting(patient.Id);
                        if(r.IsSuccessStatusCode)
                        {
                            var s = AddCheckIn(patientData);
                            if(s.IsSuccessStatusCode)
                            {
                                await HubContext.Clients.All.SendAsync("ReceiveNotification", patient.HospitalID);
                            }
                        }
                        else
                        {
                            return Json("Patient already exists in the list");
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return Json("Success");
            }
            return Json("Error");
        }


        // TODO: add check for whether times has expired, perhaphs leave it, re-triage might be done before time expires
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post(int id)
        {
            int i = id;
            //Int32.TryParse(id, out i);
            if (i > 0)
            {
                try
                {
                    var response = GetWaitingPatient(id);
                    if (response.IsSuccessStatusCode)
                    {
                        var patient = JsonConvert.DeserializeObject<PatientWaitingList>(response.Content.ReadAsStringAsync().Result);
                        var patientData = new PatientCheckIn { PatientId = patient.PatientId, Arrival = patient.Arrival, HospitalID = patient.HospitalID, Infections = patient.Infections, Time_checked_in = patient.Time_checked_in, Time_triaged = patient.Time_triaged };
                        var r = RemoveWaiting(patient.Id);
                        if (r.IsSuccessStatusCode)
                        {
                            var s = AddCheckIn(patientData);
                            await HubContext.Clients.All.SendAsync("ReceiveNotification", patient.HospitalID);
                        }
                        else
                        {
                            TempData["Error"] = r.Content.ReadAsStringAsync().Result;
                            return RedirectToAction(nameof(HomeController.Index), "Home");
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Admit(int id)
        {
            await AddToAdmitted(id);
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendHome(int id)
        { 
            await AddToAdmitted(id, true);
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }


        public IActionResult AdmittedPatients()
        {
            var h = User.Claims.Where(u => u.Type == "HospitalID").FirstOrDefault().Value;
            int hospitalID = 0;
            Int32.TryParse(h, out hospitalID);
            var response = GetHospitalAdmitted(hospitalID);
            var message = response.Content.ReadAsStringAsync().Result;
            IEnumerable<PatientAdmitted> list = new List<PatientAdmitted>();
            try
            {
                 list = JsonConvert.DeserializeObject<IEnumerable<PatientAdmitted>>(message);
            }
            catch
            {
                throw;
            }
            return View(list);
        }


        private async Task<IActionResult> AddToAdmitted(int id, bool sentHome = false)
        {
            if (id > 0)
            {
                try
                {
                    var response = GetWaitingPatient(id);
                    var patientData = JsonConvert.DeserializeObject<PatientWaitingList>(response.Content.ReadAsStringAsync().Result);
                    var patientAdmitted = new PatientAdmitted
                    {
                        PatientId = id,
                        Condition = patientData.Condition,
                        Priority = patientData.Priority,
                        Infections = patientData.Infections,
                        Arrival = patientData.Arrival,
                        Time_checked_in = patientData.Time_checked_in,
                        Time_triaged = patientData.Time_triaged,
                        HospitalID = patientData.HospitalID,
                        FlowchartName = patientData.FlowchartName
                    };

                    if (sentHome)
                    {
                        patientAdmitted.Time_released = GetNow();
                        patientAdmitted.FinalCondition = "Sent home";
                    }
                    else
                    {
                        patientAdmitted.Time_admitted = GetNow();
                    }
                    response = AdmitPatient(patientAdmitted);
                    if(response.IsSuccessStatusCode)
                    {
                        response = RemoveWaiting(patientData.Id);
                        await HubContext.Clients.All.SendAsync("ReceiveNotification", patientData.HospitalID);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
            return null;
        }

        private HttpResponseMessage Add(Patient patient)
        {
            AddHeader();
            var response = _client.PostAsJsonAsync(apiUrl + "Patients", patient).Result;
            return response;
        }

        private HttpResponseMessage AddCheckIn(PatientCheckIn p)
        {
            AddHeader();
            var response = _client.PostAsJsonAsync(apiUrl + "PatientCheckIns", p).Result;
            return response;
        }

        private HttpResponseMessage AdmitPatient(PatientAdmitted patient)
        {
            AddHeader();
            var response = _client.PostAsJsonAsync(apiUrl + "PatientAdmitted", patient).Result;
            return response;
        }

        private HttpResponseMessage GetWaitingPatient(int id)
        {
            AddHeader();
            var response = _client.GetAsync(apiUrl + "PatientWaitingLists/patient/" + id).Result;
            return response;
        }

        private HttpResponseMessage RemoveWaiting(int id)

        {
            AddHeader();
            var response = _client.DeleteAsync(apiUrl + "PatientWaitingLists/" + id).Result;
            return response;
        }

        private HttpResponseMessage GetAll()
        {
            AddHeader();
            var response = _client.GetAsync(apiUrl + "Patients").Result;
            return response;
        }

        private HttpResponseMessage GetById(int id)
        {
            AddHeader();
            var response = _client.GetAsync(apiUrl + "Patients/" + id).Result;
            return response;
        }

        private HttpResponseMessage GetByPps(string id)
        {
            AddHeader();
            var response = _client.GetAsync(apiUrl + "Patients/pps/" + id).Result;
            return response;
        }

        private HttpResponseMessage GetCheckIn(int id)
        {
            AddHeader();
            var response = _client.GetAsync(apiUrl + "PatientCheckIns/patient/" + id).Result;
            return response;
        }


        private HttpResponseMessage GetHospitalAdmitted(int id)
        {
            AddHeader();
            var response = _client.GetAsync(apiUrl + "PatientAdmitted/hospital/" + id).Result;
            return response;
        }

        private void AddHeader()
        {
            var token = HttpContext.User.Claims.Where(u => u.Type == "Token").FirstOrDefault().Value;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }


        private static DateTime GetNow()
        {
            return DateTime.Now;
        }

    }
}