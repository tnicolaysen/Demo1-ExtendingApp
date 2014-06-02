using System;
using NLog.Common;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;

namespace GenericWebApp.Logging
{
    [Target("ElasticSearch")]
    public sealed class ElasticSearchTarget : TargetWithLayout
    {
        [RequiredParameter]
        public Layout Url { get; set; }

        public ElasticSearchTarget()
        {
            // Harcdoded. Won't work if it doesn't render proper JSON.
            Layout = Layout.FromString("$(jsondump)");
        }

        protected override void Write(AsyncLogEventInfo info)
        {
            try
            {
                SendToElasticSearch(info);
            }
            catch (Exception ex)
            {
                info.Continuation(ex);
            }
        }

        private void SendToElasticSearch(AsyncLogEventInfo info)
        {
            var json = Layout.Render(info.LogEvent);
            var uri = GetUriFromLayout(info);

             var jsonClient = new SimpleElasticSearchWebClient();
            jsonClient.Error += exception => info.Continuation(exception);
            jsonClient.PostAsync(uri, json);
        }

        private Uri GetUriFromLayout(AsyncLogEventInfo info)
        {
            return new Uri(Url.Render(info.LogEvent));
        }
    }
}