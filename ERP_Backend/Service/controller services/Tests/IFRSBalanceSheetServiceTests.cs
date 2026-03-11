using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using ERP.Model;
using ERP.Service;
using Moq;
using Microsoft.AspNetCore.Mvc;
using ERP.Controllers;
using static ERP.Model.ApplicationDbContext;

namespace ERP.Service.controller_services.Tests
{
    public class IFRSBalanceSheetServiceTests
    {

        [Fact]
        public void IFRSBalanceSheetService_GenerateBalanceSheet_ReturnsNonNullBalanceSheet()
        {
            // Arrange
            var mockLedgerService = new Mock<INominalLedgerService>();
            mockLedgerService.Setup(s => s.GetEntries(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(new List<NominalLedgerEntry>());

            var service = new IFRSBalanceSheetService(mockLedgerService.Object);

            // Act
            var balanceSheet = service.GenerateBalanceSheet(DateTime.Now);

            // Assert
            Assert.NotNull(balanceSheet);
        }
        [Fact]
        public void IFRSBalanceSheetService_GetBalanceSheet_ReturnsValidBalanceSheet()
        {
            // Arrange
            var expectedSheet = new IFRSBalanceSheet
            {
                Assets = new List<AccountBalance>
                {
                    new AccountBalance { AccountCode = "1000", Balance = 1000 }
                },
                Liabilities = new List<AccountBalance>
                {
                    new AccountBalance { AccountCode = "2000", Balance = 500 }
                },
                Equity = new List<AccountBalance>
                {
                    new AccountBalance { AccountCode = "3000", Balance = 300 }
                }
            };
            var mockBalanceSheetService = new Mock<IIFRSBalanceSheetService>();
            mockBalanceSheetService.Setup(s => s.GenerateBalanceSheet(It.IsAny<DateTime>()))
                .Returns(expectedSheet);
            var controller = new IFRSBalanceSheetController(mockBalanceSheetService.Object);
            // Act
            var result = controller.GetBalanceSheet(DateTime.Now) as OkObjectResult;
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualSheet = okResult.Value as IFRSBalanceSheet;
            Assert.Equal(expectedSheet, okResult.Value);
        }

        [Fact]
        public void IFRSBalanceSheetService_GenerateBalanceSheet_ReturnsCorrectClassification()
        {
            // Arrange
            var mockLedgerService = new Mock<INominalLedgerService>();
            mockLedgerService.Setup(s => s.GetEntries(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(new List<NominalLedgerEntry>
                {
                    new NominalLedgerEntry { AccountCode = "1000", Debit = 1000, Credit = 0, EntryDate = DateTime.Now },
                    new NominalLedgerEntry { AccountCode = "2000", Debit = 0, Credit = 500, EntryDate = DateTime.Now },
                    new NominalLedgerEntry { AccountCode = "3000", Debit = 0, Credit = 300, EntryDate = DateTime.Now }
                });

            var service = new IFRSBalanceSheetService(mockLedgerService.Object);

            // Act
            var balanceSheet = service.GenerateBalanceSheet(DateTime.Now);

            // Assert
            Assert.Single(balanceSheet.Assets);
            Assert.Single(balanceSheet.Liabilities);
            Assert.Single(balanceSheet.Equity);
            Assert.Equal("1000", balanceSheet.Assets[0].AccountCode);
            Assert.Equal("2000", balanceSheet.Liabilities[0].AccountCode);
            Assert.Equal("3000", balanceSheet.Equity[0].AccountCode);
        }

        [Fact]
        public void IFRSBalanceSheetController_GetBalanceSheet_ReturnsOkResult()
        {
            // Arrange
            var mockService = new Mock<IIFRSBalanceSheetService>();
            mockService.Setup(s => s.GenerateBalanceSheet(It.IsAny<DateTime>()))
                .Returns(new IFRSBalanceSheet());

            var controller = new IFRSBalanceSheetController(mockService.Object);

            // Act
            var result = controller.GetBalanceSheet(DateTime.Now);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
