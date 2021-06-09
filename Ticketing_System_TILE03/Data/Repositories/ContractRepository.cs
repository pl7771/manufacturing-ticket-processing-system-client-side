using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Ticketing_System_TILE03.Models.Domain;

namespace Ticketing_System_TILE03.Data.Repositories
{
    public class ContractRepository : IContractRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<Contract> _contracts;

        public ContractRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _contracts = dbContext.Contracts;

        }
        public void Add(Contract contract)
        {
            _contracts.Add(contract);
        }

        public void AddAll(IEnumerable<Contract> contracts)
        {
            _contracts.AddRangeAsync(contracts);
        }

        public void Delete(Contract contract)
        {
            _contracts.Remove(contract);
        }

        public IEnumerable<Contract> GetAll()
        {
            return _contracts.Include(c => c.Type).ToList();
        }

        public SelectList GetAllAsSelectList()
        {
             IEnumerable<Contract> contracts = _contracts
                .Include(c => c.Type)
                .Include(c => c.Company)
                .ToList();

            List<string> contractsForSelect = new List<string>();
            foreach(var contract in contracts)
            {
                string c = contract.ContractId + "-" + contract.Type.Naam + "-" + contract.Company.Name;
                contractsForSelect.Add(c);
            }
            return new SelectList(contractsForSelect);
        }

        public Contract GetById(int contractId)
        {
            return _contracts
                .Include(contract => contract.Type)
                .First(c => c.ContractId == contractId);
        }

        public IEnumerable<Contract> GetByStatus(ContractStatus status)
        {
            return _contracts.Where(e => e.Status == status).OrderBy(e => e.StartDatum).AsNoTracking().ToList();
        }

        public IEnumerable<Contract> GetByStatusAndCompany(ContractStatus status, Company company)
        {
            return _contracts
                .Where(c => c.Company == company)
                .Where(e => e.Status == status)
                .Include(c => c.Type)
                .OrderBy(e => e.StartDatum).AsNoTracking().ToList();
        }

        public IEnumerable<Contract> GetActiveContractsOfCompany(Company company)
        {
            return _contracts
                .Where(c => c.Company == company)
                .Where(c => c.Status == ContractStatus.IN_BEHANDELING || c.Status == ContractStatus.LOPEND)
                .Include(c => c.Type)
                .ToList();
        }

        public SelectList GetActiveContractsOfCompanyAsSelectList(Company company)
        {
            IEnumerable<Contract> contracts = _contracts
                .Where(c => c.Company == company)
                .Where(c => c.Status == ContractStatus.IN_BEHANDELING || c.Status == ContractStatus.LOPEND)
                .Include(c => c.Type)
                .Include(c => c.Company)
                .ToList();

            List<string> contractsForSelect = new List<string>();
            foreach (var contract in contracts)
            {
                string c = contract.ContractId + "-" + contract.Type.Naam + "-" + contract.Company.Name;
                contractsForSelect.Add(c);
            }
            return new SelectList(contractsForSelect);      
        }

        public IEnumerable<Contract> GetAllContractsOfCompany(Company company)
        {
            return _contracts
                .Where(c => c.Company == company)
                .ToList();
        }

        public Boolean ExpiresThisMonth(Company company)
        {
            return _contracts.Where(c => c.Company == company).ToList().Any(c => c.VervaltBinnenkort == true && c.Status != ContractStatus.BEEINDIGD);
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
