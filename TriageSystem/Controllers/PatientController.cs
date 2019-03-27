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
        private OnConfiguring _context;
        private IHubContext<NotificationHub> HubContext { get; set; }

        public PatientController(UserManager<TriageSystemUser> userManager, OnConfiguring context, IHubContext<NotificationHub> hubContext)
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
        public IActionResult CheckedIn(int id)
        {
            var patientCheckedIn = _context.PatientCheckIns.Where(m => m.PatientId == id).FirstOrDefault();
            var p = _context.Patients.Where(m => m.Id == id).FirstOrDefault();
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
        public IActionResult Actions(int id)
        {
            var patientData = _context.PatientWaitingList.Where(m => m.PatientId == id).FirstOrDefault();
            var p = _context.Patients.Where(m => m.Id == id).FirstOrDefault();

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

        [HttpPost]
        public IActionResult Admit(int id)
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        public IActionResult SendHome(int id)
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");

        }



        private static DateTime GetNow()
        {
            return DateTime.Now;
        }

    }
}