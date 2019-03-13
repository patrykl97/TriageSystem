using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            var patientData = new PatientCheckIn { HospitalID = user.Staff.HospitalID};
            var patientList = _context.Patients.Select(p => p.PPS).ToList();
            var selectList = patientList.Select(p => new SelectListItem { Text = p, Value = p });
            //selectList.Add(0, new SelectListItem { Text = "Please Select...", Value = string.Empty });
            ViewBag.PPS = selectList;
            return View(patientData);
        }



        [HttpPost]
        public async Task<IActionResult> RegisterPatient([FromBody] PatientCheckIn patientData)
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
                    throw;
                }
                return Json("Success");
            }
            return Json(getErrors());


        }

        public IActionResult TriageAssessment()
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
                var patientData = new PatientWaitingList { PPS = patientCheckedIn.PPS, Patient = patientCheckedIn.Patient, HospitalID = patientCheckedIn.HospitalID, Condition = patientCheckedIn.Condition };
                string name, path;
                string[] filePaths = Directory.GetFiles(@"./Flowcharts");
                List<string> flowchartNames = new List<string>();
                for (int i = 0; i < filePaths.Length; ++i)
                {
                    path = filePaths[i];
                    name = Path.GetFileName(path);
                    name = name.Replace("_", " ");
                    name = name.Replace(".json", "");
                    flowchartNames.Add(name);
                }
                ViewBag.FlowchartNames = flowchartNames.Select(f => new SelectListItem { Text = f, Value = f });
                return View(patientData);
            }

        }


        [HttpPost]
        public async Task<IActionResult> TriageAssessment([FromBody] PatientWaitingList patientData)
        {


            if (ModelState.IsValid)
            {
                try
                {
                    patientData.Time_checked_in = GetNow();
                    //patientData.HospitalID = HospitalID;
                    _context.PatientWaitingList.Add(patientData);
                    _context.PatientCheckIns.Remove(_context.PatientCheckIns.Where(p => p.PPS == patientData.PPS).First());
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

        private string getErrors()
        {
            string errors = "";
            foreach (var value in ModelState.Values)
            {
                foreach (var error in value.Errors)
                {
                    errors += error.ErrorMessage;
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

        //public IActionResult Register(RegisterModel model)
        //{
        //    return View(model);
        //}

        //public PartialViewResult ShowError(String sErrorMessage)
        //{
        //    return PartialView("_Error");
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
