using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticketing_System_TILE03.Models;
using Ticketing_System_TILE03.Models.Domain;
using Ticketing_System_TILE03.Models.Services;
using Ticketing_System_TILE03.Models.ViewModels;

namespace Ticketing_System_TILE03.Controllers
{
    [Authorize(Roles = "Client")]
    public class ContractController : Controller
    {
        private readonly IContractRepository _contractRepository;
        private readonly IContractTypeRepository _contractTypeRepository;
        private readonly IClientRepository _clientRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IContractService _contractService;

        public ContractController(
            IContractRepository contractRepository,
            IClientRepository clientRepository,
            UserManager<ApplicationUser> userManager,
            IContractTypeRepository contractTypeRepository,
            IContractService contractService)
        {
            _contractRepository = contractRepository;
            _contractTypeRepository = contractTypeRepository;
            _userManager = userManager;
            _clientRepository = clientRepository;
            _contractService = contractService;
        }
        #region Index
        public async Task<IActionResult> Index(ContractStatus? nullableStatus)
        {
            Client client = await GetClientAsync();
            if (client == null) return Challenge();

            IEnumerable<Contract> contracten = _contractService.FindAllByStatus(nullableStatus, client.CompanyId);

            ViewData["Title"] = nullableStatus switch
            {
                ContractStatus.LOPEND => "Lopende contracten",
                ContractStatus.IN_BEHANDELING => "Contracten in behandeling",
                ContractStatus.BEEINDIGD => "Beëindigde contracten",
                _ => "Overzicht contracten"
            };

            ViewData["Statussen"] = GetStatussenSelectList();
            //contracten = new List<Contract>(); //als voorbeeld indien client geen contracten heeft
            return View(contracten);
        }
        #endregion
        #region Details
        public IActionResult Details(int id)
        {
            Contract contract = _contractService.FindByContractId(id);
            if (contract == null) return NotFound();
            return View(contract);
        }

        #endregion
        #region ContractAfsluiten
        public IActionResult ContractAfsluiten()
        {
            IEnumerable<String> contractTypes = _contractService.FindActiveContractTypes().Select(c => c.Naam);
            ViewData["ContractTypes"] = _contractTypeRepository.GetAllActiveContractsAsSelectList();
            int[] periods = { 365, 183, 120 };
            ViewData["Periods"] = new SelectList(periods);

            return View(nameof(ContractAfsluiten), new ContractViewModel());
        }

        [HttpPost, ActionName("ContractAfsluiten")]
        public async Task<IActionResult> ContractAfsluitenBevestig(ContractViewModel contractViewModel)
        {
            Client client = await GetClientAsync();
            if (client == null) return Challenge();

            if (ModelState.IsValid) {
                ContractType contractType = null;
                try
                {
                    contractType = _contractTypeRepository.GetById(int.Parse(contractViewModel.Type.Split('-')[0]));
            
                    if (contractType == null) throw new Exception();
                    if (!contractViewModel.StartDatum.ValidateStartDatum()) throw new ArgumentException();
                    Contract contract = new Contract(contractType, ContractStatus.IN_BEHANDELING, contractViewModel.StartDatum,
                    DateTime.Now.AddDays(contractViewModel.Days));
                    _contractService.AddContractToCompany(contract, client.CompanyId);
                    TempData["message"] = $"Contract '{contractType.Naam}' is succesvol afgesloten.";

                }
                catch(ArgumentException a)
                {
                    TempData["error"] = $"De startdatum mag niet in het verleden liggen en kan niet meer dan een jaar vooraf afgesloten worden. Het contract werd niet afgesloten.";
                }
                catch
                {
                    TempData["error"] = $"Sorry, er is iets verkeerd gelopen, contract '{contractType?.Naam}' is niet afgesloten…";
                }
            }
            return RedirectToAction(nameof(Index));
        }
        #endregion
        #region Delete
        public IActionResult Delete(int id)
        {
            Contract contract = _contractRepository.GetById(id);
            if (contract == null)
                return NotFound();
            ViewData[nameof(Contract.Type.Naam)] = contract.Type.Naam;
            return View();
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            Contract contract = null;
            try
            {
                contract = _contractRepository.GetById(id);
                contract.Status = ContractStatus.BEEINDIGD;
                _contractRepository.SaveChanges();
                TempData["message"] = $"Contract '{contract.Type.Naam}' werd succesvol beëindigd.";
            }
            catch
            {
                TempData["error"] = $"Er is iets misgelopen, contract '{contract?.Type.Naam}' is niet beëindigd…";
            }
            return RedirectToAction(nameof(Index));
        }
        #endregion
        #region Hulpmethodes
        private SelectList GetStatussenSelectList()
        {
            var statusList = Enum.GetValues(typeof(ContractStatus)).Cast<ContractStatus>().ToList();
            return new SelectList(statusList);
        }

        private async Task<Client> GetClientAsync()
        {
            var applicationUser = await _userManager.GetUserAsync(User);
            if (applicationUser == null) return null;
            Client client = _clientRepository.GetByApplicationUserId(applicationUser.Id);
            return client;

        }
        #endregion
    }
}
