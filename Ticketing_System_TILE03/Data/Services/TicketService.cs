using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ticketing_System_TILE03.Models;
using Ticketing_System_TILE03.Models.Domain;
using Ticketing_System_TILE03.Models.Repositories;
using Ticketing_System_TILE03.Models.Services;
using Ticketing_System_TILE03.Models.ViewModels;

namespace Ticketing_System_TILE03.Data.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IBijlageRepository _bijlageRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IContractRepository _contractRepository;

        public TicketService(
            ITicketRepository ticketRepository,
            IClientRepository clientRepository,
            IBijlageRepository bijlageRepository,
            IEmployeeRepository employeeRepository,
            ICompanyRepository companyRepository,
            IContractRepository contractRepository)
        {
            _ticketRepository = ticketRepository;
            _clientRepository = clientRepository;
            _bijlageRepository = bijlageRepository;
            _employeeRepository = employeeRepository;
            _companyRepository = companyRepository;
            _contractRepository = contractRepository;
        }

        public void CreateTicketForUser(ApplicationUser user, TicketCreateViewModel ticketViewModel, List<string> userRoles)
        {
            Ticket createdTicket = new Ticket(ticketViewModel.DatumAangemaakt,
                                     ticketViewModel.Titel,
                                     ticketViewModel.Omschrijving,
                                     Status.AANGEMAAKT
                                    );

            createdTicket.Opmerkingen = ticketViewModel.Opmerkingen == null 
                ? "[" + DateTime.Now.ToString("dd/MM/yyyy hh:mm") + "] " + "Geen opmerking bij creatie ticket" + "|a|" 
                : string.Format("[{0}] {1} {2}: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm"), user.FirstName, user.LastName) + ticketViewModel.Opmerkingen + "|a|";
            createdTicket.ImageDescription = ticketViewModel.ImageDescription;
            createdTicket.DatumGewijzigd = DateTime.Now;



            #region BijlageToevoegen
            if (ticketViewModel.MyFile != null)
            { //Als MyFile niet leeg is, maak dan bijlage aan
                var fileName = Path.GetFileName(ticketViewModel.MyFile.FileName);
                var fileExtension = Path.GetExtension(fileName);
                var newFileName = string.Concat(Convert.ToString(Guid.NewGuid()), fileExtension);

                var objfiles = new Bijlage()
                {
                    Name = newFileName,
                    FileType = fileExtension,
                };

                using (var target = new MemoryStream())
                {
                    ticketViewModel.MyFile.CopyTo(target);
                    objfiles.DataFiles = target.ToArray();
                }

                createdTicket.Bijlage = objfiles;
            }
            #endregion


            #region BedrijfToevoegen
            if (ticketViewModel.Company != null)
            {
                if (userRoles.Contains(Roles.SupportManager.ToString()))
                {
                    Company foundCompany = _companyRepository.GetByCompanyName(ticketViewModel.Company);
                    createdTicket.Company = foundCompany;

                }
            }
            else
            {
                Client foundClient = _clientRepository.GetByApplicationUserId(user.Id);
                createdTicket.Company = foundClient.Company;
            }

            #endregion


            #region ContractToevoegen
            if (!string.IsNullOrEmpty(ticketViewModel.Contract))
            {
                string[] contractGegevens = ticketViewModel.Contract.Split('-');
                if (contractGegevens.Length > 2)
                {
                    int contractId = int.Parse(contractGegevens[0]);
                    Contract toeTevoegenContract = _contractRepository.GetById(contractId);
                    createdTicket.Contract = toeTevoegenContract;
                }
            }
            #endregion
            _ticketRepository.Add(createdTicket);
            _ticketRepository.SaveChanges();
        }

        public void DeleteTicket(int ticketId)
        {
            Ticket ticket = _ticketRepository.GetById(ticketId);
            _ticketRepository.Delete(ticket);
            _ticketRepository.SaveChanges();
        }

        public Ticket FindTicket(int ticketId)
        {
            return _ticketRepository.GetById(ticketId);
        }

        public IEnumerable<Ticket> FindTicketsByStatusAndUserIdAndRole(Status? status, string userId, List<string> userRoles)
        {
            if (userRoles.Contains(Roles.Client.ToString()))
            {
                Client foundClient = _clientRepository.GetByApplicationUserId(userId);
                Company clientsCompany = _companyRepository.GetByCompanyId(foundClient.CompanyId);

                if (status.HasValue)
                {
                    return _ticketRepository.GetByStatusAndCompany(status.Value, clientsCompany);
                }
                else
                {
                    return _ticketRepository.GetActiveTicketsByCompany(clientsCompany);
                }
            }

            if (userRoles.Contains(Roles.Technician.ToString()))
            {
                Employee foundEmployee = _employeeRepository.GetByApplicationUserId(userId);

                if (status.HasValue)
                {
                    return _ticketRepository.GetByStatusAndEmployee(status.Value, foundEmployee);
                }
                else
                {
                    return _ticketRepository.GetActiveTicketsByEmployee(foundEmployee);
                }
            }

            if (status.HasValue)
            {
                return _ticketRepository.GetByStatus(status.Value);
            }
            else
            {
                return _ticketRepository.GetAllActiveTickets();
            }

        }

        public void UpdateTicket(int ticketId, TicketEditViewModel ticketEditViewModel, ApplicationUser user)
        {
            //Overschrijven wijzigingen van viewmodel naar Ticket
            Ticket foundTicket = _ticketRepository.GetById(ticketId);

            if (ticketEditViewModel.MyFile != null)
            {
                if (foundTicket.Bijlage != null)
                {
                    _bijlageRepository.Delete(foundTicket.Bijlage);
                }
                LinkBijlageToTicket(ticketEditViewModel, foundTicket);
            }

            foundTicket.UpdateTicket(ticketEditViewModel);
            if (ticketEditViewModel.Opmerkingen != null)
            {
                foundTicket.Opmerkingen += string.Format("[{0}] {1} {2}: {3}{4}", DateTime.Now.ToString("dd/MM/yyyy hh:mm"), user.FirstName, user.LastName, ticketEditViewModel.Opmerkingen, "|a|");
            }
            _bijlageRepository.SaveChanges();
        }

        private void LinkBijlageToTicket(TicketEditViewModel ticketEditViewModel, Ticket foundTicket)
        {
            //Als MyFile niet leeg is, maak dan bijlage aan
            if (ticketEditViewModel.MyFile != null)
            {
                var fileName = Path.GetFileName(ticketEditViewModel.MyFile.FileName);
                var fileExtension = Path.GetExtension(fileName);
                var newFileName = String.Concat(Convert.ToString(Guid.NewGuid()), fileExtension);

                var objfiles = new Bijlage()
                {
                    Name = newFileName,
                    FileType = fileExtension,
                };

                using (var target = new MemoryStream())
                {
                    ticketEditViewModel.MyFile.CopyTo(target);
                    objfiles.DataFiles = target.ToArray();
                }

                foundTicket.Bijlage = objfiles;
            }
        }
    }
}
