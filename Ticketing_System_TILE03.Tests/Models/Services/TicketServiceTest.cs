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
using Ticketing_System_TILE03.Data.Services;
using Ticketing_System_TILE03.Models;
using Ticketing_System_TILE03.Models.Domain;
using Ticketing_System_TILE03.Models.Repositories;
using Ticketing_System_TILE03.Models.Services;
using Ticketing_System_TILE03.Models.ViewModels;
using Ticketing_System_TILE03.Tests.Data;
using Xunit;

namespace Ticketing_System_TILE03.Tests.Controllers
{
    public class TicketServiceTest
    {
        private TicketService _ticketService;
        private readonly Mock<ITicketRepository> _ticketRepoMock = new Mock<ITicketRepository>();
        private readonly Mock<IClientRepository> _clientRepoMock = new Mock<IClientRepository>();
        private readonly Mock<IBijlageRepository> _bijlageRepoMock = new Mock<IBijlageRepository>();
        private readonly Mock<IEmployeeRepository> _employeeRepository = new Mock<IEmployeeRepository>();
        private readonly Mock<ICompanyRepository> _companyRepository = new Mock<ICompanyRepository>();
        private readonly Mock<IContractRepository> _contractRepository = new Mock<IContractRepository>();
        private DummyApplicationDbContext _context;
        private DummyUsers _dummyUsers = new DummyUsers();

        public TicketServiceTest()
        {
            _ticketService = new TicketService(_ticketRepoMock.Object, _clientRepoMock.Object, _bijlageRepoMock.Object, _employeeRepository.Object, _companyRepository.Object, _contractRepository.Object);
            _context = new DummyApplicationDbContext();
        }

        [Fact]
        public void ShouldCreateTicketForUser()
        {
            TicketCreateViewModel model = new TicketCreateViewModel(_context.SingleTicket);
            _clientRepoMock.Setup(c => c.GetByApplicationUserId("123")).Returns(_context.GetTestClient());
            Ticket createdTicket = null;
            _ticketRepoMock.Setup(t => t.Add(It.IsAny<Ticket>())).Callback<Ticket>(captured => createdTicket = captured);

            _ticketService.CreateTicketForUser(_dummyUsers.ApplicationUsers[0], model, new List<string>() { "Client" });

            Assert.Equal(model.DatumAangemaakt, createdTicket.DatumAangemaakt);
            Assert.Equal(model.Titel, createdTicket.Titel);
            Assert.Equal(model.Omschrijving, createdTicket.Omschrijving);
            Assert.Equal(Status.AANGEMAAKT, createdTicket.Status);
            Assert.Equal(_context.GetTestCompany().Name, createdTicket.Company.Name);
            _clientRepoMock.Verify(c => c.GetByApplicationUserId("123"));
            _ticketRepoMock.Verify(t => t.Add(It.IsAny<Ticket>()));
            _ticketRepoMock.Verify(t => t.SaveChanges());
        }

        [Fact]
        public void ShouldDeleteTicket()
        {
            Ticket foundTicket = _context.SingleTicket;
            _ticketRepoMock.Setup(t => t.GetById(foundTicket.TicketId)).Returns(foundTicket);

            _ticketService.DeleteTicket(foundTicket.TicketId);

            _ticketRepoMock.Verify(t => t.Delete(foundTicket));
            _ticketRepoMock.Verify(t => t.SaveChanges());
        }

        [Fact]
        public void ShouldFindTicket() 
        {
            _ticketService.FindTicket(123);

            _ticketRepoMock.Verify(t => t.GetById(123));
        }

    }

}
