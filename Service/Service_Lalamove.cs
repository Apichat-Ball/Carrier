using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Carrier.Service
{
    public class Service_Lalamove
    {
        public void CallAPILalamoveSFG()
        {
            var client = new RestClient("https://www.sfg-th.com/API_Carrier/Carrier/AutoLalamove");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            IRestResponse response = client.Execute(request);
            //JObject j = JObject.Parse(response.Content);
            var res = response.Content;
        }
    }
}