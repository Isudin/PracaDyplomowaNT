using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracaDyplomowaNT.Shipx.Requests
{
    public static class Requests
    {
        public static RequestsBase PrepareGetRequest(string url, Config config) => PrepareRequest(url, config, "GET");

        public static RequestsBase PreparePostRequest(string url, Config config) => PrepareRequest(url, config, "POST");

        private static RequestsBase PrepareRequest(string url, Config config, string method)
        {
            var request = new RequestsBase();
            request.URL = url;
            request.ContentType = "application/json";
            request.Method = method;
            request.Json = "";
            request.AddHeader("Authorization", string.Format("Bearer {0}", config.ApiToken));

            return request;
        }
    }
}
