using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SamLearnsAzure.Service.Controllers;
using SamLearnsAzure.Service.DataAccess;
using SamLearnsAzure.Service.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamLearnsAzure.Tests.UnitTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("UnitTest")]
    public class InventoryPartsUnitTests : BaseUnitTest
    {
        [TestMethod]
        public async Task GetInventoryPartsMockTest()
        {
            //Arrange
            SamsAppDBContext context = new SamsAppDBContext(base.DbOptions);
            Mock<IInventoryPartsRepository> mock = new Mock<IInventoryPartsRepository>();
            mock.Setup(repo => repo.GetInventoryParts()).Returns(Task.FromResult(GetInventoryPartsTestData()));
            InventoryPartsController controller = new InventoryPartsController(mock.Object);

            //Act
            IEnumerable<InventoryParts> sets = await controller.GetInventoryParts();

            //Assert
            Assert.IsTrue(sets != null);
            Assert.IsTrue(sets.Count() == 1);
            TestInventoryParts(sets.FirstOrDefault());
        }

        private void TestInventoryParts(InventoryParts InventoryParts)
        {
            Assert.IsTrue(InventoryParts.InventoryId == 1);
            Assert.IsTrue(InventoryParts.PartNum == "abc");
            Assert.IsTrue(InventoryParts.ColorId == 2);
            Assert.IsTrue(InventoryParts.Quantity == 3);
            Assert.IsTrue(InventoryParts.IsSpare == false);
            Assert.IsTrue(InventoryParts.InventoryPartId == 4);
            Assert.IsTrue(InventoryParts.Color != null);
            Assert.IsTrue(InventoryParts.Inventory != null);
            Assert.IsTrue(InventoryParts.Part != null);
        }

        private IEnumerable<InventoryParts> GetInventoryPartsTestData()
        {
            List<InventoryParts> InventoryParts = new List<InventoryParts>
            {
                GetTestRow()
            };
            return InventoryParts;
        }

        private InventoryParts GetTestRow()
        {
            return new InventoryParts()
            {
                InventoryId = 1,
                PartNum = "abc",
                ColorId = 2,
                Quantity = 3,
                IsSpare = false,
                InventoryPartId = 4,
                Color = new Colors(),
                Inventory = new Inventories(),
                Part = new Parts()
            };
        }

    }
}
