using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketing_System_TILE03.Models.Domain
{
    public interface IBijlageRepository
    {
        void Add(Bijlage bijlage);
        void Delete(Bijlage bijlage);
        Bijlage GetBijlage(string id);
        void SaveChanges();
    }
}
