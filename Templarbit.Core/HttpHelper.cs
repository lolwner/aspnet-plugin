using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Templarbit.Core
{
    internal static class HttpHelper
    {
        private const int TIMEOUT = 60000;
        public static async Task<ApiResponse> DataPost(string url, string data)
        {
            var apiResponse = new ApiResponse();
            string privacyPolicyContentReport = "";
            string responseString = string.Empty;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {

                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                if (!String.IsNullOrEmpty(privacyPolicyContentReport))
                {
                    request.Headers.Add("Content-Security-Policy-Report-Only", privacyPolicyContentReport);
                }
                var webResponse = (HttpWebResponse)await request.GetResponseAsync();
                var reader = new System.IO.StreamReader(webResponse.GetResponseStream());
                responseString = reader.ReadToEnd();
                apiResponse.StatusCode = webResponse.StatusCode;
                apiResponse.Response = responseString;
                return apiResponse;
            }
            catch (WebException we)
            {
                apiResponse.StatusCode = ((HttpWebResponse)we.Response).StatusCode;
                return apiResponse;
            }
        }
    }
    internal class ApiResponse
    {
        public string Response { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
