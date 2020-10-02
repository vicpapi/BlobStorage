using Document.Core.Interfaces;
using Serilog;
using System;
using System.Configuration;

namespace Document.Infrastructure.Repository
{
    public class SeriLogRepository : ISeriLogRepository
    {
        private readonly ILogger _logger = null;

        public SeriLogRepository()
        {
            try
            {
                _logger = new LoggerConfiguration()
                    .WriteTo.File(ConfigurationManager.AppSettings["serilogFileName"].ToString(), shared: true, rollingInterval: RollingInterval.Day)
                    .CreateLogger();

            }
            catch (Exception ex)
            {
                _logger.Error("Error", ex);
            }
        }

        public void LogInfo(string message)
        {
            _logger.Information(message);
        }

        public void LogDebug(string message)
        {
            _logger.Debug(message);
        }

        public void LogWarn(string message)
        {
            _logger.Warning(message);
        }

        public void LogError(string message)
        {
            _logger.Error(message);
        }

        public void LogFatal(string message)
        {
            _logger.Fatal(message);
        }

        public void Dispose()
        {
            Log.CloseAndFlush();
        }
    }
}
