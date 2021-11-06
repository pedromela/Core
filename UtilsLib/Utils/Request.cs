using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UtilsLib.Utils
{
    public class Request
    {
        public enum AuthTypes 
        {
            BasicAuth = 1,
            BearerToken = 2
        }


        public static string Get(string uri, string auth = null, AuthTypes authType = AuthTypes.BasicAuth)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                if (!string.IsNullOrEmpty(auth))
                {
                    if (authType == AuthTypes.BasicAuth)
                    {
                        var bytes = Encoding.UTF8.GetBytes(auth);
                        string base64auth = Convert.ToBase64String(bytes);
                        request.Headers.Add("Authorization", "Basic " + base64auth);
                    }
                    else if (authType == AuthTypes.BearerToken)
                    {
                        request.Headers.Add("Authorization", "Bearer " + auth);
                    }
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (WebException e)
            {
                if (e.Response != null)
                {
                    var reader = new StreamReader(e.Response.GetResponseStream());
                    if (reader != null)
                    {
                        string resp = reader.ReadToEnd();
                        if (resp.Contains("Invalid value specified for 'instrument'"))
                        {
                            Console.WriteLine("DEBUG");
                        }
                        UtilsLib.DebugMessage(resp);
                        return resp;
                    }
                }
                UtilsLib.DebugMessage(e);
                return e.Message;
            }
            catch (InvalidOperationException e)
            {
                UtilsLib.DebugMessage(e);
                return e.Message;

            }
            catch (Exception e)
            {
                UtilsLib.DebugMessage(e);
                return e.Message;
            }

        }

        public static async Task<string> GetAsync(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public static string Post(string uri, string auth, string data, string contentType = null, AuthTypes authType = AuthTypes.BasicAuth)
        {
            try
            {
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.ContentLength = dataBytes.Length;

                if (!string.IsNullOrEmpty(contentType))
                {
                    request.ContentType = contentType;
                }
                request.Method = "POST";
                if (!string.IsNullOrEmpty(auth))
                {
                    if (authType == AuthTypes.BasicAuth)
                    {
                        var bytes = Encoding.UTF8.GetBytes(auth);
                        string base64auth = Convert.ToBase64String(bytes);
                        request.Headers.Add("Authorization", "Basic " + base64auth);
                    }
                    else if (authType == AuthTypes.BearerToken)
                    {
                        request.Headers.Add("Authorization", "Bearer " + auth);
                    }
                }

                using (Stream requestBody = request.GetRequestStream())
                {
                    requestBody.Write(dataBytes, 0, dataBytes.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (WebException e)
            {
                var resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                UtilsLib.DebugMessage(e);
                UtilsLib.DebugMessage(resp);
                return resp;
            }
            catch (InvalidOperationException e)
            {
                UtilsLib.DebugMessage(e);
            }
            catch (Exception e)
            {
                UtilsLib.DebugMessage(e);
            }
            return "";
        }

        public static string Put(string uri, string auth, string data, string contentType = null, AuthTypes authType = AuthTypes.BasicAuth)
        {
            try
            {
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.ContentLength = dataBytes.Length;

                if (!string.IsNullOrEmpty(contentType))
                {
                    request.ContentType = contentType;
                }
                request.Method = "PUT";
                if (!string.IsNullOrEmpty(auth))
                {
                    if (authType == AuthTypes.BasicAuth)
                    {
                        var bytes = Encoding.UTF8.GetBytes(auth);
                        string base64auth = Convert.ToBase64String(bytes);
                        request.Headers.Add("Authorization", "Basic " + base64auth);
                    }
                    else if (authType == AuthTypes.BearerToken)
                    {
                        request.Headers.Add("Authorization", "Bearer " + auth);
                    }
                }

                using (Stream requestBody = request.GetRequestStream())
                {
                    requestBody.Write(dataBytes, 0, dataBytes.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (WebException e)
            {
                var resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                UtilsLib.DebugMessage(e);
                UtilsLib.DebugMessage(resp);
                return resp;
            }
            catch (InvalidOperationException e)
            {
                UtilsLib.DebugMessage(e);
            }
            catch (Exception e)
            {
                UtilsLib.DebugMessage(e);
            }
            return "";
        }

        public static string Delete(string uri, string auth = null, AuthTypes authType = AuthTypes.BasicAuth)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.Method = "POST";

                if (!string.IsNullOrEmpty(auth))
                {
                    if (authType == AuthTypes.BasicAuth)
                    {
                        var bytes = Encoding.UTF8.GetBytes(auth);
                        string base64auth = Convert.ToBase64String(bytes);
                        request.Headers.Add("Authorization", "Basic " + base64auth);
                    }
                    else if (authType == AuthTypes.BearerToken)
                    {
                        request.Headers.Add("Authorization", "Bearer " + auth);
                    }
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (WebException e)
            {
                string resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                UtilsLib.DebugMessage(e);
                UtilsLib.DebugMessage(resp);
                return resp;
            }
            catch (InvalidOperationException e)
            {
                UtilsLib.DebugMessage(e);
                return e.Message;

            }
            catch (Exception e)
            {
                UtilsLib.DebugMessage(e);
                return e.Message;
            }

        }

        public static async Task<string> PostAsync(string uri, string data, string contentType)
        {
            try
            {
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.ContentLength = dataBytes.Length;
                request.ContentType = contentType;
                request.Method = "POST";

                using (Stream requestBody = request.GetRequestStream())
                {
                    await requestBody.WriteAsync(dataBytes, 0, dataBytes.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            catch (WebException e)
            {
                var resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                UtilsLib.DebugMessage(e);
                UtilsLib.DebugMessage(resp);
            }
            catch (InvalidOperationException e)
            {
                UtilsLib.DebugMessage(e);
            }
            catch (Exception e)
            {
                UtilsLib.DebugMessage(e);
            }
            return "";
        }
    }
}
