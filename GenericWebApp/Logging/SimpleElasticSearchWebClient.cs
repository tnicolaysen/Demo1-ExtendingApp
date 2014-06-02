using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GenericWebApp.Logging
{
    /// <summary>
    /// A bit complex, but this is a safe way to handle errors for async calls to ElasticSearch.
    /// </summary>
    public class SimpleElasticSearchWebClient
    {
        private readonly WebClient _internalClient;

        public event Action<Exception> Error;

        protected virtual void OnError(Exception obj)
        {
            var handler = Error;
            if (handler != null) 
                handler(obj);
        }

        public SimpleElasticSearchWebClient()
        {
            _internalClient = new WebClient();
            _internalClient.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=UTF-8");
            _internalClient.Headers.Add(HttpRequestHeader.Accept, "application/json");
            
            SetupErrorHandling(_internalClient);
        }

        public void PostAsync(Uri uri, string jsonData)
        {
            // Run async
            Task.Run(() =>
                _internalClient.UploadString(uri, "POST", jsonData)
             );
        }

        private void SetupErrorHandling(WebClient client)
        {
            UploadStringCompletedEventHandler cb = null;
            cb = (s, e) =>
            {
                if (cb != null)
                    client.UploadStringCompleted -= cb;

                if (!(e.Error is WebException))
                {
                    OnError(e.Error);
                    return;
                }

                try
                {
                    var we = e.Error as WebException;

                    JObject result = JObject.Load(new JsonTextReader(new StreamReader(we.Response.GetResponseStream() ?? Stream.Null)));
                    JToken error = result.GetValue("error");

                    if (error != null)
                    {
                        OnError(new Exception(result.ToString(), e.Error));
                        return;
                    }
                }
                catch (Exception)
                {
                    OnError(new Exception("Failed to send log event to ElasticSearch", e.Error));
                }

                OnError(e.Error);
            };

            client.UploadStringCompleted += cb;
        }
    }
}