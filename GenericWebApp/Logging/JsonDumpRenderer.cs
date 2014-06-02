using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using Newtonsoft.Json;
using NLog;
using NLog.LayoutRenderers;

namespace GenericWebApp.Logging
{
    [LayoutRenderer("jsondump")]
    public class JsonDumpLayoutRenderer : LayoutRenderer
    {
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var logEntry = CreateLogEntry(logEvent);
            builder.Append(logEntry);
        }

        private string CreateLogEntry(LogEventInfo @event)
        {
            // Dynamic FTW! Easy to add properties.
            dynamic logEntry = new ExpandoObject();
            logEntry.Level = @event.Level.Name;
            logEntry.LoggerName = @event.LoggerName;

            // Slight hack to add the special timestamp property
            ((IDictionary<String, Object>) logEntry).Add("@timestamp", @event.TimeStamp.ToUniversalTime().ToString("O"));

            // Using NLog's built in "renderers" to get more details
            logEntry.Message = RenderUsing<MessageLayoutRenderer>(@event);
            logEntry.Identity = RenderUsing<IdentityLayoutRenderer>(@event);
            logEntry.MachineName = RenderUsing<MachineNameLayoutRenderer>(@event);
            logEntry.WindowsIdentity = RenderUsing<WindowsIdentityLayoutRenderer>(@event);
            logEntry.ThreadId = RenderUsing<ThreadIdLayoutRenderer>(@event);
            logEntry.ProcessId = RenderUsing<ProcessIdLayoutRenderer>(@event);

            logEntry.NDC = "2014";
            // Add your own relevant information here

            if (@event.Exception != null)
                logEntry.ErrorDetails = CreateExceptionEntry(@event.Exception);

            return JsonConvert.SerializeObject(logEntry, Formatting.Indented);
        }

        #region Magic little helpers

        private static string RenderUsing<TRenderer>(LogEventInfo @event)
            where TRenderer : LayoutRenderer, new()
        {
            return new TRenderer().Render(@event);
        }

        private static dynamic CreateExceptionEntry(Exception ex)
        {
            // Hint: Make it recursive if you want to get inner exceptions
            dynamic exception = new ExpandoObject();
            exception.Type = ex.GetType().ToString();
            exception.Message = ex.Message;
            exception.ExceptionData = ex.Data;
            exception.StackTraceString = ex.StackTrace;
            return exception;
        }

        #endregion
    }
}   