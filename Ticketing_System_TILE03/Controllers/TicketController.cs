using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ticketing_System_TILE03.Models;
using Ticketing_System_TILE03.Models.Domain;
using Ticketing_System_TILE03.Models.Services;
using Ticketing_System_TILE03.Models.ViewModels;

namespace Ticketing_System_TILE03.Controllers
{
    [Authorize(Roles = "Client,Technician,SupportManager,Administrator")]
    public class TicketController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly ICompanyRepository _companyRepository;
        private readonly IContractRepository _contractRepository;
        private readonly IClientRepository _clientRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public TicketController(ITicketService ticketService,
            UserManager<ApplicationUser> userManager,
            ICompanyRepository companyRepository,
            IContractRepository contractRepository,
            IClientRepository clientRepository)
        {
            _ticketService = ticketService;
            _userManager = userManager;
            _companyRepository = companyRepository;
            _contractRepository = contractRepository;
            _clientRepository = clientRepository;
        }

        #region Methods GET/POST

        #region Index
        // GET: TicketController
        public async Task<ViewResult> Index(Status? status)
        {
            //user ophalen en laatste keer dat tickets bekeken zijn aanpassen
            var applicationUser = await _userManager.GetUserAsync(User);
            await UpdateLastSession(applicationUser);
            var rolesForUser = await _userManager.GetRolesAsync(applicationUser);

            IEnumerable<Ticket> tickets = _ticketService.FindTicketsByStatusAndUserIdAndRole(status, applicationUser.Id, rolesForUser.ToList());

            ViewData["Title"] = status switch
            {
                Status.AANGEMAAKT => "Aangemaakte tickets",
                Status.AFGEHANDELD => "Afgehandelde tickets",
                Status.GEANNULEERD => "Geannuleerde tickets",
                Status.IN_BEHANDELING => "Tickets in behandeling",
                _ => "Openstaande tickets"
            };

            ViewData["Statussen"] = GetStatussenSelectList();
            return View(tickets);
        }

        #endregion
        #region Details

        // GET: TicketController/Details/5
        public IActionResult Details(int id)
        {
            Ticket ticket = _ticketService.FindTicket(id);
            return View(ticket);
        }


        #endregion
        #region Create

        // GET: TicketController/Create
        public async Task<ViewResult> Create()
        {
            var applicationUser = await _userManager.GetUserAsync(User);
            var rolesForUser = await _userManager.GetRolesAsync(applicationUser);


            ViewData["Contracts"] = null;

            if (rolesForUser.Contains(Roles.SupportManager.ToString()))
            {


                ViewData["AvailableCompanies"] = _companyRepository.FindAllCompaniesAsSelectList();
                ViewData["Contracts"] = _contractRepository.GetAllAsSelectList();
            }

            if (rolesForUser.Contains(Roles.Client.ToString()))
            {
                Client currentClient = _clientRepository.GetByApplicationUserId(applicationUser.Id);

                if (currentClient.Company.Contracts == null)
                {
                    ViewData["message"] = "Er werd geen geschikt support contract gevonden voor het aanmaken van een ticket";
                    //weergeven en view en terug naar index
                }

                //in Else blok zetten
                ViewData["Contracts"] = _contractRepository.GetActiveContractsOfCompanyAsSelectList(currentClient.Company);


            }

            return View(new TicketCreateViewModel());
        }

        // POST: TicketController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TicketCreateViewModel ticketViewModel) //hier komt dan de EditViewModel
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var applicationUser = await _userManager.GetUserAsync(User);
                    var rolesForUser = await _userManager.GetRolesAsync(applicationUser);

                    _ticketService.CreateTicketForUser(applicationUser, ticketViewModel, rolesForUser.ToList());
                    TempData["message"] = $"You successfully added ticket {ticketViewModel.Titel}.";
                }
                catch
                {
                    TempData["error"] = "Sorry, something went wrong, the ticket was not created...";
                }
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        #endregion
        #region Edit

        // GET: TicketController/Edit/5
        public async Task<ViewResult> EditAsync(int id)
        {

            var applicationUser = await _userManager.GetUserAsync(User);
            var rolesForUser = await _userManager.GetRolesAsync(applicationUser);

            foreach (String role in Enum.GetNames(typeof(Roles)))
            {
                ViewData[role] = rolesForUser.Contains(role);
            }


            Ticket ticket = _ticketService.FindTicket(id);

            ViewData["Statussen"] = GetStatussenSelectList();
            return View(new TicketEditViewModel(ticket));
        }


        // POST: TicketController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(int id, TicketEditViewModel ticketViewModel) //hier komt dan de EditViewModel
        {
            if (ModelState.IsValid)
            {

                int wijzigingen = _ticketService.FindTicket(id).AantalWijzigingen;

                try
                {
                    var applicationuser = await _userManager.GetUserAsync(User);
                    
                    _ticketService.UpdateTicket(id, ticketViewModel, applicationuser);

                    if (_ticketService.FindTicket(id).AantalWijzigingen == wijzigingen)
                    {
                        TempData["nochanges"] = $"Ticket {ticketViewModel.Titel} not changed";
                    }
                    else
                    {
                        TempData["message"] = $"You successfully updated ticket {ticketViewModel.Titel}.";
                    }
                }
                catch
                {
                    TempData["error"] = $"Ticket was not successfully updated {ticketViewModel.Titel}.";
                }
            }
            return RedirectToAction(nameof(Index));
        }

        #endregion
        #region Delete


        // GET: TicketController/Delete/5
        public ActionResult Delete(int id)
        {
            Ticket ticket = _ticketService.FindTicket(id);
            if (ticket == null)
                return NotFound();
            ViewData[nameof(Ticket.Titel)] = ticket.Titel;
            return View();
        }

        // POST: TicketController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) //iformcontroller mag dan ook weg
        {
            try
            {
                _ticketService.DeleteTicket(id);
                TempData["message"] = $"You successfully deleted ticket.";
            }
            catch
            {
                TempData["error"] = $"Sorry, something went wrong, ticket was not deleted…";
            }
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #endregion

        #region hulpmethodes

        public IActionResult DownloadFile(int id)
        {
            Ticket ticket = _ticketService.FindTicket(id);
            byte[] docContent = ticket.Bijlage.DataFiles;
            string mimeType = "application/octet-stream";
            Response.Headers.Add("Content-Disposition", "inline; filename=" + ticket.Bijlage.Name);
            Response.ContentType = "application/octet-stream";
            return File(docContent, mimeType, ticket.Bijlage.Name);
        }

        private SelectList GetStatussenSelectList()
        {
            var statusList = Enum.GetNames(typeof(Status)).ToList();
            return new SelectList(statusList);
        }


        private async Task UpdateLastSession(ApplicationUser applicationUser)
        {
            applicationUser.LastSession = DateTime.Now;
            await _userManager.UpdateAsync(applicationUser);
        }

        private byte[] GetByteArrayFromImage(IFormFile file)
        {
            using (var target = new MemoryStream())
            {
                file.CopyTo(target);
                return target.ToArray();
            }
        }
        #endregion

    }
}
