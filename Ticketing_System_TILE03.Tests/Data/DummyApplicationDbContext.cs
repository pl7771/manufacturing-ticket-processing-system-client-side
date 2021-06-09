using System;
using System.Collections.Generic;
using System.Linq;
using Ticketing_System_TILE03.Models.Domain;

namespace Ticketing_System_TILE03.Tests.Data
{
    public class DummyApplicationDbContext
    {
        public IList<Ticket> Tickets { get; }
        public IList<Contract> Contracts { get; }
        public IList<ContractType> ContractTypes { get; }
        public IList<ContractType> ActieveContractTypes { get; }
        public Ticket SingleTicket { get; }
        public Client Client { get; }
        public Company Company { get; }

        public DummyApplicationDbContext()
        {
            #region Tickets
            Tickets = new List<Ticket>();
            Company = new Company("Coca-cola", "straat", "12345678");
            SingleTicket = new Ticket(DateTime.Now, "Het is de ticket9", "Er zit al de omschrijving9", Status.AANGEMAAKT) { TicketId = 7 };
            SingleTicket.Company = Company;
            Tickets.Add(new Ticket(DateTime.Now, "Het is de ticket1", "Er zit al de omschrijving1", Status.AANGEMAAKT) {Company = Company });
            Tickets.Add(new Ticket(DateTime.Now, "Het is de ticket2", "Er zit al de omschrijving2", Status.AANGEMAAKT) { Company = Company });
            Tickets.Add(new Ticket(DateTime.Now, "Het is de ticket3", "Er zit al de omschrijving3", Status.AANGEMAAKT) { Company = Company });
            Tickets.Add(new Ticket(DateTime.Now, "Het is de ticket4", "Er zit al de omschrijving4", Status.AFGEHANDELD) { Company = Company });
            Tickets.Add(new Ticket(DateTime.Now, "Het is de ticket5", "Er zit al de omschrijving5", Status.AFGEHANDELD) { Company = Company });
            Tickets.Add(new Ticket(DateTime.Now, "Het is de ticket6", "Er zit al de omschrijving6", Status.GEANNULEERD) { Company = Company });
            Tickets.Add(new Ticket(DateTime.Now, "Het is de ticket7", "Er zit al de omschrijving7", Status.GEANNULEERD) { Company = Company });
            Tickets.Add(new Ticket(DateTime.Now, "Het is de ticket8", "Er zit al de omschrijving8", Status.IN_BEHANDELING) { Company = Company });
            #endregion
            #region ContractTypes
            ContractType best = new ContractType("Contract1", ManierAanmakenTicket.APPLICATIE, GedekteTijdstippen.ALTIJD,
                   new TimeSpan(1, 0, 0).TotalHours, TimeSpan.Zero.TotalHours, 1000);
            ContractType worst = new ContractType("Contract2", ManierAanmakenTicket.TELEFOON, GedekteTijdstippen.ENKELWEEKDAGEN,
                new TimeSpan(7, 0, 0, 0).TotalHours, new TimeSpan(2, 0, 0, 0).TotalHours, 5);
            ContractType normal = new ContractType("Contract3", ManierAanmakenTicket.EMAIL, GedekteTijdstippen.ENKELWEEKDAGEN,
                new TimeSpan(1, 0, 0, 0).TotalHours, new TimeSpan(3, 0, 0).TotalHours, 100);
            ContractType oudContract = new ContractType("Contract4", ManierAanmakenTicket.EMAIL, GedekteTijdstippen.ENKELWEEKDAGEN,
                new TimeSpan(4, 0, 0, 0).TotalHours, new TimeSpan(1, 0, 0).TotalHours, 100)
            { Status = ContractTypeStatus.NIET_ACTIEF };
            ContractType oudContract2 = new ContractType("Contract5", ManierAanmakenTicket.EMAIL, GedekteTijdstippen.ENKELWEEKDAGEN,
                new TimeSpan(4, 0, 0, 0).TotalHours, new TimeSpan(1, 0, 0).TotalHours, 100)
            { Status = ContractTypeStatus.NIET_ACTIEF };
            ContractTypes = new List<ContractType>();
            ContractTypes.Add(best); ContractTypes.Add(worst); ContractTypes.Add(normal); ContractTypes.Add(oudContract); ContractTypes.Add(oudContract2);
            ActieveContractTypes = new List<ContractType>();
            ActieveContractTypes.Add(best); ActieveContractTypes.Add(worst); ActieveContractTypes.Add(normal);
            #endregion
            #region Contracts
            Contracts = new List<Contract>();
            DateTime startdatum1 = new DateTime(2020, 12, 31, 15, 30, 30);
            DateTime einddatum1 = startdatum1.AddDays(30);

            Contracts.Add(new Contract(best, ContractStatus.LOPEND, startdatum1, einddatum1));
            Contracts.Add(new Contract(worst, ContractStatus.LOPEND, startdatum1.AddDays(2), einddatum1.AddDays(2)));
            Contracts.Add(new Contract(normal, ContractStatus.IN_BEHANDELING, startdatum1.AddDays(6), einddatum1.AddDays(6)));
            Contracts.Add(new Contract(oudContract, ContractStatus.BEEINDIGD, startdatum1.AddDays(-50), einddatum1.AddDays(-50)));
            Contracts.Add(new Contract(oudContract2, ContractStatus.BEEINDIGD, startdatum1.AddDays(-70), einddatum1.AddDays(-70)));
            #endregion
        }

        public Client GetTestClient()
        {
            return new Client() {  ApplicationUserId = "123" ,Company = GetTestCompany() };
        }

        public IEnumerable<Contract> GetDummyContractsFor(ContractStatus? contractStatus)
        {
            if (contractStatus == ContractStatus.BEEINDIGD)
            {
                return Contracts.Where(c => c.Status == ContractStatus.BEEINDIGD);
            }

            if (contractStatus == ContractStatus.LOPEND)
            {
                return Contracts.Where(c => c.Status == ContractStatus.LOPEND);
            }
            if (contractStatus == ContractStatus.IN_BEHANDELING)
            {
                return Contracts.Where(c => c.Status == ContractStatus.IN_BEHANDELING);
            }
            return Contracts.Where(c => c.Status == ContractStatus.IN_BEHANDELING || c.Status == ContractStatus.LOPEND);

        }

        public Contract GetTestContract()
        {
            DateTime startdatum1 = new DateTime(2020, 12, 31, 15, 30, 30);
            DateTime einddatum1 = startdatum1.AddDays(30);
            ContractType best = new ContractType("Contract1", ManierAanmakenTicket.APPLICATIE, GedekteTijdstippen.ALTIJD, new TimeSpan(1, 0, 0).TotalHours, TimeSpan.Zero.TotalHours, 1000);
            return new Contract(best, ContractStatus.LOPEND, startdatum1, einddatum1);
        }

        public Company GetTestCompany()
        {
            return new Company("Coca-cola", "FooBarStraat 4 9000 Gent", "0044556699")
            {
                CompanyId = "123",
                Contracts = new List<Contract>()
            };
        }
    }
}
