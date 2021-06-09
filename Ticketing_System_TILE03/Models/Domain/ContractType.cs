using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketing_System_TILE03.Models.Domain
{// Contract Type hoort bij het java deel, daarom heb ik verder geen annotaties of tests geschreven
    public class ContractType
    {
        private string _naam;

        #region properties
        public int ContractTypeId { get; set; }
        public string Naam { get => _naam; set
            {
                if (value == string.Empty)
                    throw new ArgumentException(nameof(Naam), "Naam mag niet leeg zijn");
                _naam = value;
            }
        }
        public ManierAanmakenTicket ManierAanmakenTicket { get; set; }
        public GedekteTijdstippen GedekteTijdstippen { get; set; }
        public double MaximaleAfhandeltijd { get; set; } //in uren
        public double MinimaleDoorlooptijd { get; set; } //in uren
        public double Prijs { get; set; }
        public int aantalContracten { get; set; }
        public ContractTypeStatus Status {get;set;}
        #endregion

        public ContractType(string naam, ManierAanmakenTicket manierAanmakenTicket, GedekteTijdstippen gedekteTijdstippen,
            double maximaleAfhandeltijd, double minimaleDoorlooptijd, double prijs)
        {
            Naam = naam;
            ManierAanmakenTicket = manierAanmakenTicket;
            GedekteTijdstippen = gedekteTijdstippen;
            MaximaleAfhandeltijd = maximaleAfhandeltijd;
            MinimaleDoorlooptijd = minimaleDoorlooptijd;
            Prijs = prijs;
            Status = ContractTypeStatus.ACTIEF;

        }
    }
}
