using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Ticketing_System_TILE03.Controllers;
using Ticketing_System_TILE03.Models;
using Ticketing_System_TILE03.Models.Domain;
using Ticketing_System_TILE03.Models.Services;
using Ticketing_System_TILE03.Models.ViewModels;
using Ticketing_System_TILE03.Tests.Data;
using Xunit;

namespace Ticketing_System_TILE03.Tests.Controllers
{
    public class ContractControllerTest
    {
        private readonly ContractController _controller;
        private readonly Mock<IContractRepository> _contractRepository;
        private readonly Mock<IContractTypeRepository> _contractTypeRepository;
        private readonly Mock<IClientRepository> _clientRepository;
        private readonly Mock<IContractService> _contractService;
        private readonly Mock<UserManager<ApplicationUser>> _userManager;
        private readonly DummyUsers _dummyUsers;
        private readonly DummyApplicationDbContext _dummyApplicationDbContext;
        private readonly Client _client;
        private readonly ApplicationUser _user;

        public ContractControllerTest()
        {
            _dummyUsers = new DummyUsers();
            _dummyApplicationDbContext = new DummyApplicationDbContext();
            //Repositories mocken
            _contractRepository = new Mock<IContractRepository>();
            _contractTypeRepository = new Mock<IContractTypeRepository>();
            _clientRepository = new Mock<IClientRepository>();
            _contractService = new Mock<IContractService>();
            //UserManager mocken
            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            //mocks injecteren
            _controller = new ContractController(
                _contractRepository.Object,
                _clientRepository.Object,
                _userManager.Object,
                _contractTypeRepository.Object,
                _contractService.Object)
            {
                TempData = new Mock<ITempDataDictionary>().Object
            };
            //Client
            _client = new Client() { ClientId = "123", ApplicationUser = _dummyUsers.ApplicationUsers[0] };
            _user = _client.ApplicationUser;

            //train mocks
            _userManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(_user);
            _userManager.Setup(m => m.GetRolesAsync(_user)).ReturnsAsync(new List<string> { Enum.Parse(typeof(Roles), _user.FirstName).ToString() });
            //geen contract type gevonden
            _contractTypeRepository.Setup(r => r.GetById(999)).Returns((ContractType)null);
            _clientRepository.Setup(cr => cr.GetByApplicationUserId(_user.Id)).Returns(_client);

        }
        #region Index
        [Theory]
        [InlineData(null, 3, "Overzicht contracten")] //contracten in behandeling en lopende contracten
        [InlineData(ContractStatus.LOPEND, 2, "Lopende contracten")]
        [InlineData(ContractStatus.IN_BEHANDELING, 1, "Contracten in behandeling")]
        [InlineData(ContractStatus.BEEINDIGD, 2, "Beëindigde contracten")]
        public void Index_PassesListOfContractsInViewResultModelAndStoresTitleAndStatussenSelectListInviewData(ContractStatus? nullableStatus, int aantalContracten, string title)
        {
            //Setup
            _contractService.Setup(c => c.FindAllByStatus(nullableStatus, null)).Returns(_dummyApplicationDbContext.GetDummyContractsFor(nullableStatus));

            //act + assert
            var result = Assert.IsType<ViewResult>(_controller.Index(nullableStatus).Result);
            //assert
            var contractsInModel = Assert.IsAssignableFrom<IEnumerable<Contract>>(result.Model);
            Assert.Equal(aantalContracten, contractsInModel.Count());
            Assert.Equal(title, result.ViewData["Title"]);
            Assert.IsType<SelectList>(result.ViewData["Statussen"]);

        }

        [Fact]
        public void Index_ApplicationUserIsNull_ReturnsChallengeResult()
        {
            //arrange
            ApplicationUser user = null;
            _userManager.Setup(m => m.GetUserAsync(null)).ReturnsAsync(user);
            //act
            var result = Assert.IsType<ChallengeResult>(_controller.Index(null).Result);

        }
        #endregion
        #region Details
        [Fact]
        public void Details_ReturnsNotFoundIfContractNotFound()
        {
            Assert.IsType<NotFoundResult>(_controller.Details(999));
        }

        [Fact]
        public void Details_PassesContractInViewResultModelIfContractIsFound()
        {
            Contract contract = _dummyApplicationDbContext.Contracts[0];
            _contractService.Setup(s => s.FindByContractId(1)).Returns(contract);
            var result = Assert.IsType<ViewResult>(_controller.Details(1));
            Assert.Equal(contract, result.Model);


        }
        #endregion
        #region ContractAfsluiten GET
        [Fact]
        public void ContractAfsluiten_PassesSelectListOfContractTypesAndPeriodsAndContractViewModel_InViewResultModel()
        {
            //train
            _contractTypeRepository.Setup(ctr => ctr.GetAllActiveContractsAsSelectList()).Returns(new SelectList(new List<string>()));
            IEnumerable<ContractType> actieveContractTypes = _dummyApplicationDbContext.ActieveContractTypes;
            _contractService.Setup(s => s.FindActiveContractTypes()).Returns(actieveContractTypes);
            var result = Assert.IsType<ViewResult>(_controller.ContractAfsluiten());
            var contractViewModel = Assert.IsType<ContractViewModel>(result.Model);
            Assert.IsType<SelectList>(result.ViewData["ContractTypes"]);
            Assert.IsType<SelectList>(result.ViewData["Periods"]);
        }
        #endregion
        #region ContractAfsluitenBevestig POST
        [Fact]
        public void ContractAfsluitenBevestig_ReturnsChallengeIfClientIsNotFound() //aanpassen
        {
            //arrange
            ApplicationUser user = null;
            _userManager.Setup(m => m.GetUserAsync(null)).ReturnsAsync(user);
            //act
            var result = Assert.IsType<ChallengeResult>(_controller.ContractAfsluitenBevestig(new ContractViewModel()).Result);

        }
        [Fact]
        public void ContractAfsluitenBevestig_ModelStateErrors_DoesNotCreateNorPersistsContractAndRedirectsToAction()
        {
            _controller.ModelState.AddModelError("", "Error message");
            var result = Assert.IsType<RedirectToActionResult>(_controller.ContractAfsluitenBevestig(new ContractViewModel()).Result);
            _contractService.Verify(cs => cs.AddContractToCompany(It.IsAny<Contract>(), It.IsAny<string>()), Times.Never());
        }
        [Fact]
        public void ContractAfsluitenBevestig_ContractTypeIsFound_CreatesAndPersistsContractAndRedirectsToActionIndex()
        {
            ContractType contractType = _dummyApplicationDbContext.ContractTypes[0];
            _contractTypeRepository.Setup(r => r.GetById(1)).Returns(contractType);
            var result = Assert.IsType<RedirectToActionResult>(_controller.ContractAfsluitenBevestig(new ContractViewModel() { Days = 365, Type = "1-contractNaam"}).Result);
            Assert.Equal("Index", result?.ActionName);
            _contractService.Verify(cs => cs.AddContractToCompany(It.IsAny<Contract>(), It.IsAny<string>()), Times.Once());

        }
        [Fact]
        public void ContractAfsluitenBevestig_ContractIsNotFound_DoesNotPersistsContractAndRedirectsToActionIndex() //aanpassen
        {
            _contractTypeRepository.Setup(r => r.GetById(999)).Returns((ContractType) null);
            var result = Assert.IsType<RedirectToActionResult>(_controller.ContractAfsluitenBevestig(new ContractViewModel() { Days = 365, Type = "999-contractNaam" }).Result);
            Assert.Equal("Index", result?.ActionName);
            _contractService.Verify(cs => cs.AddContractToCompany(It.IsAny<Contract>(), It.IsAny<string>()), Times.Never());


        }
        #endregion
        #region Delete GET
        [Fact]
        public void Delete_PassesNameOfContractTypeInViewData()
        {
            _contractRepository.Setup(m => m.GetById(1)).Returns(_dummyApplicationDbContext.Contracts[0]);
            var result = Assert.IsType<ViewResult>(_controller.Delete(1));
            Assert.Equal("Contract1", result.ViewData["Naam"]);
        }

        [Fact]
        public void Delete_UnknownContract_ReturnsNotFound()
        {
            _contractRepository.Setup(m => m.GetById(1)).Returns((Contract)null);
            IActionResult action = _controller.Delete(1);
            Assert.IsType<NotFoundResult>(action);
        }
        #endregion
        #region DeleteConfirmed POST
        [Fact]
        public void DeleteConfirmed_ExistingContract_ChangesContractStatusToBeëindigdAndRedirectsToActionIndex()
        {
            Contract contract = _dummyApplicationDbContext.Contracts[0];
            _contractRepository.Setup(m => m.GetById(1)).Returns(contract);
            var result = Assert.IsType<RedirectToActionResult>(_controller.DeleteConfirmed(1));
            Assert.Equal("Index", result.ActionName);
            _contractRepository.Verify(m => m.GetById(1), Times.Once());
            Assert.Equal(ContractStatus.BEEINDIGD, contract.Status);
            _contractRepository.Verify(m => m.SaveChanges(), Times.Once());
        }
        #endregion
    }
}
