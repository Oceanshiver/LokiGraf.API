using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using System;

namespace LokiGraf.Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                var config = new ConfigurationBuilder()
                                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                .Build();

                var servicesProvider = BuildDi(config);

                using (servicesProvider as IDisposable)
                {
                    var person = servicesProvider.GetRequiredService<LoggerTest>();
                    person.Log();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        private static IServiceProvider BuildDi(IConfiguration config)
        {
            return new ServiceCollection()
                  .AddTransient<LoggerTest>()
                  .AddLogging(loggingBuilder =>
                  {
                      loggingBuilder.ClearProviders();
                      loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                      loggingBuilder.AddNLog(config);
                  })
                  .BuildServiceProvider();
        }
    }

    internal class LoggerTest
    {
        private readonly ILogger<LoggerTest> _logger;

        public LoggerTest(ILogger<LoggerTest> logger)
        {
            _logger = logger;
        }

        public string Parameter { get; set; }

        public void Log()
        {
            Parameter = "hello world";
            _logger.LogInformation("{@parameter}", Parameter);
        }
    }
}