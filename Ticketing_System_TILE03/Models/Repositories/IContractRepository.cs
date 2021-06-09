using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace Ticketing_System_TILE03.Models.Domain
{
    public interface IContractRepository
    {
        IEnumerable<Contract> GetAll();
        SelectList GetAllAsSelectList();
        Contract GetById(int contractId);
        IEnumerable<Contract> GetByStatus(ContractStatus status);
        IEnumerable<Contract> GetByStatusAndCompany(ContractStatus status, Company company);
        IEnumerable<Contract> GetActiveContractsOfCompany(Company company);
        IEnumerable<Contract> GetAllContractsOfCompany(Company company);
        SelectList GetActiveContractsOfCompanyAsSelectList(Company company);
        public Boolean ExpiresThisMonth(Company company);
        void Add(Contract contract);
        void AddAll(IEnumerable<Contract> contracts);
        void Delete(Contract contract);
        void SaveChanges();
    }
}
