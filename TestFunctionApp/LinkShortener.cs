using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Configuration;
using Microsoft.Azure.Management.DataLake.Store;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest.Azure.Authentication;
using System.Net.Http.Headers;

namespace TestFunctionApp
{
    public static class HaithemKAROUIApiCaseClass
    {
        private static DataLakeStoreFileSystemManagementClient adlsFileSystemClient;

        // Portal > Azure AD > App Registrations > App > Application ID (aka Client ID)


        [FunctionName("AzureWebApiFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            try
            {
                // Convert all request param into Json object

                var content = req.Content;
                string jsonContent = content.ReadAsStringAsync().Result;
                dynamic requestPram = JsonConvert.DeserializeObject<PartnerMpnModel>(jsonContent);


                // Extract each param
                //string mpnId = requestPram.mpnId;

                if (string.IsNullOrEmpty(requestPram.id))
                {
                  //  return req.CreateResponse(HttpStatusCode.OK, "Please enter the valid partner Mpn Id!");
                }
                // Call Your  API
                HttpClient newClient = new HttpClient();
                //HttpRequestMessage newRequest = new HttpRequestMessage(HttpMethod.Get, string.Format("YourAPIURL?mpnId={0}", requestPram.MpnID));
                // HttpRequestMessage newRequest = new HttpRequestMessage(HttpMethod.Get, string.Format("http://localhost:53027/api/Demo/{0}", requestPram.id));
                HttpRequestMessage newRequest = new HttpRequestMessage(HttpMethod.Get, string.Format("https://webapi20190903093807.azurewebsites.net/api/demo/{0}", requestPram.id));


                //Read Server Response

                HttpResponseMessage response = await newClient.SendAsync(newRequest);
                var responseData = response.Content.ReadAsStringAsync().Result.ToString().Replace("[", "").Replace("]", "");

                //bool isValidMpn = await response.Content.ReadAsAsync<bool>();
                var responseModel = JsonConvert.DeserializeObject<Customer>(responseData);

                var lstData = new List<Customer>();
                lstData.Add(responseModel);
                //Extensions.ExportCSV(lstData, "mycsv");

 //               var config = new ConfigurationBuilder()
 //.SetBasePath(context.FunctionAppDirectory)
 //.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
 //.AddEnvironmentVariables()
 //.Build();
                string clientId1 = ConfigurationManager.AppSettings["clientId"];

                //// Portal > Azure AD > App Registrations > App > Settings > Keys (aka Client Secret)
                //string clientSecret = ConfigurationManager.AppSettings["clientSecretId"];

                //// Portal > Azure AD > Properties > Directory ID (aka Tenant ID)
                //string tenantId = ConfigurationManager.AppSettings["tenantId"];

                //// Name of the Azure Data Lake Store
                //string adlsAccountName = ConfigurationManager.AppSettings["adlsAccountName"];
                string clientId = "67d00f97-4c4d-417c-8e3b-748fcf9e69f5";
                string tenantId = "f7240cee-bd57-4eeb-9bfb-6815bb83b3b4";
                string clientSecretId = "Mu6qmY-4?o43:TjAK+8F1bCXiM+T@G?N";
                string adlsAccountName = "ADLS App";

                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

                // 2. Create credentials to authenticate requests as an Active Directory application
                var clientCredential = new ClientCredential(clientId, clientSecretId);
                var creds = ApplicationTokenProvider.LoginSilentAsync(tenantId, clientCredential).Result;

                //// 2. Initialise Data Lake Store File System Client
                adlsFileSystemClient = new DataLakeStoreFileSystemManagementClient(creds);

                //// 3. Upload a file to the Data Lake Store
                 var source = "G:\\AzureApps\\TestFunctionApp\\TestFunctionApp\\TestFunctionApp\\source.txt";
                string filename = string.Empty;
               // var source = Extensions.ExportCSV(this lstData, filename);


                string csv = Extensions.GetCSV(lstData);
                //CSVHelper.ExportCSV(csv, filename);

                //byte[] filebytes = Encoding.UTF8.GetBytes(csv);

               
                var destination = "destination.txt";


                //// FINISHED
                Console.WriteLine("6. Finished!");


                //return responseData;
                //Return Mpn status

                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StringContent(csv);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "export_" + DateTime.Now + ".csv" };

                adlsFileSystemClient.FileSystem.UploadFile(adlsAccountName, source, destination, 1, false, true);

                return result;

                //return new FileContentResult(filebytes, "application/octet-stream")
                //{
                //    FileDownloadName = "export_" + DateTime.Now + ".csv"
                //};


                // return req.CreateResponse(HttpStatusCode.OK, responseModel);
                //return req.CreateResponse(HttpStatusCode.OK, new PartnerMpnResponseModel { isValidMpn = isValidMpn });
            }
            catch (Exception ex)
            {

                return req.CreateResponse(HttpStatusCode.OK, "Invaild MPN Number! Reason: {0}", string.Format(ex.Message));
                //return new FileContentResult(null, "application/octet-stream")
                //{
                //    FileDownloadName = "Export.csv"
                //};
            }
        }




        public class Customer
        {
            public int Id { set; get; }
            public string CustomerName { set; get; }
            public string Address { set; get; }
            public string City { set; get; }
            public string Pincode { set; get; }
        }
        public class PartnerMpnModel
        {
            public string id { get; set; }
        }


        public class PartnerMpnResponseModel
        {
            public bool isValidMpn { get; set; }
        }
    }
}