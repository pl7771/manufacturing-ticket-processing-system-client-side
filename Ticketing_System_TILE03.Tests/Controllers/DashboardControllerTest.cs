using System;
using System.Collections.Generic;
using System.Text;
using Ticketing_System_TILE03.Controllers;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using Ticketing_System_TILE03.Models;
using Microsoft.AspNetCore.Mvc;
using Ticketing_System_TILE03.Models.Domain;
using System.Threading.Tasks;
using System.Security.Claims;
using Ticketing_System_TILE03.Tests.Data;

namespace Ticketing_System_TILE03.Tests.Controllers
{
    public class DashboardControllerTest
    {
        private readonly DashboardController _controller;
        private readonly Mock<ITicketRepository> _ticketRepository;
        private readonly Mock<IContractRepository> _contractRepository;
        private readonly Mock<IClientRepository> _clientRepository;
        private readonly Mock<UserManager<ApplicationUser>> _userManager;
        private readonly DummyUsers _dummyUsers;
        private readonly DummyApplicationDbContext _dummyContext;

        public DashboardControllerTest()
        {
            _dummyContext = new DummyApplicationDbContext();
            _dummyUsers = new DummyUsers();
            //repositories mocken
            _ticketRepository = new Mock<ITicketRepository>();
            _contractRepository = new Mock<IContractRepository>();
            _clientRepository = new Mock<IClientRepository>();
            //UserManager mocken
            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            //mocks injecteren
            _controller = new DashboardController(_userManager.Object, _ticketRepository.Object, _clientRepository.Object, _contractRepository.Object);

            //mocks trainen
            TrainUserManager();
            TrainRepositories();

        }

        [Fact]
        public void Index_Stores_Roles_LastSession_InViewData_and_ReturnsViewResult()
        {
            //arrange
            foreach(ApplicationUser user in _dummyUsers.ApplicationUsers)
            {
                //train mocks
                _userManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
                _userManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { Enum.Parse(typeof(Roles), user.FirstName).ToString() });

                //act + assert
                var result = Assert.IsType<ViewResult>(_controller.Index().Result);
                Assert.Equal(true, result.ViewData[user.FirstName]); //testen of waarde voor viewdata met naam van role waarde true heeft
                foreach (String role in Enum.GetNames(typeof(Roles))) //testen of voor de resterende roles ook een ViewData wordt aangemaakt met waarde false
                {
                    if(user.FirstName != role)
                    Assert.Equal(false, result.ViewData[role]);
                }
                Assert.Equal(user.LastSession, result.ViewData["LastSession"]);
            }
        }

        [Fact]
        public void Index_ApplicationUserIsNull_ReturnsChallengeResult()
        {
            //act
            var result = Assert.IsType<ChallengeResult>(_controller.Index().Result);

        }

        [Fact]
        public void Index_IfClient_Stores_ChangesAndContractExpires_InViewData()
        {
            ApplicationUser user = _dummyUsers.ApplicationUsers[0];
            _userManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { Enum.Parse(typeof(Roles), user.FirstName).ToString() });
            //act + assert
            var result = Assert.IsType<ViewResult>(_controller.Index().Result);
            Assert.Equal("3", result.ViewData["changes"]);
            Assert.Equal(true, result.ViewData["contractExpires"]);
        }

        [Fact]
        public void Index_IfNoClient_DoesNotStore_ChangesAndContractExpires_InViewData()
        {
            ApplicationUser user = _dummyUsers.ApplicationUsers[1];
            _userManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { Enum.Parse(typeof(Roles), user.FirstName).ToString() });
            //act + assert
            var result = Assert.IsType<ViewResult>(_controller.Index().Result);
            Assert.Equal("", result.ViewData["changes"]);
            Assert.Equal(false, result.ViewData["contractExpires"]);
        }

        private void TrainUserManager()
        {
            ApplicationUser applicationUserIsNull = null;
            _userManager.Setup(m => m.GetUserAsync(null)).ReturnsAsync(applicationUserIsNull);

        }

        private void TrainRepositories()
        {
            _clientRepository.Setup(m => m.GetByApplicationUserId("123")).Returns(_dummyContext.GetTestClient());
            _contractRepository.Setup(cr => cr.ExpiresThisMonth(It.IsAny<Company>())).Returns(true);
            _ticketRepository.Setup(m => m.GetChangeCountByCompany(It.IsAny<DateTime>(), It.IsAny<Company>())).Returns(3);
        }
    }
}
