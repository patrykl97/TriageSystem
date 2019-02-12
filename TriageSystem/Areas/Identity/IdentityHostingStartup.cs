//using System;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.UI;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using TriageSystem.Areas.Identity.Data;
//using TriageSystem.Models;
//using TriageSystemAPI.Models;

//[assembly: HostingStartup(typeof(TriageSystem.Areas.Identity.IdentityHostingStartup))]
//namespace TriageSystem.Areas.Identity
//{
//    public class IdentityHostingStartup : IHostingStartup
//    {
//        public void Configure(IWebHostBuilder builder)
//        {
//            builder.ConfigureServices((context, services) => {
//                services.AddDbContext<TriageSystemContext>(options =>
//                    options.UseSqlServer(
//                        context.Configuration.GetConnectionString("DefaultConnection")));

//                services.AddDefaultIdentity<TriageSystemUser>()
//                    .AddEntityFrameworkStores<TriageSystemContext>();
//            });
//        }
//    }
//}