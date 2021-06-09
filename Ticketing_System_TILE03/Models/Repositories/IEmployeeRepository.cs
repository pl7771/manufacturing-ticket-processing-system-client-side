using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticketing_System_TILE03.Models.Domain;

namespace Ticketing_System_TILE03.Models.Repositories
{
    public interface IEmployeeRepository
    {
        Employee GetByApplicationUserId(string applicationUserId);
        void Add(Employee newEmployee);
        void SaveChanges();
    }
}
