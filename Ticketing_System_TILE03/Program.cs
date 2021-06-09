using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticketing_System_TILE03.Data;
using Ticketing_System_TILE03.Models;
using Ticketing_System_TILE03.Models.Domain;
using Ticketing_System_TILE03.Models.Repositories;
using Ticketing_System_TILE03.Models.Services;

namespace Ticketing_System_TILE03
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    var ticketRepository = services.GetRequiredService<ITicketRepository>();
                    var contractTypeRepository = services.GetRequiredService<IContractTypeRepository>();
                    var companyRepository = services.GetRequiredService<ICompanyRepository>();
                    var clientRepository = services.GetRequiredService<IClientRepository>();
                    await ContextSeed.SeedRolesAsync(roleManager);
                    await ContextSeed.SeedClientAsync(userManager, ticketRepository, companyRepository);
                    await ContextSeed.SeedTechnicianAsync(userManager, clientRepository, ticketRepository);
                    await ContextSeed.SeedSupportManagerAsync(userManager);
                    await ContextSeed.SeedAdminAsync(userManager);
                    //ContextSeed.SeedContractTypes(contractTypeRepository);
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
