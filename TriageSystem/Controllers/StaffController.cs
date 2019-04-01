using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TriageSystem.Models;

namespace TriageSystem.Controllers
{

    [Authorize]
    public class StaffController : Controller
    {

        private readonly HttpClient _client = new HttpClient();
        private readonly string apiUrl;


        public StaffController(IOptions<ApiSettings> apiSettings)
        {
            apiUrl = apiSettings.Value.ApiConnection;
        }

        public IActionResult Index()
        {
            var hospitalID = getHospitalID();
            var response = GetStaff(hospitalID);
            if(response.IsSuccessStatusCode)
            {
                var staffList = JsonConvert.DeserializeObject<IEnumerable<Staff>>(response.Content.ReadAsStringAsync().Result);
                return View(staffList);
            }
            return NotFound();
        }

        
        public IActionResult Details(int id)
        {
            if (id > 0)
            {
                var response = GetStaffById(id);
                if(response.IsSuccessStatusCode)
                {
                    var staff = JsonConvert.DeserializeObject<Staff>(response.Content.ReadAsStringAsync().Result);
                    if(staff != null)
                    {
                        return View(staff);
                    }
                }
            }
            return NotFound();
        }

 
        public IActionResult Create()
        {
            var hospitalID = getHospitalID();
            var staff = new Staff { HospitalID = hospitalID };
            return View(staff);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Staff staff)
        {
            if (ModelState.IsValid)
            {
                var response = AddStaff(staff);
                if(response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(staff);
        }


        public ActionResult Edit(int id)
        {
            if (id > 0)
            {
                var response = GetStaffById(id);
                if(response.IsSuccessStatusCode)
                {
                    var staff = JsonConvert.DeserializeObject<Staff>(response.Content.ReadAsStringAsync().Result);
                    return View(staff);
                }
            }
            return NotFound();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Staff staff)
        {
            if (id != staff.StaffID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var response = await UpdateStaff(id, staff);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StaffExists(staff.StaffID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(staff);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id > 0 )
            {
                var response = await DeleteStaff(id);
                if(response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return NotFound();

        }

        //// POST: Staff/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var staff = await _context.Staff.FindAsync(id);
        //    _context.Staff.Remove(staff);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private HttpResponseMessage GetStaff(int id)
        {
            AddHeader();
            var response = _client.GetAsync(apiUrl + "Staff/hospital/" + id).Result;
            return response;
        }

        private HttpResponseMessage GetStaffById(int id)
        {
            AddHeader();
            var response = _client.GetAsync(apiUrl + "Staff/" + id).Result;
            return response;
        }

        private HttpResponseMessage AddStaff(Staff staff)
        {
            AddHeader();
            var response = _client.PostAsJsonAsync(apiUrl + "Staff", staff).Result;
            return response;
        }

        private async Task<HttpResponseMessage> UpdateStaff(int id, Staff staff)
        {
            AddHeader();
            var response = await _client.PutAsJsonAsync(apiUrl + "Staff/" + id, staff);
            return response;
        }

        private async Task<HttpResponseMessage> DeleteStaff(int id)
        {
            AddHeader();
            var response = await _client.DeleteAsync(apiUrl + "Staff/" + id);
            return response;
        }

        private int getHospitalID()
        {
            var h = User.Claims.Where(u => u.Type == "HospitalID").FirstOrDefault().Value;
            int hospitalID = 0;
            Int32.TryParse(h, out hospitalID);
            return hospitalID;
        }



        private bool StaffExists(int id)
        {
            var response = GetStaffById(id);
            if(response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }


        private void AddHeader()
        {
            var token = HttpContext.User.Claims.Where(u => u.Type == "Token").FirstOrDefault().Value;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
