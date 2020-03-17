using Microsoft.AppCenter.Crashes;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using System.Collections.Generic;

namespace TrialManager.Utils
{
    public class AppCentreSink : ILogEventSink
    {
        public void Emit(LogEvent logEvent)
        {
            Dictionary<string, string> appCentreProperties = new Dictionary<string, string>();
            foreach (KeyValuePair<string, LogEventPropertyValue> pair in logEvent.Properties)
                appCentreProperties.Add(pair.Key, pair.Value.ToString());
            Crashes.TrackError(logEvent.Exception, appCentreProperties);
        }
    }

    public static class AppCentreSinkExtensions
    {
        public static LoggerConfiguration AppCentreSink(
                  this LoggerSinkConfiguration loggerConfiguration)
        {
            return loggerConfiguration.Sink(new AppCentreSink());
        }
    }
}
