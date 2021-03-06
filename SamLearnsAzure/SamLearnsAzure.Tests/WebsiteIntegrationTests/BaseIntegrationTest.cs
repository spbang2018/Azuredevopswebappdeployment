using System;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SamLearnsAzure.Tests.WebsiteIntegrationTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class BaseIntegrationTest
    {
        public IConfigurationRoot Configuration = null!;

        [TestInitialize]
        public void TestStartUp()
        {
            IConfigurationBuilder config = new ConfigurationBuilder()
               .SetBasePath(AppContext.BaseDirectory)
               .AddJsonFile("appsettings.json")
               .AddUserSecrets<BaseIntegrationTest>();
            Configuration = config.Build();

            string azureKeyVaultURL = Configuration["AppSettings:KeyVaultURL"];
            string clientId = Configuration["AppSettings:ClientId"];
            string clientSecret = Configuration["AppSettings:ClientSecret"];
            //AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            //KeyVaultClient keyVaultClient = new KeyVaultClient(
            //    new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            //config.AddAzureKeyVault(azureKeyVaultURL, keyVaultClient, new DefaultKeyVaultSecretManager());
            config.AddAzureKeyVault(azureKeyVaultURL, clientId, clientSecret);
            Configuration = config.Build();
        }
    }
}
