using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Collections.Generic;
using SamLearnsAzure.Service.Models;
using System.Data.SqlClient;
using SamLearnsAzure.Service.Controllers;
using Moq;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using SamLearnsAzure.Service.DataAccess;
using System.Net.Http;

namespace SamLearnsAzure.Tests.IntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("IntegrationTest")]
    public class PartRelationshipsIntegrationTests : BaseIntegrationTest
    {
        [TestMethod]
        public async Task GetPartRelationshipsIntegrationTest()
        {
            //Arrange

            //Act
            HttpResponseMessage response = await base.Client.GetAsync("/api/partrelationships/getpartrelationships");
            response.EnsureSuccessStatusCode();
            IEnumerable<PartRelationships> items = await response.Content.ReadAsAsync<IEnumerable<PartRelationships>>();

            //Assert
            Assert.IsTrue(items != null);
            Assert.IsTrue(items.Count() == 0); //There is more than one owner
            //Assert.IsTrue(items.FirstOrDefault().PartRelationshipId > 0); //The first item has an id
            //Assert.IsTrue(items.FirstOrDefault().Name.Length > 0); //The first item has an name
        }
        
    }
}