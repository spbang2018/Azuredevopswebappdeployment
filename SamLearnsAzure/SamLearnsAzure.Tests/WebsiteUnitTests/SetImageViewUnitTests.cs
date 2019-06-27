using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SamLearnsAzure.Models;
using SamLearnsAzure.Web.Controllers;
using SamLearnsAzure.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamLearnsAzure.Tests.WebsiteUnitTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("UnitTest")]
    public class SetImageViewUnitTests : BaseUnitTest
    {
        //[TestMethod]
        //public async Task GetSetImagesViewMockTest()
        //{
        //    //Arrange
        //    string setNum = "abc123";
        //    string configValue = "xyz321";
        //    Mock<IServiceApiClient> mockService = new Mock<IServiceApiClient>();
        //    Mock<IConfiguration> mockConfiguration = new Mock<IConfiguration>();
        //    mockService.Setup(repo => repo.GetSet(It.IsAny<string>())).Returns(Task.FromResult(GetSetTestData()));
        //    mockService.Setup(repo => repo.GetSetImages(It.IsAny<string>())).Returns(Task.FromResult(GetSetImagesTestData()));
        //    mockConfiguration.SetupGet(x => x[It.IsAny<string>()]).Returns(configValue);
        //    HomeController controller = new HomeController(mockService.Object, mockConfiguration.Object);

        //    //Act
        //    IActionResult result = await controller.UpdateImage(setNum);

        //    //Assert
        //    ViewResult viewResult = (ViewResult)result;
        //    UpdateImageViewModel updateImageViewModel = (UpdateImageViewModel)viewResult.Model;
        //    Assert.IsTrue(updateImageViewModel != null);
        //    Assert.IsTrue(updateImageViewModel.Set != null);
        //    TestSet(updateImageViewModel.Set);
        //    Assert.IsTrue(updateImageViewModel.BaseSetImagesStorageURL == null);
        //    Assert.IsTrue(updateImageViewModel.SetImages != null);
        //    Assert.IsTrue(updateImageViewModel.SetImages.Any());
        //    TestSetImages(updateImageViewModel.SetImages[0]);
        //}

        [TestMethod]
        public async Task GetSetViewMockTest()
        {
            //Arrange
            string setNum = "abc123";
            string configValue = "xyz321";
            Mock<IServiceApiClient> mockService = new Mock<IServiceApiClient>();
            Mock<IConfiguration> mockConfiguration = new Mock<IConfiguration>();
            mockService.Setup(repo => repo.GetSet(It.IsAny<string>())).Returns(Task.FromResult(GetSetTestData()));
            mockService.Setup(repo => repo.GetSetImages(It.IsAny<string>())).Returns(Task.FromResult(GetSetImagesTestData()));
            mockConfiguration.SetupGet(x => x[It.IsAny<string>()]).Returns(configValue);
            HomeController controller = new HomeController(mockService.Object, mockConfiguration.Object);

            //Act
            IActionResult result = await controller.UpdateImage(setNum);

            //Assert
            ViewResult viewResult = (ViewResult)result;
            UpdateImageViewModel updateImageViewModel = (UpdateImageViewModel)viewResult.Model;
            Assert.IsTrue(updateImageViewModel != null);
            Assert.IsTrue(updateImageViewModel.Set != null);
            TestSet(updateImageViewModel.Set);
            Assert.IsTrue(updateImageViewModel.BaseSetImagesStorageURL == null);
            Assert.IsTrue(updateImageViewModel.SetImages != null);
            Assert.IsTrue(updateImageViewModel.SetImages.Any());
            TestSetImages(updateImageViewModel.SetImages[0]);
        }

        private void TestSet(Sets set)
        {
            Assert.IsTrue(set.SetNum == "abc");
            Assert.IsTrue(set.Name == "def");
            Assert.IsTrue(set.NumParts == 1);
            Assert.IsTrue(set.ThemeId == 2);
            Assert.IsTrue(set.Year == 3);
            Assert.IsTrue(set.Theme != null);
            Assert.IsTrue(set.Inventories != null);
            Assert.IsTrue(set.InventorySets != null);
            Assert.IsTrue(set.OwnerSets != null);
        }

        private Sets GetSetTestData()
        {
            Themes Theme = new Themes
            {
                Id = 2,
                Name = "ghi",
                ParentId = null
            };

            return new Sets()
            {
                SetNum = "abc",
                Name = "def",
                NumParts = 1,
                ThemeId = 2,
                Year = 3,
                Theme = Theme
            };
        }

        private void TestSetImages(SetImages setImage)
        {
            Assert.IsTrue(setImage.SetNum == "abc");
            Assert.IsTrue(setImage.SetImage == "def");
            Assert.IsTrue(setImage.SetImageId == 1);
        }

        private List<SetImages> GetSetImagesTestData()
        {
            List<SetImages> items = new List<SetImages>();
            items.Add(
                new SetImages()
                {
                    SetNum = "abc",
                    SetImage = "def",
                    SetImageId = 1
                }
            );
            return items;
        }

    }
}