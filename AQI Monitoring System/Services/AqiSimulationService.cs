// Services/AqiSimulationService.cs (Updated)
using AQI_Monitoring_System.Data;
using AQI_Monitoring_System.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AQI_Monitoring_System.Services
{
    public class AqiSimulationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AqiSimulationService> _logger;
        private bool _isRunning = false;

        public AqiSimulationService(IServiceProvider serviceProvider, ILogger<AqiSimulationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public bool IsRunning => _isRunning;

        public int GenerationIntervalMinutes
        {
            get
            {
                using var scope = _serviceProvider.CreateScope();
                var configService = scope.ServiceProvider.GetRequiredService<ISimulationConfigService>();
                return configService.GetConfig().FrequencyMinutes;
            }
        }

        public int BaselineAqi
        {
            get
            {
                using var scope = _serviceProvider.CreateScope();
                var configService = scope.ServiceProvider.GetRequiredService<ISimulationConfigService>();
                return configService.GetConfig().BaselineAqi;
            }
        }

        public int MaxVariation
        {
            get
            {
                using var scope = _serviceProvider.CreateScope();
                var configService = scope.ServiceProvider.GetRequiredService<ISimulationConfigService>();
                return configService.GetConfig().VariationRange;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_isRunning)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var configService = scope.ServiceProvider.GetRequiredService<ISimulationConfigService>();
                    var config = configService.GetConfig();
                    var sensors = dbContext.Sensors.Where(s => s.IsActive).ToList();
                    var random = new Random();

                    foreach (var sensor in sensors)
                    {
                        int variation = random.Next(-config.VariationRange / 2, config.VariationRange / 2);
                        int rawAqi = config.BaselineAqi + variation;
                        int aqi = Math.Max(0, Math.Min(500, rawAqi));
                        if (rawAqi != aqi)
                        {
                            _logger.LogWarning($"AQI adjusted from {rawAqi} to {aqi} for sensor {sensor.SensorId}");
                        }

                        dbContext.AqiReadings.Add(new AqiReading
                        {
                            SensorId = sensor.SensorId,
                            Aqi = aqi,
                            RecordedAt = DateTime.UtcNow
                        });
                    }
                    await dbContext.SaveChangesAsync(stoppingToken);
                }
                using var delayScope = _serviceProvider.CreateScope();
                var delayConfigService = delayScope.ServiceProvider.GetRequiredService<ISimulationConfigService>();
                await Task.Delay(TimeSpan.FromMinutes(delayConfigService.GetConfig().FrequencyMinutes), stoppingToken);
            }
        }

        public void StartSimulation()
        {
            _isRunning = true;
        }

        public void StopSimulation()
        {
            _isRunning = false;
        }
    }
}