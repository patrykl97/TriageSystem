﻿using System;
using System.Collections.Generic;
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

namespace TriageSystem.Controllers
{
    [Authorize] // User needs to be singed in to display this view
    public class TriageController : Controller
    {
        UserManager<TriageSystemUser> _userManager;
        private readonly OnConfiguring _context;
        private IHubContext<NotificationHub> HubContext { get; set; }

        public TriageController(UserManager<TriageSystemUser> userManager, OnConfiguring context, IHubContext<NotificationHub> hubContext)
        {
            _userManager = userManager;
            _context = context;
            HubContext = hubContext;
        }

        public IActionResult SelectFlowcharts()
        {
            var user = _userManager.GetUserAsync(User).Result;
            if (user.Staff.Hospital.PatientCheckInList.Count == 0)
            {
                TempData["Error"] = "No patients awaiting triage assessment!";
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else
            {
                var patientCheckedIn = user.Staff.Hospital.PatientCheckInList.First();
                var patientData = new PatientWaitingList { PatientId = patientCheckedIn.PatientId, PPS = patientCheckedIn.PPS, Patient = patientCheckedIn.Patient, HospitalID = patientCheckedIn.HospitalID };
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


        // TODO: refactor to use PatientId rather than pps
        public IActionResult Assessment(PatientWaitingList patient)
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
                    patientData.Time_checked_in = GetNow();
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

        private static DateTime GetNow()
        {
            return DateTime.Now;
        }


    }
}