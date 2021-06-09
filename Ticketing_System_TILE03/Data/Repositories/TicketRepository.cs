using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Ticketing_System_TILE03.Models;
using Ticketing_System_TILE03.Models.Domain;
namespace Ticketing_System_TILE03.Data.Repositories
{
    public class TicketRepository : ITicketRepository
    {

        private readonly ApplicationDbContext _context;
        private readonly DbSet<Ticket> _tickets;
        //test test 
        public TicketRepository(ApplicationDbContext context)
        {
            _context = context;
            _tickets = _context.Tickets;
        }

        public void Add(Ticket ticket)
        {
            _tickets.Add(ticket);
        }

        public void AddAll(IEnumerable<Ticket> tickets)
        {
            _tickets.AddRangeAsync(tickets);
        }

        public void Delete(Ticket ticket)
        {
            _tickets.Remove(ticket);
        }

        public IEnumerable<Ticket> GetByStatus(Status status)
        {
            return _tickets
                .Where(e => e.Status == status)
                .Include(t => t.Contract.Type)
                .ToList();
        }

        public IEnumerable<Ticket> GetAll() //kweet niet of deze methode nuttig is aangezien een user kan enkel zijn tickets zien -> een manager gaat ze wel allemaal moeten kunnen oproepen
        {
            return _tickets.Include(t => t.Contract.Type).ToList();
            //Gaan nog de bijhorende users en bijlagen moeten koppelen later
            //dan kunnen we de resterende methodes onderaan ook vervolledigen
        }

        public IEnumerable<Ticket> GetAllChanged()
        {
            return _tickets.Where(t => t.IsGewijzigd == true);

        }

        public IEnumerable<Ticket> GetAllClosedTicketsFromPast6Months() 
        {
            return _tickets.Where(t => t.DatumAfgesloten >= DateTime.Now.AddMonths(-6) && t.Status == Status.AFGEHANDELD).Include(t => t.Contract.Type).ToList();
           
        }

        public IEnumerable<Ticket> GetAllNewTicketsFromPast6Months()
        {
            return _tickets.Where(t => t.DatumAangemaakt >= DateTime.Now.AddMonths(-6)).Include(t => t.Contract.Type).ToList();

        }

        public Ticket GetById(int ticketId)
        {
            return _tickets
                .Include(e => e.Bijlage)
                .Include(t => t.Employee.ApplicationUser)
                .Include(t => t.Company)
                .Include(t => t.Contract.Type)
                .First(t => t.TicketId == ticketId);
        }

        public IEnumerable<Ticket> GetAllActiveTickets()
        {
            return _tickets.Where(e => e.Status == Status.AANGEMAAKT || e.Status == Status.IN_BEHANDELING)
                .Include(t => t.Contract.Type)
                .ToList();
        }

        public IEnumerable<Ticket> GetActiveTicketsByCompany(Company company)
        {
            return _tickets
                .Where(t => t.Company == company)
                .Where(e => e.Status == Status.AANGEMAAKT || e.Status == Status.IN_BEHANDELING)
                .OrderBy(e => e.DatumAangemaakt)
                .Include(t => t.Company)
                .Include(t => t.Contract.Type)
                .ToList();
        }

        public IEnumerable<Ticket> GetAllTicketsByCompany(Company company)
        {
            return _tickets.Where(t => t.Company == company)
                .Include(t => t.Company)
                .Include(t => t.Contract.Type).ToList();
        }

        public IEnumerable<Ticket> GetAllClosedTicketsByCompanyFromPast6Months(Company company)
        {
            return _tickets.Where(t => t.Company == company && t.DatumAfgesloten >= DateTime.Now.AddMonths(-6) && t.Status == Status.AFGEHANDELD && t.Contract != null)
                .Include(t => t.Company)
                .Include(t => t.Contract.Type).ToList();

        }

        public IEnumerable<Ticket> GetAllNewTicketsByCompanyFromPast6Months(Company company)
        {
            return _tickets.Where(t => t.Company == company && t.DatumAangemaakt >= DateTime.Now.AddMonths(-6) && t.Contract != null)
                .Include(t => t.Company)
                .Include(t => t.Contract.Type).ToList();

        }

        public IEnumerable<Ticket> GetByStatusAndCompany(Status status, Company company)
        {
            return _tickets
                .Where(t => t.Company == company)
                .Where(t => t.Status == status)
                .Include(t => t.Company)
                .Include(t => t.Contract.Type)
                .ToList();
        }

        public IEnumerable<Ticket> GetByStatusAndEmployee(Status status, Employee employee)
        {
            return _tickets
                .Where(t => t.Employee == employee)
                .Where(t => t.Status == status)
                .Include(t => t.Company)
                .Include(t => t.Contract.Type)
                .ToList();
        }

        public IEnumerable<Ticket> GetActiveTicketsByEmployee(Employee employee)
        {
            return _tickets
                .Where(t => t.Employee == employee)
                .Where(e => e.Status == Status.AANGEMAAKT || e.Status == Status.IN_BEHANDELING)
                .Include(t => t.Company)
                .Include(t => t.Contract.Type)
                .OrderBy(e => e.DatumAangemaakt)
                .ToList();
        }

        public int GetChangeCount(DateTime lastSession)
        {
            return _tickets.Where(t => t.DatumGewijzigd > lastSession).Count();
        }

        public int GetChangeCountByCompany(DateTime lastSession, Company company)
        {
            return _tickets.Where(t => t.Company == company && t.DatumGewijzigd > lastSession).Count();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

    }
}