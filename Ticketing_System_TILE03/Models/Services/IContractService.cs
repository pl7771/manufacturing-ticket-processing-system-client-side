using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticketing_System_TILE03.Models.Domain;

namespace Ticketing_System_TILE03.Models.Services
{
    public interface IContractService
    {
        void AddContractToCompany(Contract createdContract, string companyId);
        IEnumerable<Contract> FindAllByStatus(ContractStatus? nullableStatus, string companyId);
        Contract FindByContractId(int contractId);
        IEnumerable<ContractType> FindActiveContractTypes();
    }
}
