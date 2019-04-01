using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using TodoListWebApp;
using TodoListWebApp.Utils;
using TriageSystem.Models;
using TriageSystem.ViewModels;



/// <summary>
/// REMEMBER!!! ALWAYS USE HTTPS FOR API CONNECTION
/// </summary>

namespace TriageSystem.Controllers
{
    //[Route("[controller]/[action]")]
    public class AccountController : Controller
    {

        private IMapper _mapper;
        private readonly string apiUrl;


        public AccountController(IMapper mapper, IOptions<ApiSettings> apiSettings)
        {
            _mapper = mapper;
            apiUrl = apiSettings.Value.ApiConnection;
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignInUser(LoginViewModel userIn)
        {
            var authenticated = await Authenticate(userIn);
            if(authenticated)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            return RedirectToAction(nameof(Login));
        }

        private async Task<bool> Authenticate(LoginViewModel userIn)
        {
            var user = _mapper.Map<User>(userIn);
            var client = new HttpClient();
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(
            //    new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            //client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
            var response = client.PostAsJsonAsync(apiUrl + "account/authenticate", user).Result;
            if (response.IsSuccessStatusCode)
            {
                var x = JsonConvert.DeserializeObject<UserSession>(response.Content.ReadAsStringAsync().Result);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userIn.Email),
                    new Claim("HospitalID", x.HospitalID.ToString()),
                    new Claim("Token", x.Token)
                };

                //var claimsIdentity = new ClaimsIdentity(claims, "login");

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties { };

                ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(principal);
                return true;
            }
            return false;
        }

        public IActionResult Register()
        {
            var model = new RegisterViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel userIn) 
        {
            var user = _mapper.Map<User>(userIn);
            var client = new HttpClient();
            var response = client.PostAsJsonAsync(apiUrl + "account/register", user).Result;
            if (response.IsSuccessStatusCode)
            {
                //var y = response.Content.ReadAsStringAsync().Result;
                //var x = JsonConvert.DeserializeObject<UserSession>(response.Content.ReadAsStringAsync().Result);
                //var claims = new List<Claim>
                //{
                //    new Claim(ClaimTypes.Name, userIn.Email),
                //    new Claim("HospitalID", x.HospitalID.ToString()),
                //    new Claim("Token", x.Token)
                //};

                ////var claimsIdentity = new ClaimsIdentity(claims, "login");

                //var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                //var authProperties = new AuthenticationProperties { };

                //ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity);
                //await HttpContext.SignInAsync(principal);
                var model = new LoginViewModel { Email = userIn.Email, Password = userIn.Password};
                var authenticated = await Authenticate(model);
                if(authenticated)
                {
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }

            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            // Remove all cache entries for this user and send an OpenID Connect sign-out request.
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult SignedOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Redirect to home page if the user is authenticated.
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }



        public HttpClient CreateClient(string token = "")
        {
            var client = new HttpClient();
            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            return client;
        }
    }
}