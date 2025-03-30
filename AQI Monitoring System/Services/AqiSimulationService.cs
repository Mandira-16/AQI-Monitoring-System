// Services/AqiSimulationService.cs
using AQI_Monitoring_System.Data;
using AQI_Monitoring_System.Models;
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

        public AqiSimulationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public bool IsRunning => _isRunning;
        public int GenerationIntervalMinutes => _generationIntervalMinutes;
        public int BaselineAqi => _baselineAqi;
        public int MaxVariation => _maxVariation;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
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
                            int variation = random.Next(-_maxVariation / 2, _maxVariation / 2);
                            int aqi = Math.Max(0, Math.Min(500, _baselineAqi + variation));
                            dbContext.AqiReadings.Add(new AqiReading
                            {
                                SensorId = sensor.Id,
                                Aqi = aqi,
                                RecordedAt = DateTime.UtcNow
                            });
                        }
                        await dbContext.SaveChangesAsync();
                    }
                }
                await Task.Delay(TimeSpan.FromMinutes(_generationIntervalMinutes), stoppingToken);
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
            _generationIntervalMinutes = Math.Max(1, Math.Min(15, intervalMinutes));
            _baselineAqi = Math.Max(0, Math.Min(500, baselineAqi));
            _maxVariation = Math.Max(0, Math.Min(500, maxVariation));
        }
    }
}