using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogDashboardDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _loggerInfo;
        private readonly ILogger _logger;

        public WeatherForecastController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("WeatherForecastController");
            _loggerInfo = loggerFactory.CreateLogger<WeatherForecastController>();
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {

            _logger.LogInformation("LogInformation");
            _logger.LogWarning("LogWarning");
            _logger.LogError("LogError");
            _logger.LogDebug("LogDebug");
            _logger.LogTrace("LogTrace");
            _logger.LogCritical("LogCritical");
            var rng = new Random();

            _loggerInfo.LogInformation("LogInformation");
            _loggerInfo.LogWarning("LogWarning");
            _loggerInfo.LogError("LogError");
            _loggerInfo.LogDebug("LogDebug");
            _loggerInfo.LogTrace("LogTrace");
            _loggerInfo.LogCritical("LogCritical");

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
