using Microsoft.EntityFrameworkCore;
using System.Linq;
using Ticketing_System_TILE03.Models.Domain;
using Ticketing_System_TILE03.Models.Repositories;

namespace Ticketing_System_TILE03.Data.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Employee> _employees;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
            _employees = context.Employees;

        }

        public void Add(Employee newEmployee)
        {
            _employees.Add(newEmployee);
        }

        public Employee GetByApplicationUserId(string applicationUserId)
        {
            return _employees
            .Where(e => e.ApplicationUserId == applicationUserId)
            .First();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
