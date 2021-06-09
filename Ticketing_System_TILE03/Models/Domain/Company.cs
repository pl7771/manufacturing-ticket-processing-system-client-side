using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketing_System_TILE03.Models.Domain
{
    public class Company
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string CompanyId { get; set; }
        public string Name { get; set; }
        public string Adress { get; set; }
        public string PhoneNumber { get; set; }

        [InverseProperty("Company")]
        public ICollection<Contract> Contracts { get; set; }

        public Company(string name, string adress, string phoneNumber)
        {
            Name = name;
            Adress = adress;
            PhoneNumber = phoneNumber;
        }

        public void AddContract(Contract contract)
        {
            /*if (contract != null && Contracts.Any(c => c.ContractId == contract.ContractId))
                throw new ArgumentException($"Client {ClientId} heeft reeds een contract met id {contract.ContractId}");*/
            if (contract == null) throw new ArgumentException($"contract is null");
            Contracts.Add(contract);
        }
    }
}
