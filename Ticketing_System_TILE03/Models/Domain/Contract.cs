using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketing_System_TILE03.Models.Domain
{
    public class Contract
    {
        private DateTime _startDatum;
        private DateTime _eindDatum;

        public int ContractId { get; }
        public ContractType Type { get; set; }
        public ContractStatus Status { get; set; }
        public DateTime StartDatum
        {
            get => _startDatum; set
            {
                if (value >= EindDatum) throw new ArgumentException("begindatum moet voor startdatum liggen");
                _eindDatum = value;
            }
        }
        public DateTime EindDatum { get => _eindDatum ; set {
                if (value <= StartDatum) throw new ArgumentException("einddatum moet na startdatum liggen");
                _eindDatum = value;
            } }

        public Company Company { get; set; }

        public Boolean VervaltBinnenkort => this._eindDatum.AddDays(-30) <= DateTime.Now && this.Status != ContractStatus.BEEINDIGD;

        private Contract()
        {

        }

        public Contract(ContractType type, ContractStatus status, DateTime startDatum, DateTime eindDatum)
        {
            Type = type;
            Status = status;
            _startDatum = startDatum;
            EindDatum = eindDatum;
        }
    }
}
