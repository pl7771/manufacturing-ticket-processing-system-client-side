using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticketing_System_TILE03.Models;
using Ticketing_System_TILE03.Models.Domain;

namespace Ticketing_System_TILE03.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IContractRepository _contractRepository;
        private readonly IClientRepository _clientRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(UserManager<ApplicationUser> userManager, ITicketRepository ticketRepository, IClientRepository clientRepository, IContractRepository contractRepository)
        {
            _ticketRepository = ticketRepository;
            _contractRepository = contractRepository;
            _clientRepository = clientRepository;
            _userManager = userManager;
            
        }

        public async Task<ActionResult> Index()
        {
            //roles van user ophalen en voor elke role een ViewData aanmaken met een bool die aangeeft of user de role heeft
            var applicationUser = await _userManager.GetUserAsync(User);
            if (applicationUser == null) return Challenge();
            var rolesForUser = await _userManager.GetRolesAsync(applicationUser);

            foreach (String role in Enum.GetNames(typeof(Roles)))
            {
                ViewData[role] = rolesForUser.Contains(role);
            }

            //laatste sessie van user ophalen en doorgeven
            ViewData["LastSession"] = applicationUser.LastSession;

            //Ophalen of een contract deze maand vervalt
            ViewData["contractExpires"] = false;
            //aantal wijzigingen aan tickets ophalen en doorgeven
            int changes = 0;

            //Indien gebruiker Client is - wijzigingen opvragen voor tickets van zijn company
            if (rolesForUser.Contains(Roles.Client.ToString()))
            {
                Client client = _clientRepository.GetByApplicationUserId(applicationUser.Id);
                changes = _ticketRepository.GetChangeCountByCompany(applicationUser.LastSession, client.Company);
                ViewData["contractExpires"] = _contractRepository.ExpiresThisMonth(client.Company);
            }
            //Indien gebruiker geen Client is - wijzigingen voor alle tickets ophalen
            else
                changes = _ticketRepository.GetChangeCount(applicationUser.LastSession);

            //Indien het aantal wijzigingen groter is dan 0 - Doorgeven van notificatie aan view
            ViewData["changes"] = changes > 0 ? changes.ToString() : "";

            return View();
        }



        public IActionResult KnowledgeBase() {
            return View();
        }

    }
}
