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


namespace TestFunctionApp
{
    public static class HaithemKAROUIApiCaseClass
    {
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
                    return req.CreateResponse(HttpStatusCode.OK, "Please enter the valid partner Mpn Id!");
                }
                // Call Your  API
                HttpClient newClient = new HttpClient();
                //HttpRequestMessage newRequest = new HttpRequestMessage(HttpMethod.Get, string.Format("YourAPIURL?mpnId={0}", requestPram.MpnID));
               // HttpRequestMessage newRequest = new HttpRequestMessage(HttpMethod.Get, string.Format("http://localhost:53027/api/Demo/{0}", requestPram.id));
                HttpRequestMessage newRequest = new HttpRequestMessage(HttpMethod.Get, string.Format("https://webapi20190903093807.azurewebsites.net/api/demo/{0}", requestPram.id));


                //Read Server Response

                HttpResponseMessage response = await newClient.SendAsync(newRequest);
                var responseData =  response.Content.ReadAsStringAsync().Result.ToString().Replace("[","").Replace("]", "");

                //bool isValidMpn = await response.Content.ReadAsAsync<bool>();
                var responseModel = JsonConvert.DeserializeObject<Customer>(responseData);


                //return responseData;
                //Return Mpn status 
                return req.CreateResponse(HttpStatusCode.OK, responseModel);
                //return req.CreateResponse(HttpStatusCode.OK, new PartnerMpnResponseModel { isValidMpn = isValidMpn });
            }
            catch (Exception ex)
            {

                return req.CreateResponse(HttpStatusCode.OK, "Invaild MPN Number! Reason: {0}", string.Format(ex.Message));
            }
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