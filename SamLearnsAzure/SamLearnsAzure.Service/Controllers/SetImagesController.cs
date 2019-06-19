﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SamLearnsAzure.Models;
using SamLearnsAzure.Service.DataAccess;

namespace SamLearnsAzure.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetImagesController : ControllerBase
    {
        private readonly ISetImagesRepository _repo;
        private readonly IRedisService _redisService;
        private readonly IConfiguration _configuration;

        public SetImagesController(ISetImagesRepository repo, IRedisService redisService, IConfiguration configuration)
        {
            _repo = repo;
            _redisService = redisService;
            _configuration = configuration;
        }

        [HttpGet("GetSetImages")]
        public async Task<List<SetImages>> GetSetImages(string setNum, bool useCache = true, bool forceBingSearch = false, int resultsToReturn = 1)
        {
            List<SetImages> setImages = new List<SetImages>();

            //1. We don't need the image from the repo/redis

            //2. Get image from Bing Image Search API
            string cognitiveServicesSubscriptionKey = _configuration["CognitiveServicesSubscriptionKey"];
            string cognitiveServicesUriBase = _configuration["AppSettings:CognitiveServicesUriBase"];
            SearchResult result = await BingImageSearch(cognitiveServicesSubscriptionKey, cognitiveServicesUriBase, setNum, resultsToReturn);
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(result.jsonResult);

            for (int i = 0; i < resultsToReturn; i++)
            {
                dynamic firstJsonObj = jsonObj["value"][i];
                string title = firstJsonObj["name"];
                string webUrl = firstJsonObj["webSearchUrl"];
                string imageUrl = (string)jsonObj.SelectToken("value[" + i + "].contentUrl");
                Console.WriteLine("Title for the " + i + " image result: " + title + "\n");
                Console.WriteLine("Web Url for the " + i + " image result: " + webUrl + "\n");
                Console.WriteLine("Image Url for the " + i + " image result: " + imageUrl + "\n");
                SetImages newSetImage = new SetImages
                {
                    SetNum = setNum,
                    SetImage = imageUrl
                };
                setImages.Add(newSetImage);
            }

            return setImages;
        }

        [HttpGet("GetSetImage")]
        public async Task<SetImages> GetSetImage(string setNum, bool useCache = true, bool forceBingSearch = false, int resultsToReturn = 1)
        {
            //1. Service looks in database to see if image exists?
            SetImages setImage = await _repo.GetSetImage(_redisService, useCache, setNum);

            if (setImage == null || forceBingSearch == true)
            {
                //2. Get image from Bing Image Search API
                string cognitiveServicesSubscriptionKey = _configuration["CognitiveServicesSubscriptionKey"];
                string cognitiveServicesUriBase = _configuration["AppSettings:CognitiveServicesUriBase"];
                SearchResult result = await BingImageSearch(cognitiveServicesSubscriptionKey, cognitiveServicesUriBase, setNum, resultsToReturn);
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(result.jsonResult);

                dynamic firstJsonObj = jsonObj["value"][0];
                string title = firstJsonObj["name"];
                string webUrl = firstJsonObj["webSearchUrl"];
                string imageUrl = (string)jsonObj.SelectToken("value[0].contentUrl");
                Console.WriteLine("Title for the first image result: " + title + "\n");
                Console.WriteLine("Web Url for the first image result: " + webUrl + "\n");
                Console.WriteLine("Image Url for the first image result: " + imageUrl + "\n");

                //2. Save image into blob storage
                setImage = await SaveSetImage(setImage, setNum, imageUrl);

            }

            //3. Service returns Image
            return setImage;
        }

        [HttpGet("SaveSetImage")]
        public async Task<SetImages> SaveSetImage(string setNum, string imageUrl)
        {
            //Update database with new image, but don't force it if there is a setimage already
            return await SaveSetImage(null, setNum, imageUrl);
        }

        private async Task<SetImages> SaveSetImage(SetImages setImage, string setNum, string imageUrl)
        {
            //Get the storage blob connection information
            string storageContainerName = _configuration["AppSettings:StorageContainerName"];
            string storageConnectionString = _configuration["AppSettings:StorageConnectionString"];
            storageConnectionString = storageConnectionString.Replace("[ACCOUNTNAME]", _configuration["AppSettings:StorageAccountName"]);
            storageConnectionString = storageConnectionString.Replace("[ACCOUNTKEY]", _configuration["StorageAccountKey" + _configuration["AppSettings:Environment"]]);

            //extract the filename from the image url
            string fileName = GetFileNameFromURL(imageUrl);
            fileName = ConvertFileNameToSetNumber(setNum, fileName);

            //Save the image to the storage blob
            bool saveResult = await SaveImageIntoBlob(storageConnectionString, storageContainerName, imageUrl, fileName);
            Console.WriteLine("Image saved into blob successfully: " + saveResult + "\n");

            if (setImage == null)
            {
                setImage = new SetImages
                {
                    SetNum = setNum,
                    SetImage = fileName
                };
                setImage = await _repo.SaveSetImage(setImage);
            }
            return setImage;
        }

        private string ConvertFileNameToSetNumber(string setNum, string fileName)
        {
            string extension = Path.GetExtension(fileName);
            return setNum + extension;
        }

        //Load up to the storage account, adapted from the Azure quick start for blob storage: 
        //https://github.com/Azure-Samples/storage-blobs-dotnet-quickstart/blob/master/storage-blobs-dotnet-quickstart/Program.cs
        private async Task<bool> SaveImageIntoBlob(string storageConnectionString, string containerName, string imageUrl, string fileName)
        {
            bool result = false;
            //Download the image
            byte[] fileBytes = await DownloadFile(imageUrl);

            CloudBlobContainer cloudBlobContainer;
            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(storageConnectionString, out CloudStorageAccount storageAccount))
            {
                // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                // Create a new container  
                cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);
                bool containerExists = cloudBlobContainer == null || await cloudBlobContainer.ExistsAsync();
                if (containerExists == false)
                {
                    await cloudBlobContainer.CreateAsync();
                    Console.WriteLine("Created container '{0}'", cloudBlobContainer.Name);
                }
                // Set the permissions so the blobs are container. 
                BlobContainerPermissions permissions = new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                };
                await cloudBlobContainer.SetPermissionsAsync(permissions);

                // Get a reference to the blob address, then upload the file to the blob.
                if (fileBytes.Length > 0)
                {
                    using (MemoryStream stream = new MemoryStream(fileBytes, writable: false))
                    {
                        CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                        await cloudBlockBlob.UploadFromStreamAsync(stream);
                    }
                }

                result = true;
            }
            return result;
        }

        private string GetFileNameFromURL(string url)
        {
            Uri uri = new Uri(url);
            return System.IO.Path.GetFileName(uri.LocalPath);
        }

        private async Task<byte[]> DownloadFile(string url)
        {
            byte[] file = null;
            //retry the download 5 times 
            for (int retries = 0; retries < 5; retries++)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        using (HttpResponseMessage result = await client.GetAsync(url))
                        {
                            if (result.IsSuccessStatusCode)
                            {
                                file = await result.Content.ReadAsByteArrayAsync();
                                break;
                            }
                        }
                    }
                }
                catch
                {
                    //do nothing, try again!
                    Console.WriteLine("Failed download - let's try again... (" + (retries + 1).ToString() + "/5");
                }
            }

            return file;
        }

        private async Task<SearchResult> BingImageSearch(string cognitiveServicesSubscriptionKey, string cognitiveServicesUriBase, string searchTerm, int resultsToReturn)
        {

            string uriQuery = cognitiveServicesUriBase + "?q=" + Uri.EscapeDataString(searchTerm) + "&safeSearch=strict&count=" + resultsToReturn.ToString();

            WebRequest request = WebRequest.Create(uriQuery);
            request.Headers["Ocp-Apim-Subscription-Key"] = cognitiveServicesSubscriptionKey;
            HttpWebResponse response =  (HttpWebResponse)request.GetResponseAsync().Result;
            StreamReader streamReader = new StreamReader(response.GetResponseStream());
            string json = await streamReader.ReadToEndAsync();
            streamReader.Dispose();

            // Create the result object for return
            SearchResult searchResult = new SearchResult()
            {
                jsonResult = json,
                relevantHeaders = new Dictionary<string, string>()
            };

            // Extract Bing HTTP headers
            foreach (string header in response.Headers)
            {
                if (header.StartsWith("BingAPIs-") || header.StartsWith("X-MSEdge-"))
                {
                    searchResult.relevantHeaders[header] = response.Headers[header];
                }
            }
            return searchResult;
        }

        struct SearchResult
        {
            public string jsonResult;
            public Dictionary<string, string> relevantHeaders;
        }

    }
}