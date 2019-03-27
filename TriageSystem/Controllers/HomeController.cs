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
            orderedList = setDuration(orderedList);
            user.Staff.Hospital.PatientWaitingList = orderedList;


            return PartialView(user);
        }

        [HttpPost]
        public async Task<IActionResult> PostAjax(int id)
        {
            if (id > 0)
            {
                try {
                    var patient = _context.PatientWaitingList.Where(p => p.PatientId == id).FirstOrDefault();
                    // TODO: after refactoring uncomment line below
                    //var patientData = new PatientCheckIn { PatientId = patient.PatientId, PPS = patient.PPS, Arrival = patient.Arrival, HospitalID = patient.HospitalID, Infections = patient.Infections, Time_checked_in = patient.Time_checked_in };
                    var patientData = new PatientCheckIn { PatientId = patient.PatientId, PPS = patient.PPS, HospitalID = patient.HospitalID, Time_checked_in = patient.Time_checked_in, Arrival = "Home" };
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
        public async Task<IActionResult> Post(int id)
        {
            int i = id;
            //Int32.TryParse(id, out i);
            if (i > 0)
            {
                try
                {
                    var patient = _context.PatientWaitingList.Where(p => p.PatientId == i).FirstOrDefault();
                    // TODO: after refactoring uncomment line below
                    //var patientData = new PatientCheckIn { PatientId = patient.PatientId, PPS = patient.PPS, Arrival = patient.Arrival, HospitalID = patient.HospitalID, Infections = patient.Infections, Time_checked_in = patient.Time_checked_in };
                    var patientData = new PatientCheckIn { PatientId = patient.PatientId, PPS = patient.PPS, HospitalID = patient.HospitalID, Time_checked_in = patient.Time_checked_in, Arrival = "Home" };
                    _context.PatientCheckIns.Add(patientData);
                    _context.PatientWaitingList.Remove(patient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
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

        private List<PatientWaitingList> setDuration(List<PatientWaitingList> orderedList)
        {
            foreach (var item in orderedList)
            {
                switch (item.Priority)
                {
                    case Priority.Red:
                        item.Duration = 0;
                        break;
                    case Priority.Orange:
                        item.Duration = 10;
                        break;
                    case Priority.Yellow:
                        item.Duration = 60;
                        break;
                    case Priority.Green:
                        item.Duration = 120;
                        break;
                    case Priority.Blue:
                        item.Duration = 240;
                        break;
                }
            }
            return orderedList;

        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
