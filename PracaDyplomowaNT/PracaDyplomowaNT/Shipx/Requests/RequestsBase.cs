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
            //ServicePointManager.CertificatePolicy = new MyPolicy();
        }

        public string URL { get; set; }
        public string METHOD { get; set; }
        public string Json { get; set; }
        public FormDataParam[] multiPartData { get; set; }
        public byte[] StreamBytes { get; set; }
        public string ContentType { get; set; }
        public bool PreAuth { get; set; }
        public bool KeepAlive { get; set; } = true;
        public string Boundary { get; set; } = "AaB03x";
        public Dictionary<string, string> Headers { get; set; }
        public NetworkCredential Credentials { get; set; }

        public void DodajNaglowek(string naglowek, string wartosc)
        {
            if (!Headers.ContainsKey(naglowek))
                Headers.Add(naglowek, wartosc);
            else Headers[naglowek] = wartosc;
        }

        public string GetMultiPartData()
        {
            string postData = "";
            foreach (FormDataParam data in multiPartData)
            {
                postData += "--" + Boundary + "\r\n";
                switch (data.Type)
                {
                    case FormDataParam.FormDataType.text:
                        postData += "Content-Disposition: form-data; name=\"" + data.Key + "\"\r\n\r\n" + data.Value + "\r\n";
                        break;
                    case FormDataParam.FormDataType.file:
                        postData += "Content-Disposition: form-data; name=\"" + data.Key + "\"; filename=\"" + ((FormDataFileParam)data).FileName + "\"\r\n";
                        postData += "Content-Type: text/xml\r\n\r\n";
                        postData += data.Value + "\r\n";
                        break;
                }
            }

            postData += "--" + Boundary + "\r\n";
            return postData;
        }

        public object ApiRequest()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            StreamBytes = null;

            if (multiPartData != null)
                Json = GetMultiPartData();

            byte[] bytes = Json == null ? null : Encoding.UTF8.GetBytes(Json);

            HttpWebRequest request = null;
            request = (HttpWebRequest)WebRequest.Create(URL);
            //try
            //{
            //request = (HttpWebRequest)WebRequest.Create(URL);
            //}
            //catch (Exception e)
            //{
            //    if (e.Message.Contains("identyfikator URI jest pusty"))
            //        return Tools.ShowError("", "Wystąpił błąd: Nie podano w konfiguracji adresu API dla kuriera");

            //    return e.Message.Contains("nie można określić formatu identyfikatora URI")
            //        ? Tools.ShowError("", "Wystąpił błąd: Błędny format adresu API.")
            //        : Tools.ShowError("", "Niepoprawny lub wprowadzony z błędami adres API. Sprawdź poprawność adresu w konfiguracji. ( Wystąpił błąd: " + e.Message + " ).");
            //}

            if (Headers.Count > 0)
                foreach (KeyValuePair<string, string> naglowek in Headers)
                    request.Headers.Add(naglowek.Key, naglowek.Value);

            request.PreAuthenticate = PreAuth;
            request.Method = METHOD;
            request.ContentType = ContentType;
            request.KeepAlive = KeepAlive;

            if (bytes != null)
                request.ContentLength = bytes.Length;

            if (METHOD != "GET" && bytes != null)
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }

            string responseText = null;
            HttpWebResponse response = null;

            request.Credentials = Credentials;

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

            if (response == null) throw new Exception("Pusta odpowiedź z serwera.");

            using (var strem = new MemoryStream())
            {
                response.GetResponseStream().CopyTo(strem);
                StreamBytes = strem.ToArray();
                responseText = Encoding.UTF8.GetString(StreamBytes);
            }

            if (error) throw new Exception(responseText);

            return responseText;
        }

        public class FormDataParam
        {
            public enum FormDataType
            {
                text,
                file
            }

            public virtual FormDataType Type => FormDataType.text;
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public class FormDataFileParam : FormDataParam
        {
            public override FormDataType Type => FormDataType.file;
            public string FileName { get; set; }
        }
    }
}
