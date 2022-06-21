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
                    var person = servicesProvider.GetRequiredService<Person>();
                    person.Name = "Sky";
                    person.Talk("Hello");
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
                  //Add DI Classes here
                  .AddTransient<Person>()
                  .AddLogging(loggingBuilder =>
                  {
                      // configure Logging with NLog
                      loggingBuilder.ClearProviders();
                      loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                      loggingBuilder.AddNLog(config);
                  })
                  .BuildServiceProvider();
        }
    }

    public class Person
    {
        private readonly ILogger<Person> _logger;

        public string Name { get; set; }

        public Person(ILogger<Person> logger)
        {
            _logger = logger;
        }

        public void Talk(string text)
        {
            _logger.LogInformation("Person {name} spoke {text}", Name, text);
        }
    }
}