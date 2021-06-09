using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticketing_System_TILE03.Models.Domain;

namespace Ticketing_System_TILE03.Models
{
    public interface IClientRepository
    {
        Client GetByApplicationUserId(string applicationUserId);
        void SaveChanges();
    }
}