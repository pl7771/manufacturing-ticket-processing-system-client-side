using System;
using System.Collections.Generic;
using System.Text;
using Ticketing_System_TILE03.Models.Domain;
using Ticketing_System_TILE03.Tests.Data;
using Xunit;

namespace Ticketing_System_TILE03.Tests.Models.Domain
{
    public class ContractTest
    {
        private DummyApplicationDbContext _context;
        private readonly IEnumerable<Contract> _contracts;

        public ContractTest()
        {
            _context = new DummyApplicationDbContext();
            _contracts = _context.Contracts;
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        public void newContract_MandatoryConstructor_EindDatumBeforeOrEqualToStartDatum_Fails(int daysBefore)
        {
            DateTime begin = new DateTime(2021, 1, 20);
            DateTime end = begin.AddDays(-daysBefore);
            Assert.Throws<ArgumentException>(() => new Contract(null, ContractStatus.LOPEND, begin, end));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        public void SetEindDatum_BeforeOrEqualToStartDatum_Fails(int daysBefore)
        {
            DateTime begin = new DateTime(2021, 1, 20);
            DateTime end = begin.AddDays(2);
            Contract contract = new Contract(null, ContractStatus.LOPEND, begin, end);
            end = begin.AddDays(-daysBefore);
            Assert.Throws<ArgumentException>(()=>contract.EindDatum = end);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        public void SetStartDatum_AfterOrEqualToEindDatum_Fails(int daysAfter)
        {
            DateTime begin = new DateTime(2021, 1, 20);
            DateTime end = begin.AddDays(2);
            Contract contract = new Contract(null, ContractStatus.LOPEND, begin, end);
            begin = end.AddDays(daysAfter);
            Assert.Throws<ArgumentException>(() => contract.StartDatum = begin);
        }

    }
}
