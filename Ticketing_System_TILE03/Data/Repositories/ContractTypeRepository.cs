using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticketing_System_TILE03.Models.Domain;

namespace Ticketing_System_TILE03.Data.Repositories
{
    public class ContractTypeRepository : IContractTypeRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<ContractType> _contractTypes;

        public ContractTypeRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _contractTypes = dbContext.ContractTypes;

        }
        public void Add(ContractType contractType)
        {
            _contractTypes.Add(contractType);
        }

        public void AddAll(IEnumerable<ContractType> contractTypes)
        {
            _contractTypes.AddRangeAsync(contractTypes);
        }

        public void Delete(ContractType contractType)
        {
            _contractTypes.Remove(contractType);
        }

        public IEnumerable<ContractType> GetAll()
        {
            return _contractTypes.ToList();
        }

        public ContractType GetById(int contractTypeId)
        {
            return _contractTypes.First(c => c.ContractTypeId == contractTypeId);
        }

        public IEnumerable<ContractType> GetByStatus(ContractTypeStatus status)
        {
            return _contractTypes.Where(e => e.Status == status).OrderBy(e => e.Naam).AsNoTracking().ToList();
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        public SelectList GetAllActiveContractsAsSelectList()
        {
            IEnumerable<ContractType> contractTypes = _contractTypes
               .ToList();

            List<string> contractTypesForSelect = new List<string>();
            foreach (var contractType in contractTypes)
            {
                string c = contractType.ContractTypeId + "-" + contractType.Naam;
                contractTypesForSelect.Add(c);
            }
            return new SelectList(contractTypesForSelect);
        }
    }
}
