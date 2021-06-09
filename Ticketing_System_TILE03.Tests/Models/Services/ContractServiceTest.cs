using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Ticketing_System_TILE03.Controllers;
using Ticketing_System_TILE03.Data.Services;
using Ticketing_System_TILE03.Models;
using Ticketing_System_TILE03.Models.Domain;
using Ticketing_System_TILE03.Models.Services;
using Ticketing_System_TILE03.Models.ViewModels;
using Ticketing_System_TILE03.Tests.Data;
using Xunit;

namespace Ticketing_System_TILE03.Tests.Controllers
{
    public class ContractServiceTest
    {
        private ContractService _contractService;
        private readonly Mock<IContractRepository> _contractRepository = new Mock<IContractRepository>();
        private readonly Mock<ICompanyRepository> _companyRepository = new Mock<ICompanyRepository>();
        private readonly Mock<IContractTypeRepository> _contractTypeRepository = new Mock<IContractTypeRepository>();
        private DummyApplicationDbContext _context;

        public ContractServiceTest()
        {
            _contractService = new ContractService(_contractRepository.Object, _companyRepository.Object, _contractTypeRepository.Object);
            _context = new DummyApplicationDbContext();
        }

        [Fact]
        public void ShouldAddContractToCompany()
        {
            Contract createdContract = _context.GetTestContract();
            Company testCompany = _context.GetTestCompany();
            _companyRepository.Setup(c => c.GetByCompanyId("123")).Returns(testCompany);

            _contractService.AddContractToCompany(createdContract , "123");

            Assert.Equal(testCompany.Contracts.First(), createdContract);
            _companyRepository.Verify(c => c.SaveChanges());
        }

        [Fact]
        public void ShouldGetActiveTicketsOfCompanyIfStatusIsNull()
        {
            Company testCompany = _context.GetTestCompany();
            _companyRepository.Setup(c => c.GetByCompanyId("123")).Returns(testCompany);

            _contractService.FindAllByStatus(null, "123");

            _contractRepository.Verify(c => c.GetActiveContractsOfCompany(testCompany));
        }

        [Fact]
        public void ShouldGetByStatusAndCompany()
        {
            Company testCompany = _context.GetTestCompany();
            _companyRepository.Setup(c => c.GetByCompanyId("123")).Returns(testCompany);

            _contractService.FindAllByStatus(ContractStatus.LOPEND, "123");

            _contractRepository.Verify(c => c.GetByStatusAndCompany(ContractStatus.LOPEND, testCompany));
        }

    }

}
