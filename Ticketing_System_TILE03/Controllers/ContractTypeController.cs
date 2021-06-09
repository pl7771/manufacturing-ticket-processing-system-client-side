using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticketing_System_TILE03.Models;
using Ticketing_System_TILE03.Models.Domain;
using Ticketing_System_TILE03.Models.ViewModels;

namespace Ticketing_System_TILE03.Controllers
{// Contract Type hoort bij het java deel, daarom heb ik verder geen annotaties of tests geschreven
    [Authorize(Roles = "SupportManager")]
    public class ContractTypeController : Controller
    {
        private readonly IContractTypeRepository _contractTypeRepository;
        private readonly IContractRepository _contractRepository;
        private readonly ITicketRepository _ticketRepository;

        public ContractTypeController(IContractTypeRepository contractTypeRepository, IContractRepository contractRepository, ITicketRepository ticketRepository)
        {
            _contractTypeRepository = contractTypeRepository;
            _contractRepository = contractRepository;
            _ticketRepository = ticketRepository;
        }

        public IActionResult Index(ContractTypeStatus? nullableStatus)
        {
            IEnumerable<ContractType> contractTypes;
            //var test = nullableStatus.GetValueOrDefault();
            ContractTypeStatus status = nullableStatus.HasValue ? nullableStatus.GetValueOrDefault() : ContractTypeStatus.ACTIEF;
            contractTypes = _contractTypeRepository.GetByStatus(status);

            foreach (var contractType in contractTypes) {
                var aantal = _contractRepository.GetAll().Where(c => c.Type.ContractTypeId == contractType.ContractTypeId && c.Status == ContractStatus.LOPEND).Count();
                contractType.aantalContracten = aantal;
            }

            ViewData["Title"] = status switch
            {
                ContractTypeStatus.ACTIEF => "Actieve contracttypes",
                _ => "Niet actieve contracttypes"
            };

            ViewData["Statussen"] = GetStatussenSelectList();

            return View(contractTypes);
        }

        #region Edit
        public ActionResult Edit(int id)
        {
            ContractType contract = _contractTypeRepository.GetById(id);
            if (contract == null) return NotFound();
            setViewDataEditView();
            ViewData["IsEdit"] = true;
            return View(new ContractTypeEditViewModel(contract));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ContractTypeEditViewModel contractTypeEditViewModel, int id)
        {
            if (ModelState.IsValid)
            {
                ContractType contract = null;
                try
                {
                    contract = _contractTypeRepository.GetById(id);
                    MapContractTypeEditViewModelToContractType(contractTypeEditViewModel, contract);
                    _contractTypeRepository.SaveChanges();
                    TempData["message"] = $"Contract {contract.Naam} werd succesvol aangepast.";

                }
                catch
                {
                    TempData["error"] = $"Contract {contract.Naam} werd niet succesvol aangepast.";
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IsEdit"] = true;
            setViewDataEditView();
            return View(nameof(Edit), contractTypeEditViewModel);

        }

        #endregion
        #region Delete
        public ActionResult Delete(int id)
        {
            ContractType contractType = _contractTypeRepository.GetById(id);
            if (contractType == null)
                return NotFound();
            ViewData[nameof(ContractType.Naam)] = contractType.Naam;
            return View();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ContractType contractType = null;
            try
            {
                contractType = _contractTypeRepository.GetById(id);
                _contractTypeRepository.Delete(contractType);
                _contractTypeRepository.SaveChanges();
                TempData["message"] = $"Je hebt contract {contractType.Naam} succesvol verwijderd.";
            }
            catch
            {
                TempData["error"] = $"Er is iets misgelopen, contract {contractType?.Naam} is niet verwijderd…";
            }
            return RedirectToAction(nameof(Index));
        }
        #endregion
        #region Create

        public IActionResult Create()
        {
            setViewDataEditView();
            ViewData["IsEdit"] = false;
            return View(nameof(Edit), new ContractTypeEditViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ContractTypeEditViewModel contractTypeEditViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ContractType contractType = new ContractType("alsDitContractVerschijntIsErIetsFout", ManierAanmakenTicket.APPLICATIE, GedekteTijdstippen.ALTIJD, 0, 1, 0);
                    MapContractTypeEditViewModelToContractType(contractTypeEditViewModel, contractType);
                    _contractTypeRepository.Add(contractType);
                    _contractTypeRepository.SaveChanges();
                    TempData["message"] = $"Contract {contractTypeEditViewModel.Naam} werd succesvol aangepast.";
                }
                catch
                {
                    TempData["error"] = $"Contract {contractTypeEditViewModel.Naam} werd niet succesvol aangepast.";
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IsEdit"] = false;
            setViewDataEditView();
            return View(nameof(Edit), contractTypeEditViewModel);
        }

        #endregion
        #region Details
        public IActionResult Details(int id)
        {
            ContractType contractType = _contractTypeRepository.GetById(id);

            //Ophalen van aantal contracten onder dit type - MOET NOG GETEST WORDEN
            int aantalContracten = _contractRepository.GetAll()
                .Where(c => c.Type.ContractTypeId == contractType.ContractTypeId && c.Status == ContractStatus.LOPEND).Count();
            double totaalPrijs = aantalContracten * contractType.Prijs;
            ViewData["aantalContracten"] = $"{aantalContracten} ({totaalPrijs}EUR)";

            //Ophalen van aantal behandelde tickets van dit contractType
            List<Ticket> tickets = _ticketRepository.GetAll().Where(t => t.Contract != null).ToList();
            ViewData["aantalAfgehandeldeTickets"] = tickets.Where(t => t.Status == Status.AFGEHANDELD && t.Contract.Type == contractType).Count();

            //% behandelde tickets op tijd
            //Haal afgehandede tickets op onder het contractType
            tickets = tickets.Where(t => t.Status == Status.AFGEHANDELD && t.Contract.Type == contractType).ToList();
            double counter = 0;
            double totalTickets = tickets.Count();
            
            foreach (Ticket ticket in tickets) 
            {
                //Bepaal voor elk ticket de looptijd
                double afhandelTijdVanTicket = (double)((ticket.DatumAfgesloten - ticket.DatumAangemaakt).Value.TotalDays);
                //Indien de looptijd kleiner is dan de maximale doorlooptijd van het contracttype - counter++
                if (afhandelTijdVanTicket <= contractType.MaximaleAfhandeltijd)
                    counter++;
            }

            double afhandeltijd = (counter / totalTickets) * 100;

            ViewData["afhandeltijd"] = totalTickets == 0 ? "-" : afhandeltijd.ToString("0");

            if (contractType == null) return NotFound();
            return View(contractType);

        }


        #endregion

        #region hulpmethodes
        private void setViewDataEditView()
        {
            ViewData["ManierAanmakenTicket"] = GetManierAanmakenTicketSelectList();
            ViewData["GedekteTijdstippen"] = GetGedekteTijdstippenSelectList();
            ViewData["Statussen"] = GetStatussenSelectList();
        }
        private SelectList GetStatussenSelectList()
        {
            var statusList = Enum.GetValues(typeof(ContractTypeStatus)).Cast<ContractTypeStatus>().ToList();
            return new SelectList(statusList);
        }

        private SelectList GetManierAanmakenTicketSelectList()
        {
            var statusList = Enum.GetValues(typeof(ManierAanmakenTicket)).Cast<ManierAanmakenTicket>().ToList();
            return new SelectList(statusList);
        }

        private SelectList GetGedekteTijdstippenSelectList()
        {
            var statusList = Enum.GetValues(typeof(GedekteTijdstippen)).Cast<GedekteTijdstippen>().ToList();
            return new SelectList(statusList);
        }

        private void MapContractTypeEditViewModelToContractType(ContractTypeEditViewModel contractTypeEditViewModel, ContractType contract)
        {
            contract.Naam = contractTypeEditViewModel.Naam;
            contract.ManierAanmakenTicket = contractTypeEditViewModel.ManierAanmakenTicket;
            contract.GedekteTijdstippen = contractTypeEditViewModel.GedekteTijdstippen;
            contract.MaximaleAfhandeltijd = contractTypeEditViewModel.MaximaleAfhandeltijd;
            contract.MinimaleDoorlooptijd = contractTypeEditViewModel.MinimaleDoorlooptijd;
            contract.Prijs = contractTypeEditViewModel.Prijs;
            contract.Status = contractTypeEditViewModel.Status;
        }



        #endregion
    }
}
