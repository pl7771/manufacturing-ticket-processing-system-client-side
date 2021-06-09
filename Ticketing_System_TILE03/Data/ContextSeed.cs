using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Ticketing_System_TILE03.Models;
using Ticketing_System_TILE03.Models.Domain;
using System;
using Ticketing_System_TILE03.Models.Repositories;

namespace Ticketing_System_TILE03.Data
{
    public static class ContextSeed
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            IQueryable<IdentityRole> roles = roleManager.Roles;
            if (roles.Count() == 0)
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Client.ToString()));
                await roleManager.CreateAsync(new IdentityRole(Roles.Technician.ToString()));
                await roleManager.CreateAsync(new IdentityRole(Roles.SupportManager.ToString()));
                await roleManager.CreateAsync(new IdentityRole(Roles.Administrator.ToString()));
            }
        }

        private static Company CreateCompany(ICompanyRepository companyRepository)
        {
            if(companyRepository.FindAllCompanies().ToList().Count() == 0)
            {
                Company company = new Company("Coca-cola", "FooBarStraat 4 9000 Gent", "0044556699")
                {
                    Contracts = CreateContractListAndSeedContractTypes()
                };
                companyRepository.Add(company);
                companyRepository.SaveChanges();
                return company;
            }
            return null;
        }

        public static async Task SeedClientAsync(UserManager<ApplicationUser> userManager, ITicketRepository ticketRepository, ICompanyRepository companyRepository)
        {
            //Seed client 
            var client = new ApplicationUser
            {
                UserName = "client@gmail.com",
                Email = "client@gmail.com",
                FirstName = "Cli",
                LastName = "Ent",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                Client = new Client()
                {
                    Company = CreateCompany(companyRepository)
                }
            };

            if (userManager.Users.All(u => u.Id != client.Id))
            {
                var user = await userManager.FindByEmailAsync(client.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(client, "client");
                    await userManager.AddToRoleAsync(client, Roles.Client.ToString());
                    IEnumerable<Ticket> ticketLijst = CreateTicketList(client.Client.Company);
                    IEnumerable<Contract> contractLijst = CreateContractListAndSeedContractTypes();
                    ticketRepository.AddAll(ticketLijst);
                    ticketRepository.SaveChanges();
                }
            }
        }

        public static async Task SeedTechnicianAsync(UserManager<ApplicationUser> userManager, IClientRepository clientRepository, ITicketRepository ticketRepository)
        {
            //Seed technician 
            var technician = new ApplicationUser
            {
                UserName = "technician@gmail.com",
                Email = "technician@gmail.com",
                FirstName = "Tech",
                LastName = "Nician",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                Employee = new Employee()
            };
            if (userManager.Users.All(u => u.Id != technician.Id))
            {
                var user = await userManager.FindByEmailAsync(technician.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(technician, "technician");
                    await userManager.AddToRoleAsync(technician, Roles.Technician.ToString());
                    await AddClientTicketsToTechnician(userManager, clientRepository, ticketRepository);
                }
            }
        }

        private static async Task AddClientTicketsToTechnician(UserManager<ApplicationUser> userManager, IClientRepository clientRepository, ITicketRepository ticketRepository)
        {
            var clientUser = await userManager.FindByEmailAsync("client@gmail.com");
            var technicianUser = await userManager.FindByEmailAsync("technician@gmail.com");
            Client client = clientRepository.GetByApplicationUserId(clientUser.Id);
            IEnumerable<Ticket> clientTickets = ticketRepository.GetActiveTicketsByCompany(client.Company);
            foreach (var ticket in clientTickets)
            {
                ticket.Employee = technicianUser.Employee;
            }
        }

        public static async Task SeedSupportManagerAsync(UserManager<ApplicationUser> userManager)
        {
            //Seed supportmanager 
            var supportmanager = new ApplicationUser
            {
                UserName = "supportmanager@gmail.com",
                Email = "supportmanager@gmail.com",
                FirstName = "Support",
                LastName = "Manager",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                Employee = new Employee()
            };
            if (userManager.Users.All(u => u.Id != supportmanager.Id))
            {
                var user = await userManager.FindByEmailAsync(supportmanager.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(supportmanager, "supportmanager");
                    await userManager.AddToRoleAsync(supportmanager, Roles.SupportManager.ToString());
                }
            }
        }

        public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager)
        {
            //Seed administrator 
            var admin = new ApplicationUser
            {
                UserName = "administrator@gmail.com",
                Email = "administrator@gmail.com",
                FirstName = "Admin",
                LastName = "Istrator",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                Employee = new Employee()
            };
            if (userManager.Users.All(u => u.Id != admin.Id))
            {
                var user = await userManager.FindByEmailAsync(admin.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(admin, "administrator");
                    await userManager.AddToRoleAsync(admin, Roles.Administrator.ToString());
                }
            }
        }

        public static IEnumerable<Ticket> CreateTicketList(Company company)
        {
            List<Contract> contracten = company.Contracts.ToList();

            Contract contract0 = contracten[0];
            Contract contract1 = contracten[1];
            Contract contract2 = contracten[2];
            Contract contract3 = contracten[3];

            Ticket ticket1 = new Ticket(DateTime.Now, "ticket 1", "dit is een omschrijving voor ticket 1", Status.AANGEMAAKT);
            ticket1.Company = company;
            ticket1.Contract = contract1;
            ticket1.DatumAangemaakt = DateTime.Now.AddDays(-54);
            Ticket ticket2 = new Ticket(DateTime.Now, "ticket 2", "dit is een omschrijving voor ticket 2", Status.AANGEMAAKT);
            ticket2.Company = company;
            ticket2.Contract = contract2;
            ticket2.DatumAangemaakt = DateTime.Now.AddDays(-92);
            Ticket ticket3 = new Ticket(DateTime.Now, "ticket 3", "dit is een omschrijving voor ticket 3", Status.AANGEMAAKT);
            ticket3.Company = company;
            ticket3.Contract = contract1;
            ticket3.DatumAangemaakt = DateTime.Now.AddDays(-121);
            Ticket ticket4 = new Ticket(DateTime.Now, "ticket 4", "dit is een omschrijving voor ticket 4", Status.AFGEHANDELD);
            ticket4.Company = company;
            ticket4.Contract = contract2;
            ticket4.DatumAangemaakt = DateTime.Now.AddDays(-151);
            ticket4.DatumAfgesloten = DateTime.Now.AddDays(-22);
            Ticket ticket5 = new Ticket(DateTime.Now, "ticket 5", "dit is een omschrijving voor ticket 5", Status.AFGEHANDELD);
            ticket5.Company = company;
            ticket5.Contract = contract0;
            ticket5.DatumAangemaakt = DateTime.Now.AddDays(-93);
            ticket5.DatumAfgesloten = DateTime.Now.AddDays(-70);
            Ticket ticket6 = new Ticket(DateTime.Now, "ticket 6", "dit is een omschrijving voor ticket 6", Status.GEANNULEERD);
            ticket6.Company = company;
            ticket6.Contract = contract0;
            ticket6.DatumAangemaakt = DateTime.Now.AddDays(-22);
            Ticket ticket7 = new Ticket(DateTime.Now, "ticket 7", "dit is een omschrijving voor ticket 7", Status.GEANNULEERD);
            ticket7.Company = company;
            ticket7.Contract = contract0;
            ticket7.DatumAangemaakt = DateTime.Now.AddDays(-2);
            Ticket ticket8 = new Ticket(DateTime.Now, "ticket 8", "dit is een omschrijving voor ticket 8", Status.IN_BEHANDELING);
            ticket8.Company = company;
            ticket8.Contract = contract0;
            ticket8.DatumAangemaakt = DateTime.Now.AddDays(-10);



            Ticket ticket41 = new Ticket(DateTime.Now, "ticket 41", "dit is een omschrijving voor ticket 41", Status.IN_BEHANDELING);
            ticket41.DatumAangemaakt = DateTime.Now.AddDays(-50);
            ticket41.DatumAfgesloten = DateTime.Now.AddDays(-20);
            ticket41.Company = company;
            ticket41.Contract = contract0;


            Ticket ticket42 = new Ticket(DateTime.Now, "ticket 42", "dit is een omschrijving voor ticket 42", Status.AFGEHANDELD);
            ticket42.DatumAangemaakt = DateTime.Now.AddDays(-30);
            ticket42.DatumAfgesloten = DateTime.Now.AddDays(-10);
            ticket42.Contract = contract1;

            Ticket ticket43 = new Ticket(DateTime.Now, "ticket 43", "dit is een omschrijving voor ticket 43", Status.AFGEHANDELD);
            ticket43.DatumAangemaakt = DateTime.Now.AddDays(-60);
            ticket43.DatumAfgesloten = DateTime.Now.AddDays(-50);
            ticket43.Contract = contract3;
            Ticket ticket44 = new Ticket(DateTime.Now, "ticket 44", "dit is een omschrijving voor ticket 44", Status.AFGEHANDELD);
            ticket44.DatumAangemaakt = DateTime.Now.AddDays(-17);
            ticket44.DatumAfgesloten = DateTime.Now.AddDays(-6);
            ticket44.Contract = contract3;
            Ticket ticket45 = new Ticket(DateTime.Now, "ticket 45", "dit is een omschrijving voor ticket 45", Status.AFGEHANDELD);
            ticket45.DatumAangemaakt = DateTime.Now.AddDays(-23);
            ticket45.DatumAfgesloten = DateTime.Now.AddDays(-20);
            ticket45.Contract = contract3;
            Ticket ticket46 = new Ticket(DateTime.Now, "ticket 46", "dit is een omschrijving voor ticket 46", Status.AFGEHANDELD);
            ticket46.DatumAangemaakt = DateTime.Now.AddDays(-23);
            ticket46.DatumAfgesloten = DateTime.Now.AddDays(-21);
            ticket46.Contract = contract2;
            Ticket ticket47 = new Ticket(DateTime.Now, "ticket 47", "dit is een omschrijving voor ticket 47", Status.AFGEHANDELD);
            ticket47.DatumAangemaakt = DateTime.Now.AddDays(-50);
            ticket47.DatumAfgesloten = DateTime.Now.AddDays(-13);
            ticket47.Contract = contract2;
            Ticket ticket48 = new Ticket(DateTime.Now, "ticket 48", "dit is een omschrijving voor ticket 48", Status.AFGEHANDELD);
            ticket48.DatumAangemaakt = DateTime.Now.AddDays(-33);
            ticket48.DatumAfgesloten = DateTime.Now.AddDays(-30);
            ticket48.Contract = contract2;
            Ticket ticket49 = new Ticket(DateTime.Now, "ticket 49", "dit is een omschrijving voor ticket 49", Status.AFGEHANDELD);
            ticket49.DatumAangemaakt = DateTime.Now.AddDays(-18);
            ticket49.DatumAfgesloten = DateTime.Now.AddDays(-15);
            ticket49.Contract = contract2;
            Ticket ticket50 = new Ticket(DateTime.Now, "ticket 50", "dit is een omschrijving voor ticket 50", Status.AFGEHANDELD);
            ticket50.DatumAangemaakt = DateTime.Now.AddDays(-15);
            ticket50.DatumAfgesloten = DateTime.Now.AddDays(-7);
            ticket50.Contract = contract1;
            Ticket ticket51 = new Ticket(DateTime.Now, "ticket 51", "dit is een omschrijving voor ticket 51", Status.AFGEHANDELD);
            ticket51.DatumAangemaakt = DateTime.Now.AddDays(-6);
            ticket51.DatumAfgesloten = DateTime.Now.AddDays(-2);
            ticket51.Contract = contract0;
            Ticket ticket52 = new Ticket(DateTime.Now, "ticket 52", "dit is een omschrijving voor ticket 52", Status.AFGEHANDELD);
            ticket52.DatumAangemaakt = DateTime.Now.AddDays(-6);
            ticket52.DatumAfgesloten = DateTime.Now.AddDays(-1);
            ticket52.Contract = contract0;
            Ticket ticket53 = new Ticket(DateTime.Now, "ticket 53", "dit is een omschrijving voor ticket 53", Status.AFGEHANDELD);
            ticket53.DatumAangemaakt = DateTime.Now.AddDays(-12);
            ticket53.DatumAfgesloten = DateTime.Now.AddDays(-9);
            ticket53.Contract = contract0;
            Ticket ticket54 = new Ticket(DateTime.Now, "ticket 54", "dit is een omschrijving voor ticket 54", Status.AFGEHANDELD);
            ticket54.DatumAangemaakt = DateTime.Now.AddDays(-60);
            ticket54.DatumAfgesloten = DateTime.Now.AddDays(-52);
            ticket54.Contract = contract0;
            Ticket ticket55 = new Ticket(DateTime.Now, "ticket 55", "dit is een omschrijving voor ticket 55", Status.AFGEHANDELD);
            ticket55.DatumAangemaakt = DateTime.Now.AddDays(-40);
            ticket55.DatumAfgesloten = DateTime.Now.AddDays(-33);
            ticket55.Contract = contract0;
            Ticket ticket56 = new Ticket(DateTime.Now, "ticket 56", "dit is een omschrijving voor ticket 56", Status.AFGEHANDELD);
            ticket56.DatumAangemaakt = DateTime.Now.AddDays(-20);
            ticket56.DatumAfgesloten = DateTime.Now.AddDays(-18);
            ticket56.Contract = contract0;
            Ticket ticket57 = new Ticket(DateTime.Now, "ticket 57", "dit is een omschrijving voor ticket 57", Status.AFGEHANDELD);
            ticket57.DatumAangemaakt = DateTime.Now.AddDays(-40);
            ticket57.DatumAfgesloten = DateTime.Now.AddDays(-33);
            ticket57.Contract = contract0;
            Ticket ticket58 = new Ticket(DateTime.Now, "ticket 58", "dit is een omschrijving voor ticket 58", Status.AFGEHANDELD);
            ticket58.DatumAangemaakt = DateTime.Now.AddDays(-170);
            ticket58.DatumAfgesloten = DateTime.Now.AddDays(-163);
            ticket58.Contract = contract0;



            return new Ticket[] { ticket1, ticket2, ticket3, ticket4, ticket5, ticket6, ticket7, ticket8,
                ticket41, ticket42, ticket43, ticket44, ticket45, ticket46, ticket47, ticket48, ticket49, ticket50,
                ticket51, ticket52, ticket53, ticket54, ticket55, ticket56, ticket57, ticket58 };
        }
    

        public static ICollection<Contract> CreateContractListAndSeedContractTypes()
        {
            ContractType best = new ContractType("SLA1", ManierAanmakenTicket.APPLICATIE, GedekteTijdstippen.ALTIJD,
                    new TimeSpan(1, 0, 0).TotalHours, TimeSpan.Zero.TotalHours, 1000);
            ContractType worst = new ContractType("SLA2", ManierAanmakenTicket.TELEFOON, GedekteTijdstippen.ENKELWEEKDAGEN,
                new TimeSpan(7, 0, 0, 0).TotalHours, new TimeSpan(2, 0, 0, 0).TotalHours, 5);
            ContractType normal = new ContractType("SLA3", ManierAanmakenTicket.EMAIL, GedekteTijdstippen.ENKELWEEKDAGEN,
                new TimeSpan(30, 0, 0, 0).TotalHours, new TimeSpan(3, 0, 0).TotalHours, 100);
            ContractType oudContract = new ContractType("SLA4", ManierAanmakenTicket.EMAIL, GedekteTijdstippen.ENKELWEEKDAGEN,
                new TimeSpan(60, 0, 0, 0).TotalHours, new TimeSpan(1, 0, 0).TotalHours, 100)
            { Status = ContractTypeStatus.NIET_ACTIEF };
            ContractType oudContract2 = new ContractType("SLA5", ManierAanmakenTicket.EMAIL, GedekteTijdstippen.ENKELWEEKDAGEN,
                new TimeSpan(90, 0, 0, 0).TotalHours, new TimeSpan(1, 0, 0).TotalHours, 100)
            { Status = ContractTypeStatus.NIET_ACTIEF };

            DateTime startdatum1 = new DateTime(2020, 12, 31, 15, 30, 30);
            DateTime einddatum1 = startdatum1.AddDays(30);

            Contract contract1 = new Contract(best, ContractStatus.LOPEND, startdatum1, einddatum1);
            Contract contract2 = new Contract(worst, ContractStatus.LOPEND, startdatum1.AddDays(2), einddatum1.AddDays(2));
            Contract contract3 = new Contract(normal, ContractStatus.IN_BEHANDELING, startdatum1.AddDays(6), einddatum1.AddDays(6));
            Contract contract4 = new Contract(oudContract, ContractStatus.BEEINDIGD, startdatum1.AddDays(-50), einddatum1.AddDays(-50));
            Contract contract5 = new Contract(oudContract2, ContractStatus.BEEINDIGD, startdatum1.AddDays(-70), einddatum1.AddDays(-70));
            return new Contract[] { contract1, contract2, contract3, contract4, contract5 };

        }

        /*public static void SeedContractTypes(IContractTypeRepository contractTypeRepository)
        {
            ContractType best = new ContractType("MostExpensiveContractEver", ManierAanmakenTicket.APPLICATIE, GedekteTijdstippen.ALTIJD,
                    new TimeSpan(1, 0, 0).TotalHours, TimeSpan.MinValue.TotalHours, 1000);
            ContractType worst = new ContractType("WorstContractEver", ManierAanmakenTicket.TELEFOON, GedekteTijdstippen.ENKELWEEKDAGEN,
                new TimeSpan(7, 0, 0, 0).TotalHours, new TimeSpan(2, 0, 0, 0).TotalHours, 5);
            ContractType normal = new ContractType("MostNormalContractEver", ManierAanmakenTicket.EMAIL, GedekteTijdstippen.ENKELWEEKDAGEN,
                new TimeSpan(1, 0, 0, 0).TotalHours, new TimeSpan(3, 0, 0).TotalHours, 100);
            ContractType oudContract = new ContractType("OldestContractEver", ManierAanmakenTicket.EMAIL, GedekteTijdstippen.ENKELWEEKDAGEN,
                new TimeSpan(4, 0, 0, 0).TotalHours, new TimeSpan(1, 0, 0).TotalHours, 100)
            { Status = ContractTypeStatus.NIET_ACTIEF };
            ContractType oudContract2 = new ContractType("OldestContract2", ManierAanmakenTicket.EMAIL, GedekteTijdstippen.ENKELWEEKDAGEN,
                new TimeSpan(4, 0, 0, 0).TotalHours, new TimeSpan(1, 0, 0).TotalHours, 100)
            { Status = ContractTypeStatus.NIET_ACTIEF };

            ContractType[] contractTypes = new ContractType[] { best, worst, normal, oudContract, oudContract2};
            contractTypeRepository.AddAll(contractTypes);
            contractTypeRepository.SaveChanges();

        }*/

    }
}
