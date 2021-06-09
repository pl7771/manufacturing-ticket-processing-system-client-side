using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticketing_System_TILE03.Models.Domain;

namespace Ticketing_System_TILE03.Models.ViewModels
{
    // Contract Type hoort bij het java deel, daarom heb ik verder geen annotaties of tests geschreven
    public class ContractTypeEditViewModel
    {
        public string Naam { get; set; }
        public ManierAanmakenTicket ManierAanmakenTicket { get; set; }
        public GedekteTijdstippen GedekteTijdstippen { get; set; }
        public double MaximaleAfhandeltijd { get; set; } //in uren
        public double MinimaleDoorlooptijd { get; set; } //in uren
        public double Prijs { get; set; }
        public ContractTypeStatus Status { get; set; }

        public ContractTypeEditViewModel()
        {

        }

        public ContractTypeEditViewModel(ContractType contractType)
        {
            Naam = contractType.Naam;
            ManierAanmakenTicket = contractType.ManierAanmakenTicket;
            GedekteTijdstippen = contractType.GedekteTijdstippen;
            MaximaleAfhandeltijd = contractType.MaximaleAfhandeltijd;
            MinimaleDoorlooptijd = contractType.MinimaleDoorlooptijd;
            Prijs = contractType.Prijs;
            Status = contractType.Status;

        }
    }
}
