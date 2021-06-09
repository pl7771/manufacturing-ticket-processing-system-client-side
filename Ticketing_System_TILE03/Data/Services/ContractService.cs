using System;
using System.Collections.Generic;
using System.IO;
using Ticketing_System_TILE03.Models;
using Ticketing_System_TILE03.Models.Domain;
using Ticketing_System_TILE03.Models.Services;
using Ticketing_System_TILE03.Models.ViewModels;
using System.Drawing;

namespace Ticketing_System_TILE03.Data.Services
{
    public class ContractService : IContractService
    {
        private readonly IContractRepository _contractRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IContractTypeRepository _contractTypeRepository;
        
        public ContractService(
            IContractRepository contractRepository,
            ICompanyRepository companyRepository,
            IContractTypeRepository contractTypeRepository)
        {
            _contractRepository = contractRepository;
            _companyRepository = companyRepository;
            _contractTypeRepository = contractTypeRepository;
        }

        public void AddContractToCompany(Contract createdContract, string companyId)
        {
            Company foundCompany = _companyRepository.GetByCompanyId(companyId);
            foundCompany.AddContract(createdContract);
            _companyRepository.SaveChanges();
        }

        public IEnumerable<ContractType> FindActiveContractTypes()
        {
            return _contractTypeRepository.GetByStatus(ContractTypeStatus.ACTIEF);
        }

        public IEnumerable<Contract> FindAllByStatus(ContractStatus? nullableStatus, string companyId)
        {
            Company company = _companyRepository.GetByCompanyId(companyId);
            if (nullableStatus.HasValue)
            {
                return _contractRepository.GetByStatusAndCompany(nullableStatus.Value, company);
            } else
            {
                return _contractRepository.GetActiveContractsOfCompany(company);
            }
        }

        public Contract FindByContractId(int contractId)
        {
            return _contractRepository.GetById(contractId);
        }
    }
}
