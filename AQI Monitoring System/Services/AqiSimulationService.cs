// Services/AqiSimulationService.cs
using AQI_Monitoring_System.Data;
using AQI_Monitoring_System.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AQI_Monitoring_System.Services
{
    public class AqiSimulationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private bool _isRunning = false;
        private int _generationIntervalMinutes = 5;
        private int _baselineAqi = 50;
        private int _maxVariation = 100;

        private volatile int _currentIntervalMinutes;
        private volatile int _currentBaselineAqi;
        private volatile int _currentMaxVariation;

        public AqiSimulationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _currentIntervalMinutes = _generationIntervalMinutes;
            _currentBaselineAqi = _baselineAqi;
            _currentMaxVariation = _maxVariation;
        }

        public bool IsRunning => _isRunning;
        public int GenerationIntervalMinutes => _currentIntervalMinutes;
        public int BaselineAqi => _currentBaselineAqi;
        public int MaxVariation => _currentMaxVariation;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var config = dbContext.SimulationConfigs.FirstOrDefault();
                if (config != null)
                {
                    _currentIntervalMinutes = config.FrequencyMinutes;
                    _currentBaselineAqi = config.BaselineAqi;
                    _currentMaxVariation = config.VariationRange;
                }
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                if (_isRunning)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        var sensors = dbContext.Sensors.Where(s => s.IsActive).ToList();
                        var random = new Random();

                        foreach (var sensor in sensors)
                        {
                            int variation = random.Next(-_currentMaxVariation / 2, _currentMaxVariation / 2);
                            int aqi = Math.Max(0, Math.Min(500, _currentBaselineAqi + variation));
                            dbContext.AqiReadings.Add(new AqiReading
                            {
                                SensorId = sensor.SensorId, // Corrected to string
                                Aqi = aqi,
                                RecordedAt = DateTime.UtcNow
                            });
                        }
                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                }
                await Task.Delay(TimeSpan.FromMinutes(_currentIntervalMinutes), stoppingToken);
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

        public void ConfigureSimulation(int intervalMinutes, int baselineAqi, int maxVariation)
        {
            _currentIntervalMinutes = Math.Max(1, Math.Min(15, intervalMinutes));
            _currentBaselineAqi = Math.Max(0, Math.Min(500, baselineAqi));
            _currentMaxVariation = Math.Max(0, Math.Min(500, maxVariation));
        }
    }
}