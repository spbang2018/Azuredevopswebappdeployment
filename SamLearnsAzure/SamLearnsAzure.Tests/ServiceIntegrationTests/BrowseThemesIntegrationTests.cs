using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SamLearnsAzure.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SamLearnsAzure.Tests.ServiceIntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("ServiceIntegrationTestA")]
    public class BrowseThemesServiceIntegrationTests : BaseIntegrationTest
    {
        [TestMethod]
        public async Task GetBrowseThemesIntegrationWithCacheTest()
        {

            if (base.Client != null)
            {
                //Arrange
                int? year = 2020;

                //Act
                HttpResponseMessage response = await base.Client.GetAsync("/api/BrowseThemes/getBrowseThemes?useCache=true&year=" + year);
                response.EnsureSuccessStatusCode();
                string bodyContent = await response.Content.ReadAsStringAsync();
                IEnumerable<BrowseThemes> items = JsonConvert.DeserializeObject<IEnumerable<BrowseThemes>>(bodyContent);
                response.Dispose();

                //Assert
                Assert.IsTrue(items != null);
                Assert.IsTrue(items.Any()); //There is more than one
                Assert.IsTrue(items.FirstOrDefault().Id > 0); //The first item has an id
                Assert.IsTrue(items.FirstOrDefault().Name?.Length > 0); //The first item has an name
            }
        }

        [TestMethod]
        public async Task GetBrowseThemesIntegrationWithoutCacheTest()
        {
            if (base.Client != null)
            {
                //Arrange
                int? year = 2020;

                //Act
                HttpResponseMessage response = await base.Client.GetAsync("/api/BrowseThemes/getBrowseThemes?useCache=false&year=" + year);
                response.EnsureSuccessStatusCode();
                string bodyContent = await response.Content.ReadAsStringAsync();
                IEnumerable<BrowseThemes> items = JsonConvert.DeserializeObject<IEnumerable<BrowseThemes>>(bodyContent);
                response.Dispose();

                //Assert
                Assert.IsTrue(items != null);
                Assert.IsTrue(items.Any()); //There is more than one
                Assert.IsTrue(items.FirstOrDefault().Id > 0); //The first item has an id
                Assert.IsTrue(items.FirstOrDefault().Name?.Length > 0); //The first item has an name
            }
        }

    }
}
