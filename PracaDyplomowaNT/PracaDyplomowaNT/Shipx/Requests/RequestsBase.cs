using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Soneta.Business.UI;

namespace PracaDyplomowaNT.Shipx.Requests
{
    public class RequestsBase
    {
        public RequestsBase()
        {
            Headers = new Dictionary<string, string>();
        }

        public string URL { get; set; }
        public string Method { get; set; }
        public string Json { get; set; }
        public string ContentType { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public NetworkCredential Credentials { get; set; }

        public void AddHeader(string naglowek, string wartosc)
        {
            if (!Headers.ContainsKey(naglowek))
                Headers.Add(naglowek, wartosc);
            else Headers[naglowek] = wartosc;
        }

        public object ApiRequest()
        {
            HttpWebRequest request = null;
            request = (HttpWebRequest)WebRequest.Create(URL);

            if (Headers.Count > 0)
                foreach (KeyValuePair<string, string> header in Headers)
                    request.Headers.Add(header.Key, header.Value);

            byte[] bytes = Encoding.UTF8.GetBytes(Json);
            request.ContentLength = bytes.Length;
            request.Method = Method;
            request.ContentType = ContentType;
            request.Credentials = Credentials;

            if (Method == "POST" && bytes != null)
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }

            HttpWebResponse response = null;

            bool error = false;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                error = true;
                if (ex.Response != null)
                    response = (HttpWebResponse)ex.Response;
                else throw ex;
            }

            if (response == null) throw new Exception("Serwer nie zwrócił odpowiedzi.");
            
            string responseText = string.Empty;
            using (var stream = new MemoryStream())
            {
                response.GetResponseStream().CopyTo(stream);
                responseText = Encoding.UTF8.GetString(stream.ToArray());
            }

            if (error) throw new Exception(responseText);

            return responseText;
        }
    }
}
