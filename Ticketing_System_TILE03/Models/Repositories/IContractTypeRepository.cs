using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketing_System_TILE03.Models.Domain
{
    public interface IContractTypeRepository
    {
        IEnumerable<ContractType> GetAll();
        ContractType GetById(int contractTypeId);
        IEnumerable<ContractType> GetByStatus(ContractTypeStatus status);
        void Add(ContractType contractType);
        void AddAll(IEnumerable<ContractType> contractTypes);
        void Delete(ContractType contractType);
        public SelectList GetAllActiveContractsAsSelectList();
        void SaveChanges();
    }
}
