using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ticketing_System_TILE03.Data;
using Ticketing_System_TILE03.Models;

[assembly: HostingStartup(typeof(Ticketing_System_TILE03.Areas.Identity.IdentityHostingStartup))]
namespace Ticketing_System_TILE03.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}