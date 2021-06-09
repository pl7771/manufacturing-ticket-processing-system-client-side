using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Ticketing_System_TILE03.Models;
using Ticketing_System_TILE03.Models.Domain;
namespace Ticketing_System_TILE03.Data.Repositories
{
    public class ClientRepository : IClientRepository
    {

        private readonly ApplicationDbContext _context;
        private readonly DbSet<Client> _clients;

        public ClientRepository(ApplicationDbContext context)
        {
            _context = context;
            _clients = _context.Clients;
        }

        public Client GetByApplicationUserId(string applicationUserId)
        {
            return _clients
                .Where(c => c.ApplicationUserId == applicationUserId)
                .Include(c => c.Company)
                .First();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}