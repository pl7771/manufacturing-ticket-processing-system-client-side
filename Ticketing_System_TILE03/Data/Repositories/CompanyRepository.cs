using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Ticketing_System_TILE03.Models;
using Ticketing_System_TILE03.Models.Domain;
namespace Ticketing_System_TILE03.Data.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {

        private readonly ApplicationDbContext _context;
        private readonly DbSet<Company> _companies;

        public CompanyRepository(ApplicationDbContext context)
        {
            _context = context;
            _companies = _context.Companies;
        }

        public void Add(Company company)
        {
            _companies.Add(company);
        }

        public IEnumerable<Company> FindAllCompanies()
        {
            return _companies;
        }

        public SelectList FindAllCompaniesAsSelectList()
        {
            IEnumerable<Company> companies = FindAllCompanies();
            List<string> companyNames = new List<string>();
            foreach(var company in companies)
            {
                companyNames.Add(company.Name);
            }
            return new SelectList(companyNames);
        }

        public Company GetByCompanyId(string companyId)
        {
            return _companies
                .Where(c => c.CompanyId == companyId)
                .Include(c => c.Contracts)
                .First();
        }

        public Company GetByCompanyName(string companyName)
        {
            return _companies
                .Where(c => c.Name == companyName)
                .Include(c => c.Contracts)
                .First();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}