using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Ticketing_System_TILE03.Controllers;
using Ticketing_System_TILE03.Models;
using Ticketing_System_TILE03.Models.Domain;
using Ticketing_System_TILE03.Models.Services;
using Ticketing_System_TILE03.Models.ViewModels;
using Ticketing_System_TILE03.Tests.Data;
using Xunit;

namespace Ticketing_System_TILE03.Tests.Controllers
{
    public class TicketControllerTest
    {
        private readonly TicketController _ticketController;
        private readonly Mock<ITicketService> _ticketService = new Mock<ITicketService>();
        private readonly Mock<ICompanyRepository> _companyRepository = new Mock<ICompanyRepository>();
        private readonly Mock<IClientRepository> _clientRepository = new Mock<IClientRepository>();
        private readonly Mock<IContractRepository> _contractRepository = new Mock<IContractRepository>();
        private readonly Mock<UserManager<ApplicationUser>> _userManager;
        private DummyApplicationDbContext _context;
        private readonly DummyUsers _dummyUsers;


        public TicketControllerTest()
        {
            //dummies instantieren
            _context = new DummyApplicationDbContext();
            _dummyUsers = new DummyUsers();

            //UserManager mocken en trainen
            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            ApplicationUser user = _dummyUsers.ApplicationUsers[1];
            _userManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { Enum.Parse(typeof(Roles), user.FirstName).ToString() });

            //Controller instantieren
            _ticketController = new TicketController(_ticketService.Object, _userManager.Object, _companyRepository.Object, _contractRepository.Object, _clientRepository.Object)
            {
                TempData = new Mock<ITempDataDictionary>().Object
            };
        }

        #region Index
        [Fact]
        public async void Index_changesLastSession_OfApplicationUser_ToCurrentDateTime()
        {
            foreach (ApplicationUser user in _dummyUsers.ApplicationUsers)
            {
                //train
                _userManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
                _userManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Client" });
                //act: index aanropen
                await _ticketController.Index(Status.AANGEMAAKT); //Enum.Parse(typeof(Status), user.FirstName)
                //Assert: is lastSession de huidige tijd?
                Assert.Equal(DateTime.Now, user.LastSession, TimeSpan.FromMilliseconds(2000d));

            }

        }

        [Fact]
        public void Index_PassesOrderedList_OfTickets()
        {
            _ticketService.Setup(s => s.FindTicketsByStatusAndUserIdAndRole(null, _dummyUsers.ApplicationUsers[1].Id, It.IsAny<List<string>>()))
                .Returns(_context.Tickets.Where(e => e.Status == Status.AANGEMAAKT || e.Status == Status.IN_BEHANDELING).ToList());
            //Act ------------------------------------------
            ViewResult result = Assert.IsType<ViewResult>(_ticketController.Index(null).Result);

            //Assert-----------------------------------------

            //Is het Model prop van ViewResult result een lijst van type Ticket
            var model = Assert.IsAssignableFrom<IEnumerable<Ticket>>(result.Model);

            //enkel de tickets met "aangemaakt" en "inbehandeling" statuut worden weergegeven
            //in de dummydb hebben we 4 tickets met deze status
            Assert.Equal(4, model.Count());

        }

        [Fact]
        public void Index_ListOfTickets_WithSpecified_Category_Return_view_with_list_of_tickets_with_specific_categories()
        {
            _ticketService.Setup(s => s.FindTicketsByStatusAndUserIdAndRole(Status.GEANNULEERD, _dummyUsers.ApplicationUsers[1].Id, It.IsAny<List<string>>()))
                .Returns(_context.Tickets.Where(t => t.Status == Status.GEANNULEERD).ToList());

            ViewResult result = Assert.IsType<ViewResult>(_ticketController.Index(Status.GEANNULEERD).Result);

            var model = Assert.IsAssignableFrom<IEnumerable<Ticket>>(result.Model);
            Assert.Equal(2, model.Count());
            _ticketService.Verify(s => s.FindTicketsByStatusAndUserIdAndRole(Status.GEANNULEERD, _dummyUsers.ApplicationUsers[1].Id, It.IsAny<List<string>>()));
        }


        [Fact]
        public void Index_passes_select_list_with_all_categories()
        {
            // _ticketRepository.Setup(t => t.GetAll()).Returns(_context.Tickets);
            ViewResult result = Assert.IsType<ViewResult>(_ticketController.Index(null).Result);
            var categories = Assert.IsType<SelectList>(result.ViewData["Statussen"]);
            Assert.Equal(4, categories.Count());
        }

        [Fact(Skip = "te testen, is net nieuw in index")]
        public void Index_passesTitleInView()
        {

        }
        #endregion

        #region Detail
        [Fact]
        public void Detail_passes_Ticketmodel()
        {
            _ticketService.Setup(s => s.FindTicket(7)).Returns(_context.SingleTicket);

            var result = Assert.IsType<ViewResult>(_ticketController.Details(7));
            var model = Assert.IsType<Ticket>(result.Model);

            Assert.Equal("Het is de ticket9", model.Titel);
            Assert.Equal("Er zit al de omschrijving9", model.Omschrijving);
            Assert.Equal(7, model.TicketId);
            Assert.Equal(Status.AANGEMAAKT, model.Status);
            _ticketService.Verify(s => s.FindTicket(7));

        }
        #endregion

        #region Create       
        //GET methode test
        [Fact]
        public void Create_passes_ticketViewMode()
        {
            var result = Assert.IsAssignableFrom<Task<ViewResult>>(_ticketController.Create()).Result;
            var ticketModel = Assert.IsType<TicketCreateViewModel>(result.Model);
            Assert.Null(ticketModel.Titel);
            Assert.Null(ticketModel.Omschrijving);
        }


        //POST methode
        [Fact]
        public void Create_valid_ticket_creates_and_persistes_and_redirects_to_action()
        {
            var ticketModel = new TicketCreateViewModel(_context.SingleTicket);

            var result = Assert.IsType<RedirectToActionResult>(_ticketController.Create(ticketModel).Result);

            Assert.Equal("Index", result.ActionName);
            _ticketService.Verify(s => s.CreateTicketForUser(_dummyUsers.ApplicationUsers[1], ticketModel, new List<string>() { "Technician" }));
        }

        //POST methode met een fout
        [Fact]
        public void Create_domain_errors_doesnt_create_nor_persist()
        {
            TicketCreateViewModel ticketModel = new TicketCreateViewModel();

            _ticketController.ModelState.AddModelError("any error", "any error");
            ViewResult result = _ticketController.Create(ticketModel).Result as ViewResult;
            _ticketService.Verify(s => s.CreateTicketForUser(_dummyUsers.ApplicationUsers[1], ticketModel, null), Times.Never());
        }
        #endregion

        #region Delete
        [Fact]
        public void delete_passes_titel_of_ticket_in_view()
        {
            _ticketService.Setup(s => s.FindTicket(1)).Returns(_context.SingleTicket);

            var result = Assert.IsType<ViewResult>(_ticketController.Delete(1));

            Assert.Equal("Het is de ticket9", result.ViewData["Titel"]);
            _ticketService.Verify(s => s.FindTicket(1));
        }

        [Fact]
        public void Delete_Unknown_Ticket_Returns_NotFound()
        {
            IActionResult action = _ticketController.Delete(1);
            Assert.IsType<NotFoundResult>(action);
        }

        [Fact]
        public void Delete_ExistingTicket_DeletesTicketAndPersistsChangesAndRedirectsToActionIndex()
        {
            var result = Assert.IsType<RedirectToActionResult>(_ticketController.DeleteConfirmed(1));
            Assert.Equal("Index", result.ActionName);
            _ticketService.Verify(s => s.DeleteTicket(1));
        }

        [Fact]
        public void Delete_Not_Existant_Ticket_Doesnt_Crash_The_Application()
        {
            _ticketService.Setup(s => s.DeleteTicket(1)).Throws(new NullReferenceException());

            var result = Assert.IsType<RedirectToActionResult>(_ticketController.DeleteConfirmed(1));
            Assert.Equal("Index", result.ActionName);
            _ticketService.Verify(s => s.DeleteTicket(1));
        }
        #endregion

        #region Edit
        // get test
        [Fact]
        public void edit_passes_ticket_in_edit_view_model_and_return_select_list_status()
        {
            _ticketService.Setup(s => s.FindTicket(1)).Returns(_context.SingleTicket);

            var result = Assert.IsType<ViewResult>(_ticketController.EditAsync(1).Result);
            var model = Assert.IsType<TicketEditViewModel>(result.Model);
            var statusInViewData = Assert.IsType<SelectList>(result.ViewData["Statussen"]);

            Assert.Equal("Het is de ticket9", model.Titel);
            Assert.Equal(Status.AANGEMAAKT, model.Status);
            Assert.Equal(4, statusInViewData.Count());
            _ticketService.Verify(s => s.FindTicket(1));
        }

        [Fact]
        public void Edit_ValidEdit_UpdatesAndPersistsTicket()
        {
            Ticket ticket = _context.SingleTicket; //heeft id 7
            TicketEditViewModel model = new TicketEditViewModel(ticket);
            _ticketService.Setup(s => s.FindTicket(ticket.TicketId)).Returns(ticket);

            var result = Assert.IsType<RedirectToActionResult>(_ticketController.EditAsync(ticket.TicketId, model).Result);

            Assert.Equal("Index", result.ActionName);
            _ticketService.Verify(s => s.UpdateTicket(ticket.TicketId, model, _dummyUsers.ApplicationUsers[1]));
        }

        [Fact]
        public void Edit_InvalidEdit_DoesnotPersistOrUpdateTicket()
        {
        

            Ticket ticket = _context.SingleTicket; //heeft id 7
                                                   // _ticketRepository.Setup(e => e.GetById(7)).Returns(ticket); 
            _ticketService.Setup(s => s.FindTicket(7)).Returns(ticket);
            TicketEditViewModel model = new TicketEditViewModel(ticket);

            char[] charArray = new char[101];
            Array.Fill(charArray, 'a');
            String TeLangeString = new String(charArray);

            model.Titel = TeLangeString; // mag niet opgeslagen worden

            var result = Assert.IsType<RedirectToActionResult>(_ticketController.EditAsync(ticket.TicketId, model).Result);

            Assert.Equal("Index", result.ActionName);
            //  _ticketRepository.Verify(ticket => ticket.SaveChanges(), Times.Never);

        }
        #endregion
    }

}
