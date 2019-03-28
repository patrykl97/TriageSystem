using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TriageSystem.Areas.Identity.Data;
using TriageSystem.Hubs;
using TriageSystem.Models;
using TriageSystem.ViewModels;

namespace TriageSystem.Controllers
{
    [Authorize] // User needs to be singed in to display this view
    public class PatientController : Controller
    {
        UserManager<TriageSystemUser> _userManager;
        private TriageSystemContext _context;
        private IHubContext<NotificationHub> HubContext { get; set; }

        public PatientController(UserManager<TriageSystemUser> userManager, TriageSystemContext context, IHubContext<NotificationHub> hubContext)
        {
            _userManager = userManager;
            _context = context;
            HubContext = hubContext;
        }

        public IActionResult Register()
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
                        var patients = _context.Patients.Where(p => p.PPS == patientData.PPS);
                        if (patients.Count() > 0)
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
                    var patientCheckIn = new PatientCheckIn { PatientId = patient.Id, HospitalID = patientData.HospitalID, Arrival = patientData.Arrival, Infections = patientData.Infections, Time_checked_in = patientData.Time_checked_in };
                    _context.PatientCheckIns.Add(patientCheckIn);
                    await _context.SaveChangesAsync();
                    //var connection = new HubConnectionBuilder().WithUrl("/NotificationHub").Build();
                    await HubContext.Clients.All.SendAsync("ReceiveNotification", patientData.HospitalID);
                    
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
            var user = _userManager.GetUserAsync(User).Result;
            var patientCheckedIn = user.Staff.Hospital.PatientCheckInList.Where(m => m.PatientId == id).FirstOrDefault();  //_context.PatientCheckIns.Where(m => m.PatientId == id).FirstOrDefault();
            var p = patientCheckedIn.Patient;
            var patientViewModel = new PatientCheckInViewModel {
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Actions(int id)
        {
            var user = _userManager.GetUserAsync(User).Result;
            var patientData = user.Staff.Hospital.PatientWaitingList.Where(m => m.PatientId == id).FirstOrDefault();  //_context.PatientWaitingList.Where(m => m.PatientId == id).FirstOrDefault();
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
                    var user = _userManager.GetUserAsync(User).Result;
                    var patient = user.Staff.Hospital.PatientWaitingList.Where(m => m.PatientId == id).FirstOrDefault();
                    var patientData = new PatientCheckIn { PatientId = patient.PatientId, Arrival = patient.Arrival, HospitalID = patient.HospitalID, Infections = patient.Infections, Time_checked_in = patient.Time_checked_in, Time_triaged = patient.Time_triaged };
                    _context.PatientCheckIns.Add(patientData);
                    _context.PatientWaitingList.Remove(patient);
                    await _context.SaveChangesAsync();
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
                    var user = _userManager.GetUserAsync(User).Result;
                    var patient = user.Staff.Hospital.PatientWaitingList.Where(m => m.PatientId == id).FirstOrDefault();
                    // TODO: after refactoring uncomment line below
                    //var patientData = new PatientCheckIn { PatientId = patient.PatientId, PPS = patient.PPS, Arrival = patient.Arrival, HospitalID = patient.HospitalID, Infections = patient.Infections, Time_checked_in = patient.Time_checked_in };
                    //var patientData = new PatientCheckIn { PatientId = patient.PatientId, HospitalID = patient.HospitalID, Time_checked_in = patient.Time_checked_in, Arrival = "Home" };
                    var patientData = new PatientCheckIn { PatientId = patient.PatientId, Arrival = patient.Arrival, HospitalID = patient.HospitalID, Infections = patient.Infections, Time_checked_in = patient.Time_checked_in, Time_triaged = patient.Time_triaged };
                    _context.PatientCheckIns.Add(patientData);
                    _context.PatientWaitingList.Remove(patient);
                    await _context.SaveChangesAsync();
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
            var user = _userManager.GetUserAsync(User).Result;
            var list = _context.PatientAdmitted.Where(p => p.HospitalID == user.Staff.HospitalID);
            return View(list);
        }


        private async Task<IActionResult> AddToAdmitted(int id, bool sentHome = false)
        {
            if (id > 0)
            {
                try
                {
                    var patientData = _context.PatientWaitingList.Where(m => m.PatientId == id).FirstOrDefault();
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

                    if(sentHome)
                    {
                        patientAdmitted.Time_released = GetNow();
                        patientAdmitted.FinalCondition = "Sent home";
                    } 
                    else
                    {
                        patientAdmitted.Time_admitted = GetNow();
                    }
                    _context.PatientAdmitted.Add(patientAdmitted);
                    _context.PatientWaitingList.Remove(patientData);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
            return null;
        }

   






        private static DateTime GetNow()
        {
            return DateTime.Now;
        }

    }
}