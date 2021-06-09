using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticketing_System_TILE03.Models.Domain;

namespace Ticketing_System_TILE03.Data.Repositories
{
    public class BijlageRepository : IBijlageRepository
    {

        private readonly ApplicationDbContext _context;
        private DbSet<Bijlage> _bijlages;

        public BijlageRepository(ApplicationDbContext context)
        {
            this._context = context;
            _bijlages = _context.Bijlages;
        }

        public void Add(Bijlage bijlage)
        {
            _bijlages.Add(bijlage);
        }

        

        
        public void Delete(Bijlage bijlage)
        {
            _bijlages.Remove(bijlage);
        }

        public Bijlage GetBijlage(string id)
        {
            return _bijlages.FirstOrDefault(e => e.BijlageId == id);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
