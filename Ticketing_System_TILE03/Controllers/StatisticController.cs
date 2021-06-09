using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticketing_System_TILE03.Models;
using Ticketing_System_TILE03.Models.Domain;

namespace Ticketing_System_TILE03.Controllers
{
    [Authorize(Roles = "Client,Technician,SupportManager,Administrator")]
    public class StatisticController : Controller
    {

        private readonly IContractRepository _contractRepository;
        private readonly IContractTypeRepository _contractTypeRepository;
        private readonly IClientRepository _clientRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public StatisticController(
            IContractRepository contractRepository,
            IContractTypeRepository contractTypeRepository,
            IClientRepository clientRepository,
            ITicketRepository ticketRepository, UserManager<ApplicationUser> userManager)
        {
            _contractRepository = contractRepository;
            _contractTypeRepository = contractTypeRepository;
            _clientRepository = clientRepository;
            _ticketRepository = ticketRepository;
            _userManager = userManager;
        }

        #region TicketStatistics
        public async Task<ViewResult> Index() //alle tickets op Status 
        {


            var applicationUser = await _userManager.GetUserAsync(User);
            var rolesForUser = await _userManager.GetRolesAsync(applicationUser);
            IEnumerable<Ticket> tickets = new List<Ticket>();

            if (rolesForUser.Contains(Roles.Client.ToString()))
            {
                Client client = _clientRepository.GetByApplicationUserId(applicationUser.Id);
                tickets = _ticketRepository.GetAllTicketsByCompany(client.Company);
            }

            else
            {
                tickets = _ticketRepository.GetAll();
            }

            List<int> repartitions = new List<int>();
            var statussen = tickets.Select(e => e.Status.ToString()).Distinct();
            foreach (var item in statussen)
            {
                repartitions.Add(tickets.Count(e => e.Status.ToString() == item));
            }

            var rep = repartitions;
            ViewBag.STATE = statussen;
            ViewBag.REP = repartitions.ToList();
            return View();
        }

        public async Task<ViewResult> TicketType() //Tickets per prioriteitsType
        {
            var applicationUser = await _userManager.GetUserAsync(User);
            var rolesForUser = await _userManager.GetRolesAsync(applicationUser);
            IEnumerable<Ticket> tickets = new List<Ticket>();

            if (rolesForUser.Contains(Roles.Client.ToString()))
            {
                Client client = _clientRepository.GetByApplicationUserId(applicationUser.Id);
                tickets = _ticketRepository.GetAllTicketsByCompany(client.Company).Where(t => t.Contract != null).ToList();
            }

            else
            {
                tickets = _ticketRepository.GetAll().Where(t => t.Contract != null).ToList();
            }

            List<int> repartitions = new List<int>();

            //Verschillende contracttypes ophalen en in iEnumerable plaatsen
            var contractTypes = tickets.OrderBy(e => e.Contract.Type.Naam).Select(e => e.Contract.Type.Naam).Distinct();
            foreach (string type in contractTypes)
            {
                repartitions.Add(tickets.Count(e => e.Contract.Type.Naam == type));
            }

            var rep = repartitions;
            ViewBag.CONTRACTTYPES = contractTypes;
            ViewBag.REP = repartitions.ToList();
            return View();
        }

        public async Task<ViewResult> aantalafgeslotenticketpermaand()
        {

            var applicationuser = await _userManager.GetUserAsync(User);
            var rolesforuser = await _userManager.GetRolesAsync(applicationuser);
            IEnumerable<Ticket> tickets = new List<Ticket>();

            if (rolesforuser.Contains(Roles.Client.ToString()))
            {
                Client client = _clientRepository.GetByApplicationUserId(applicationuser.Id);
                tickets = _ticketRepository.GetAllClosedTicketsByCompanyFromPast6Months(client.Company);
            }

            else
            {
                tickets = _ticketRepository.GetAllClosedTicketsFromPast6Months();
            }

            //vul map op ContractType (string), List<Ticket> 
            var ticketMap = new Dictionary<string, List<Ticket>>();
            var maptickets = tickets.OrderBy(e => e.Contract.Type.Naam).Distinct();

            foreach (var ticket in maptickets)
            {
                if (!ticketMap.ContainsKey(ticket.Contract.Type.Naam))
                    ticketMap.Add(ticket.Contract.Type.Naam, maptickets.Where(t => t.Contract.Type.Naam == ticket.Contract.Type.Naam).ToList());

            }

            var keys = ticketMap.Keys.ToList();

            //Array van List<int>
            List<int>[] repartitionsPerMaand = new List<int>[keys.Count];

            for (var i = 0; i < keys.Count; i++)
            {

                List<Ticket> lijst = ticketMap.ElementAt(i).Value;
                repartitionsPerMaand[i] = CountCompletedTicketsPerMonth(lijst);
            }

            ViewBag.KEYS = keys;
            ViewBag.REPARTITIONSPERMAAND = repartitionsPerMaand;
            ViewBag.MAANDEN = getLabelsPastSixMonths();


            return View();
        }

        public async Task<ViewResult> AantalAangemaakteTicketsPerMaand()
        {

            var applicationUser = await _userManager.GetUserAsync(User);
            var rolesForUser = await _userManager.GetRolesAsync(applicationUser);
            IEnumerable<Ticket> tickets = new List<Ticket>();

            if (rolesForUser.Contains(Roles.Client.ToString()))
            {
                Client client = _clientRepository.GetByApplicationUserId(applicationUser.Id);
                tickets = _ticketRepository.GetAllNewTicketsByCompanyFromPast6Months(client.Company);
            }

            else
            {
                tickets = _ticketRepository.GetAllNewTicketsFromPast6Months();
            }

            //vul map op ContractType (string), List<Ticket> 
            var ticketMap = new Dictionary<string, List<Ticket>>();
            var maptickets = tickets.OrderBy(e => e.Contract.Type.Naam).Distinct();

            foreach (var ticket in maptickets)
            {
                if (!ticketMap.ContainsKey(ticket.Contract.Type.Naam))
                    ticketMap.Add(ticket.Contract.Type.Naam, maptickets.Where(t => t.Contract.Type.Naam == ticket.Contract.Type.Naam).ToList());
            }

            var keys = ticketMap.Keys.ToList();

            //Array van List<int>
            List<int>[] repartitionsPerMaand = new List<int>[keys.Count];

            for (var i = 0; i < keys.Count; i++)
            {

                List<Ticket> lijst = ticketMap.ElementAt(i).Value;
                repartitionsPerMaand[i] = CountNewTicketsPerMonth(lijst);
            }

            List<string> maanden = getLabelsPastSixMonths();

            ViewBag.KEYS = keys;
            ViewBag.REPARTITIONSPERMAAND = repartitionsPerMaand;
            ViewBag.MAANDEN = getLabelsPastSixMonths(); 

            return View();
        }
        #endregion
        #region ContractStatistics
        public async Task<ViewResult> ContractType() //Aantal afgesloten contracten per type voor ingelogde klant
        {
            var applicationUser = await _userManager.GetUserAsync(User);
            var rolesForUser = await _userManager.GetRolesAsync(applicationUser);
            IEnumerable<Contract> contracts = new List<Contract>();

            if (rolesForUser.Contains(Roles.Client.ToString()))
            {
                Client client = _clientRepository.GetByApplicationUserId(applicationUser.Id);
                contracts = _contractRepository.GetAllContractsOfCompany(client.Company);
            }

            else
            {
                contracts = _contractRepository.GetAll();

            }

            List<int> repartitions = new List<int>();
            var types = contracts.Select(e => e.Status.ToString()).Distinct();
            foreach (var item in types)
            {
                repartitions.Add(contracts.Count(e => e.Status.ToString() == item));
            }

            var rep = repartitions;
            ViewBag.CONTRACTTYPES = types;
            ViewBag.REP = repartitions.ToList();
            return View();
        }
        #endregion
        #region Other Stats
        public async Task<ViewResult> ShowAverages()
        {


            var applicationUser = await _userManager.GetUserAsync(User);
            var rolesForUser = await _userManager.GetRolesAsync(applicationUser);
            double gemiddeldeWijzigingen = 0;
            double gemiddeldAantalDagen = 0;

            if (rolesForUser.Contains(Roles.Client.ToString()))
            {
                Client client = _clientRepository.GetByApplicationUserId(applicationUser.Id);
                var tickets = _ticketRepository.GetAllTicketsByCompany(client.Company);
                gemiddeldeWijzigingen = tickets.Average(e => e.AantalWijzigingen);
                gemiddeldAantalDagen = tickets.Where(e => e.DatumAfgesloten != null)
                                              .Select(e => ((e.DatumAfgesloten - e.DatumAangemaakt).Value.TotalDays)).Average();

            }

            else
            {
                gemiddeldeWijzigingen = _ticketRepository.GetAll().Average(e => e.AantalWijzigingen);
                gemiddeldAantalDagen = _ticketRepository.GetAll()
                                                       .Where(e => e.DatumAfgesloten != null)
                                                       .Select(e => ((e.DatumAfgesloten - e.DatumAangemaakt).Value.TotalDays)).Average();
            }


            gemiddeldAantalDagen = Math.Round(gemiddeldAantalDagen, 1);

            ViewBag.AVGWIJZ = gemiddeldeWijzigingen;
            ViewBag.AANTDAGEN = gemiddeldAantalDagen;
            return View();
        }
        #endregion
        #region Hulpmethodes
        private static List<string> getLabelsPastSixMonths()
        {
            return new List<string>()
            {
                DateTime.Now.AddMonths(-6).ToString("m") + " - " + DateTime.Now.AddMonths(-5).ToString("m"),
                DateTime.Now.AddMonths(-5).ToString("m") + " - " + DateTime.Now.AddMonths(-4).ToString("m"),
                DateTime.Now.AddMonths(-4).ToString("m") + " - " + DateTime.Now.AddMonths(-3).ToString("m"),
                DateTime.Now.AddMonths(-3).ToString("m") + " - " + DateTime.Now.AddMonths(-2).ToString("m"),
                DateTime.Now.AddMonths(-2).ToString("m") + " - " + DateTime.Now.AddMonths(-1).ToString("m"),
                DateTime.Now.AddMonths(-1).ToString("m") + " - " + DateTime.Now.ToString("m")
            };
        }
        private static List<int> CountNewTicketsPerMonth(IEnumerable<Ticket> ticketArray)
        {

            List<int> repartitions = new List<int>();

            repartitions.Add(ticketArray.Count(e => e.DatumAangemaakt < DateTime.Now.AddMonths(-5) && e.DatumAangemaakt > DateTime.Now.AddMonths(-6)));
            repartitions.Add(ticketArray.Count(e => e.DatumAangemaakt < DateTime.Now.AddMonths(-4) && e.DatumAangemaakt > DateTime.Now.AddMonths(-5)));
            repartitions.Add(ticketArray.Count(e => e.DatumAangemaakt < DateTime.Now.AddMonths(-3) && e.DatumAangemaakt > DateTime.Now.AddMonths(-4)));
            repartitions.Add(ticketArray.Count(e => e.DatumAangemaakt < DateTime.Now.AddMonths(-2) && e.DatumAangemaakt > DateTime.Now.AddMonths(-3)));
            repartitions.Add(ticketArray.Count(e => e.DatumAangemaakt < DateTime.Now.AddMonths(-1) && e.DatumAangemaakt > DateTime.Now.AddMonths(-2)));
            repartitions.Add(ticketArray.Count(e => e.DatumAangemaakt < DateTime.Now && e.DatumAangemaakt > DateTime.Now.AddMonths(-1)));

            return repartitions;

        }
        private static List<int> CountCompletedTicketsPerMonth(IEnumerable<Ticket> ticketArray)
        {
            List<int> repartitions = new List<int>();

            repartitions.Add(ticketArray.Count(e => e.DatumAfgesloten < DateTime.Now.AddMonths(-5) && e.DatumAfgesloten > DateTime.Now.AddMonths(-6)));
            repartitions.Add(ticketArray.Count(e => e.DatumAfgesloten < DateTime.Now.AddMonths(-4) && e.DatumAfgesloten > DateTime.Now.AddMonths(-5)));
            repartitions.Add(ticketArray.Count(e => e.DatumAfgesloten < DateTime.Now.AddMonths(-3) && e.DatumAfgesloten > DateTime.Now.AddMonths(-4)));
            repartitions.Add(ticketArray.Count(e => e.DatumAfgesloten < DateTime.Now.AddMonths(-2) && e.DatumAfgesloten > DateTime.Now.AddMonths(-3)));
            repartitions.Add(ticketArray.Count(e => e.DatumAfgesloten < DateTime.Now.AddMonths(-1) && e.DatumAfgesloten > DateTime.Now.AddMonths(-2)));
            repartitions.Add(ticketArray.Count(e => e.DatumAfgesloten < DateTime.Now && e.DatumAfgesloten > DateTime.Now.AddMonths(-1)));

            return repartitions;
        }
        #endregion

    }



}
