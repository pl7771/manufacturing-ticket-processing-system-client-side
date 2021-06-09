using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticketing_System_TILE03.Models.Domain;

namespace Ticketing_System_TILE03.Models
{
    public interface ICompanyRepository
    {
        void Add(Company company);
        IEnumerable<Company> FindAllCompanies();
        SelectList FindAllCompaniesAsSelectList();
        void SaveChanges();
        Company GetByCompanyId(string companyId);
        Company GetByCompanyName(string companyName);
    }
}