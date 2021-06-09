using System;
using System.Collections.Generic;
using Ticketing_System_TILE03.Models.Domain;
using Ticketing_System_TILE03.Tests.Data;
using Xunit;

namespace Ticketing_System_TILE03.Tests.Models.Domain
{
    public class TicketTest
    {

        private DummyApplicationDbContext context;
        private readonly Ticket ticket;
        private readonly IEnumerable<Ticket> tickets;


        public TicketTest()
        {
            context = new DummyApplicationDbContext();
            tickets = context.Tickets;
            ticket = context.SingleTicket;
        }

        [Fact]
        public void NewTicket_MandatoryConstructor_EmptyTitle_Fails()
        {
            Assert.Throws<ArgumentException>(() => new Ticket(DateTime.Now, string.Empty, "Testomschrijving"));
        }

        [Fact]
        public void NewTicket_MandatoryConstructor_TitleLongerThan100Characters_Fails() {

            char[] charArray = new char[101];
            Array.Fill(charArray, 'a');
            String TeLangeString = new String(charArray);

            

            Assert.Throws<ArgumentException>(() => new Ticket(DateTime.Now, TeLangeString, "test"));
        }

        [Fact]
        public void NewTicket_OptionalConstructor_EmptyTitle_Fails()
        {
            Assert.Throws<ArgumentException>(() => new Ticket(DateTime.Now, string.Empty, "testOmschrijving", Status.AANGEMAAKT));
        }

        [Fact]
        public void NewTicket_EmptyOmschrijving_Fails()
        {
            Assert.Throws<ArgumentException>(() => new Ticket(DateTime.Now, "testTitel", string.Empty, Status.AANGEMAAKT));
        }

        [Theory]
        [InlineData(Status.AANGEMAAKT, true)]
        [InlineData(Status.IN_BEHANDELING, true)]
        [InlineData(Status.AFGEHANDELD, false)]
        [InlineData(Status.GEANNULEERD, false)]
        public void KanWijzigen(Status status, bool expected)
        {
            ticket.Status = status;
            bool result = ticket.KanWijzigen();
            Assert.Equal(expected, result);
        }

        [Fact]
        public void NewTicket_IsGewijzigd_Staat_Op_False()
        {
            Assert.False(ticket.IsGewijzigd);
        }

        [Fact]
        public void New_Ticket_Datum_in_toekomst_werpt_exception()
        {
            var dateString = "5/1/2023 8:30:52 AM";
            DateTime date1 = DateTime.Parse(dateString,
                                      System.Globalization.CultureInfo.InvariantCulture);
            Assert.Throws<ArgumentException>(() => new Ticket(date1, "testTitel", string.Empty, Status.AANGEMAAKT));
        }

        [Fact]
        public void New_Ticket_AantalWijzigingen_Is_0() {

            Assert.Equal(0, ticket.AantalWijzigingen);
        }

        
    }
}
