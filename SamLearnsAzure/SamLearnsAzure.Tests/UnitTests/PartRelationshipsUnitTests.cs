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
    public class PartRelationshipsUnitTests : BaseUnitTest
    {
        [TestMethod]
        public async Task GetPartRelationshipsMockTest()
        {
            //Arrange
            SamsAppDBContext context = new SamsAppDBContext(base.DbOptions);
            Mock<IPartRelationshipsRepository> mock = new Mock<IPartRelationshipsRepository>();
            mock.Setup(repo => repo.GetPartRelationships()).Returns(Task.FromResult(GetPartRelationshipsTestData()));
            PartRelationshipsController controller = new PartRelationshipsController(mock.Object);

            //Act
            IEnumerable<PartRelationships> sets = await controller.GetPartRelationships();

            //Assert
            Assert.IsTrue(sets != null);
            Assert.IsTrue(sets.Count() == 1);
            TestPartRelationships(sets.FirstOrDefault());
        }

        private void TestPartRelationships(PartRelationships PartRelationships)
        {
            Assert.IsTrue(PartRelationships.RelType == "abc");
            Assert.IsTrue(PartRelationships.ChildPartNum == "def");
            Assert.IsTrue(PartRelationships.ParentPartNum == "ghi");
            Assert.IsTrue(PartRelationships.PartRelationshipId == 1);
        }

        private IEnumerable<PartRelationships> GetPartRelationshipsTestData()
        {
            List<PartRelationships> PartRelationships = new List<PartRelationships>
            {
                GetTestRow()
            };
            return PartRelationships;
        }

        private PartRelationships GetTestRow()
        {
            return new PartRelationships()
            {
                RelType = "abc",
                ChildPartNum = "def",
                ParentPartNum = "ghi",
                PartRelationshipId = 1
            };
        }

    }
}
