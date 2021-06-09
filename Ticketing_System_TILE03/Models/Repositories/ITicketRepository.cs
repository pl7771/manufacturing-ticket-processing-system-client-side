using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticketing_System_TILE03.Models.Domain;

namespace Ticketing_System_TILE03.Models
{
    public interface ITicketRepository
    {
        IEnumerable<Ticket> GetAll();
        IEnumerable<Ticket> GetAllClosedTicketsFromPast6Months();
        IEnumerable<Ticket> GetAllNewTicketsFromPast6Months();
        IEnumerable<Ticket> GetByStatus(Status status); 
        IEnumerable<Ticket> GetAllActiveTickets();
        IEnumerable<Ticket> GetActiveTicketsByCompany(Company company);
        IEnumerable<Ticket> GetAllTicketsByCompany(Company company);
        IEnumerable<Ticket> GetAllClosedTicketsByCompanyFromPast6Months(Company company);
        IEnumerable<Ticket> GetAllNewTicketsByCompanyFromPast6Months(Company company);
        IEnumerable<Ticket> GetByStatusAndCompany(Status status, Company company);
        IEnumerable<Ticket> GetByStatusAndEmployee(Status status, Employee employee);
        IEnumerable<Ticket> GetActiveTicketsByEmployee(Employee employee);
        Ticket GetById(int ticketId);
        void Add(Ticket ticket); 
        void Delete(Ticket ticket);
        void SaveChanges();
        void AddAll(IEnumerable<Ticket> tickets);
        int GetChangeCount(DateTime lastSession);
        int GetChangeCountByCompany(DateTime lastSession, Company company);
    }
}