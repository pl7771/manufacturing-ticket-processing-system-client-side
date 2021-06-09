using System;
using Ticketing_System_TILE03.Models.Domain;
using Ticketing_System_TILE03.Models.ViewModels;

namespace Ticketing_System_TILE03.Models.Domain
{
    public class Ticket
    {

        private string _titel;
        private string _omschrijving;
        private DateTime _datumAangemaakt;
        private DateTime? _datumAfgesloten;
        private DateTime _datumGewijzigd;

        #region Properties
        public int TicketId { get; set; }

        //Titel mag niet langer zijn dan 100 karakters
        public string Titel
        {
            get => _titel;
            set
            {
                if (value == string.Empty || value.Length > 100)
                    throw new ArgumentException(nameof(Titel), "Titel mag niet leeg zijn of langer dan 100 karakters");
                _titel = value;
            }
        }
        public string Omschrijving
        {
            get => _omschrijving;
            set
            {
                if (value == string.Empty)
                    throw new ArgumentException(nameof(Omschrijving), "Omschrijving mag niet leeg zijn");
                _omschrijving = value;
            }
        }
        public string Opmerkingen { get; set; }
        public bool? IsGewijzigd { get; set; }
        public DateTime DatumAangemaakt
        {
            get => _datumAangemaakt;
            set
            {
                if (value > DateTime.Now) throw new ArgumentException("Datum aanmaak ticket mag niet in de toekomst liggen");
                _datumAangemaakt = value;
            }
        }
        public DateTime? DatumAfgesloten
        {
            get => _datumAfgesloten;
            set
            {
                if (value < this.DatumAangemaakt) throw new ArgumentException("Ongeldige afsluitdatum");
                if (value != null)
                {
                    _datumAfgesloten = (DateTime)value;
                }
                else _datumAfgesloten = null;
            }

        }
        public DateTime DatumGewijzigd
        {
            get => _datumGewijzigd;
            set
            {
                if (value > DateTime.Now) throw new ArgumentException("Datum wijziging moet het huidige tijdstip zijn");
                _datumGewijzigd = value;
            }

        }


        public Status Status { get; set; }
        public int AantalWijzigingen { get; set; } = 0;
        public Company Company { get; set; }
        public Employee Employee { get; set; }
        public Bijlage Bijlage { get; set; }
        public Contract Contract { get; set; }

        public string ImageDescription { get; set; }
        #endregion

        #region Constructors
        public Ticket(DateTime datumAangemaakt, string titel, string omschrijving)
        {
            DatumAangemaakt = datumAangemaakt;
            DatumAfgesloten = null;

            Titel = titel;
            Omschrijving = omschrijving;
         
        }

        public Ticket(DateTime datumAangemaakt, string titel, string omschrijving, Status status)
        {
            DatumAangemaakt = datumAangemaakt;
            DatumAfgesloten = null;
            Titel = titel;
            Omschrijving = omschrijving;
            Status = status;
         
            IsGewijzigd = false;
        }
        #endregion

        #region Methods

        public bool KanWijzigen()
        {
            if (Status == Status.AANGEMAAKT || Status == Status.IN_BEHANDELING)
                return true;
            else return false;
        }

        public void UpdateTicket(TicketEditViewModel ticketEditViewModel)
        {

            if (ticketUpdated(this, ticketEditViewModel)) {
                DatumGewijzigd = DateTime.Now;
                AantalWijzigingen++;
                IsGewijzigd = true;
            }

            //titel, omschrijving, opmerkingen, type, status
            Titel = ticketEditViewModel.Titel;
            Omschrijving = ticketEditViewModel.Omschrijving;
            //Opmerkingen = ticketEditViewModel.Opmerkingen;
           
            if (ticketEditViewModel.Status == Status.AFGEHANDELD)
            {
                DatumAfgesloten = DateTime.Now;
            }

            Status = ticketEditViewModel.Status;
            ImageDescription = ticketEditViewModel.ImageDescription;

        }

        private bool ticketUpdated(Ticket ticket, TicketEditViewModel tevm)
        {
            return
                 tevm.Titel != ticket.Titel ||
                 tevm.Omschrijving != ticket.Omschrijving ||
                 tevm.Opmerkingen != ticket.Opmerkingen ||
                 tevm.Status != ticket.Status ||
                 tevm.ImageDescription != ticket.ImageDescription ||
                 tevm.MyFile != null;              
        }
        #endregion
    }
}
