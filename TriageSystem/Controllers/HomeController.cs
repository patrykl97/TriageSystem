using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TriageSystem.Areas.Identity.Data;
using TriageSystem.Models;

namespace TriageSystem.Controllers
{
    [Authorize] // User needs to be singed in to display this view
    public class HomeController : Controller
    {
        UserManager<TriageSystemUser> _userManager;
        private readonly OnConfiguring _context;

        public HomeController(UserManager<TriageSystemUser> userManager, OnConfiguring context)
        {
            _userManager = userManager;
            _context = context;
        }

        public ActionResult Index()
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:53353/ChatHub")
             .Build();

            var user = _userManager.GetUserAsync(User).Result;
            user.Staff.Hospital.PatientCheckInList.OrderBy(t => t.Time_checked_in);
            var orderedList = user.Staff.Hospital.PatientWaitingList.OrderBy(p => (int)(p.Priority)).ThenBy(t => t.Time_checked_in).ToList(); // orders by priority, then by time checked in
            user.Staff.Hospital.PatientWaitingList = orderedList;
            return View(user);
        }

        public IActionResult RegisterPatient()
        {
            var user = _userManager.GetUserAsync(User).Result;
            var patientData = new PatientCheckIn { HospitalID = user.Staff.HospitalID};
            var patientList = _context.Patients.Select(p => p.PPS).ToList();
            var selectList = new SelectList(patientList);
            ViewBag.PPS = selectList;
            return View(patientData);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterPatient([Bind("PPS,Condition,Priority,HospitalID")] PatientCheckIn patientData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    patientData.Time_checked_in = GetNow();
                    _context.PatientCheckIns.Add(patientData);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    //if (!PatientWaitingListExists(staff.StaffID))
                    //{
                    //    return NotFound();
                    //}
                    //else
                    //{
                    throw;
                    //}
                }
                return RedirectToAction(nameof(Index));
            }
            return View(patientData);
        
        }

        public IActionResult TriageAssessment()
        {
            var user = _userManager.GetUserAsync(User).Result;
            var patientCheckedIn = user.Staff.Hospital.PatientCheckInList.First();
            var patientData = new PatientWaitingList { PPS = patientCheckedIn.PPS, Patient = patientCheckedIn.Patient, HospitalID = patientCheckedIn.HospitalID, Condition = patientCheckedIn.Condition };
            return View(patientData);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TriageAssessment(int HospitalID, [Bind("PPS,Condition,Priority")] PatientWaitingList patientData)
        {


            if (ModelState.IsValid)
            {
                try
                {
                    patientData.Time_checked_in = GetNow();
                    patientData.HospitalID = HospitalID;
                    _context.PatientWaitingList.Add(patientData);
                    _context.PatientCheckIns.Remove(_context.PatientCheckIns.Where(p => p.PPS == patientData.PPS).First());
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    //if (!PatientWaitingListExists(staff.StaffID))
                    //{
                    //    return NotFound();
                    //}
                    //else
                    //{
                        throw;
                    //}
                }
                return RedirectToAction(nameof(Index));
            }
            return View(patientData);
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

        //public IActionResult Register(RegisterModel model)
        //{
        //    return View(model);
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
