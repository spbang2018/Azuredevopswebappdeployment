using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SamLearnsAzure.Models;
using SamLearnsAzure.Service.Controllers;
using SamLearnsAzure.Service.DataAccess;

namespace SamLearnsAzure.Tests.ServiceUnitTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("UnitTest")]
    public class OwnerSetsServiceUnitTests : BaseUnitTest
    {
        [TestMethod]
        public async Task GetOwnerSetsMockTest()
        {
            //Arrange
            Mock<IOwnerSetsRepository> mock = new Mock<IOwnerSetsRepository>();
            Mock<IRedisService> mockRedis = new Mock<IRedisService>();
            mock.Setup(repo => repo.GetOwnerSets(It.IsAny<IRedisService>(), It.IsAny<bool>(), It.IsAny<int>())).Returns(Task.FromResult(GetOwnerSetsTestData()));
            OwnerSetsController controller = new OwnerSetsController(mock.Object, mockRedis.Object);
            int ownerId = 1;

            //Act
            IEnumerable<OwnerSets> sets = await controller.GetOwnerSets(ownerId);

            //Assert
            Assert.IsTrue(sets != null);
            Assert.IsTrue(sets.Count() == 1);
            TestOwnerSets(sets.FirstOrDefault());
        }

        [TestMethod]
        public async Task SaveOwnerSetsMockTest()
        {
            //Arrange
            Mock<IOwnerSetsRepository> mock = new Mock<IOwnerSetsRepository>();
            Mock<IRedisService> mockRedis = new Mock<IRedisService>();
            mock.Setup(repo => repo.SaveOwnerSet(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult(true));
            OwnerSetsController controller = new OwnerSetsController(mock.Object, mockRedis.Object);
            int ownerId = 1;
            string setNum = "abc123";
            bool owned = true;
            bool wanted = false;

            //Act
            bool result = await controller.SaveOwnerSet(setNum, ownerId, owned, wanted);

            //Assert
            Assert.IsTrue(result == true);
        }

        private void TestOwnerSets(OwnerSets OwnerSet)
        {
            Assert.IsTrue(OwnerSet.SetNum == "abc");
            Assert.IsTrue(OwnerSet.OwnerId == 1);
            Assert.IsTrue(OwnerSet.Owned == false);
            Assert.IsTrue(OwnerSet.Wanted == true);
            Assert.IsTrue(OwnerSet.OwnerSetId == 2);
            Assert.IsTrue(OwnerSet.OwnerName != null);
            Assert.IsTrue(OwnerSet.SetName == "setabc");
            Assert.IsTrue(OwnerSet.SetNumParts == 3);
            Assert.IsTrue(OwnerSet.SetThemeName == "rst");
            Assert.IsTrue(OwnerSet.SetYear == 2000);
        }

        private IEnumerable<OwnerSets> GetOwnerSetsTestData()
        {
            List<OwnerSets> OwnerSets = new List<OwnerSets>
            {
                GetTestRow()
            };
            return OwnerSets;
        }

        private OwnerSets GetTestRow()
        {
            return new OwnerSets()
            {
                SetNum = "abc",
                OwnerId = 1,
                Owned = false,
                Wanted = true,
                OwnerSetId = 2,
                OwnerName = "xyz",
                SetName = "setabc",
                SetNumParts = 3,
                SetThemeName = "rst",
                SetYear = 2000

            };
        }

    }
}
