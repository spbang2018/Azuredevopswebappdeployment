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
using Newtonsoft.Json;

namespace SamLearnsAzure.Tests.ServiceIntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("ServiceIntegrationTestB")]
    public class PartRelationshipsServiceIntegrationTests : BaseIntegrationTest
    {
        [TestMethod]
        public async Task GetPartRelationshipsIntegrationTest()
        {
            if (base.Client != null)
            {
                //Arrange

                //Act
                HttpResponseMessage response = await base.Client.GetAsync("/api/partrelationships/getpartrelationships");
                response.EnsureSuccessStatusCode();
                string bodyContent = await response.Content.ReadAsStringAsync();
                IEnumerable<PartRelationships> items = JsonConvert.DeserializeObject<IEnumerable<PartRelationships>>(bodyContent);
                response.Dispose();

                //Assert
                Assert.IsTrue(items != null);
                Assert.IsTrue(items.Any()); //There is more than one
                Assert.IsTrue(items.FirstOrDefault().PartRelationshipId > 0); //The first item has an id
                Assert.IsTrue(items.FirstOrDefault().ChildPartNum?.Length > 0); //The child item has an name
                Assert.IsTrue(items.FirstOrDefault().ParentPartNum?.Length > 0); //The parent item has an name
            }
        }

    }
}
