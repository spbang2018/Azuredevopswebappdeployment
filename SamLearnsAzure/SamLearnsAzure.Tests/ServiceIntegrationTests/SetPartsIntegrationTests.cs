using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Collections.Generic;
using SamLearnsAzure.Models;
using System.Data.SqlClient;
using SamLearnsAzure.Service.Controllers;
using Moq;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using SamLearnsAzure.Service.DataAccess;
using System.Net.Http;

namespace SamLearnsAzure.Tests.ServiceIntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("IntegrationTest")]
    [TestCategory("RedisTest")]
    public class SetPartsIntegrationTests : BaseIntegrationTest
    {
        [TestMethod]
        public async Task GetSetPartsIntegrationWithCacheTest()
        {
            if (base.Client != null)
            {
                //Arrange
                string setNum = "75218-1";

                //Act
                HttpResponseMessage response = await base.Client.GetAsync("/api/setparts/getsetparts?setnum=" + setNum + "&useCache=true");
                response.EnsureSuccessStatusCode();
                IEnumerable<SetParts> items = await response.Content.ReadAsAsync<IEnumerable<SetParts>>();
                response.Dispose();

                //Assert
                Assert.IsTrue(items != null);
                Assert.IsTrue(items.Any()); //There is more than one
                Assert.IsTrue(items.FirstOrDefault().PartNum != ""); //The first item has an id
                Assert.IsTrue(items.FirstOrDefault().PartName?.Length > 0); //The first item has an name
            }
        }

        [TestMethod]
        public async Task GetSetPartsIntegrationWithoutCacheTest()
        {
            if (base.Client != null)
            {
                //Arrange
                string setNum = "75218-1";

                //Act
                HttpResponseMessage response = await base.Client.GetAsync("/api/setparts/getsetparts?setnum=" + setNum + "&useCache=false");
                response.EnsureSuccessStatusCode();
                IEnumerable<SetParts> items = await response.Content.ReadAsAsync<IEnumerable<SetParts>>();
                response.Dispose();

                //Assert
                Assert.IsTrue(items != null);
                Assert.IsTrue(items.Any()); //There is more than one
                Assert.IsTrue(items.FirstOrDefault().PartNum != ""); //The first item has an id
                Assert.IsTrue(items.FirstOrDefault().PartName?.Length > 0); //The first item has an name
            }
        }


        [TestMethod]
        public async Task SearchForMissingPartsIntegrationTest()
        {
            if (base.Client != null)
            {
                //Arrange
                string setNum = "75168-1"; //Yoda's Jedi starfighter

                //Act
                HttpResponseMessage response = await base.Client.GetAsync("/api/setparts/SearchForMissingParts?setnum=" + setNum);
                response.EnsureSuccessStatusCode();
                bool result = await response.Content.ReadAsAsync<bool>();
                response.Dispose();

                //Assert
                Assert.IsTrue(result);
            }
        }

    }
}
