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
using TriageSystem.Models;
using TriageSystem.ViewModels;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;

namespace TriageSystem.Controllers
{
    [Authorize] // User needs to be singed in to display this view
    public class HomeController : Controller
    {
        //UserManager<TriageSystemUser> _userManager;
        //private IConfiguration Configuration { get; set; }
        //private IHubContext<NotificationHub> HubContext { get; set; }
        private readonly HttpClient _client = new HttpClient();

        public HomeController()
        {
            //Configuration = configuration;
            //_userManager = userManager;
            //_context = context;
  

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
            var h = User.Claims.Where(u => u.Type == "HospitalID").FirstOrDefault().Value;
            int hospitalID = 0;
            Int32.TryParse(h, out hospitalID);
            //var client = new HttpClient();
            var response = GetById(hospitalID);

            if (response.IsSuccessStatusCode)
            {
                var r = response.Content.ReadAsStringAsync().Result;
                var hospital = JsonConvert.DeserializeObject<Hospital>(response.Content.ReadAsStringAsync().Result);

                var orderedCheckIns = hospital.PatientCheckInList.OrderBy(t => t.Time_checked_in);
                hospital.PatientCheckInList = orderedCheckIns.ToList();
                var orderedList = hospital.PatientWaitingList.OrderBy(p => (int)(p.Priority)).ThenBy(t => t.Time_checked_in).ToList(); // orders by priority, then by time checked in
                orderedList = setDuration(orderedList);
                hospital.PatientWaitingList = orderedList;
                return PartialView(hospital);
            }
            return null;

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

        private HttpResponseMessage GetById(int id)
        {
            var token = HttpContext.User.Claims.Where(u => u.Type == "Token").FirstOrDefault().Value;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = _client.GetAsync("https://localhost:44342/api/Hospital/" + id).Result;
            return response;
        }
    }
}
