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
			_logger.LogInformation("AqiSimulationService is starting.");

			while (!stoppingToken.IsCancellationRequested)
			{
				if (_isRunning)
				{
					try
					{
						using var scope = _serviceProvider.CreateScope();
						var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
						var configService = scope.ServiceProvider.GetRequiredService<ISimulationConfigService>();
						var config = configService.GetConfig();
						var sensors = dbContext.Sensors.Where(s => s.IsActive).ToList();
						var random = new Random();

						// Adjust baselines based on time of day (e.g., rush hour)
						var now = DateTime.UtcNow;
						double timeFactor = 1.0;
						if (now.Hour >= 7 && now.Hour <= 9 || now.Hour >= 17 && now.Hour <= 19) // Morning and evening rush hours
						{
							timeFactor = 1.5; // Increase pollution by 50% during rush hours
						}

						if (!sensors.Any())
						{
							_logger.LogWarning("No active sensors found for data simulation.");
						}
						else
						{
							foreach (var sensor in sensors)
							{
								// Simulate AQI
								int aqiVariation = random.Next(-config.VariationRange / 2, config.VariationRange / 2);
								int rawAqi = (int)(config.BaselineAqi * timeFactor) + aqiVariation;
								int aqi = Math.Max(0, Math.Min(500, rawAqi));
								if (rawAqi != aqi)
								{
									_logger.LogWarning($"AQI adjusted from {rawAqi} to {aqi} for sensor {sensor.SensorId}");
								}

								// Simulate PM2.5
								double pm25Variation = random.NextDouble() * config.VariationPm25 - (config.VariationPm25 / 2);
								double pm25 = Math.Max(0, config.BaselinePm25 * timeFactor + pm25Variation);

								// Simulate PM10 (ensure it's generally higher than PM2.5)
								double pm10Variation = random.NextDouble() * config.VariationPm10 - (config.VariationPm10 / 2);
								double pm10 = Math.Max(0, (config.BaselinePm10 * timeFactor + pm10Variation) * 1.2);

								// Simulate O3
								double o3Variation = random.NextDouble() * config.VariationO3 - (config.VariationO3 / 2);
								double o3 = Math.Max(0, config.BaselineO3 * timeFactor + o3Variation);

								// Simulate NO2
								double no2Variation = random.NextDouble() * config.VariationNo2 - (config.VariationNo2 / 2);
								double no2 = Math.Max(0, config.BaselineNo2 * timeFactor + no2Variation);

								// Simulate SO2
								double so2Variation = random.NextDouble() * config.VariationSo2 - (config.VariationSo2 / 2);
								double so2 = Math.Max(0, config.BaselineSo2 * timeFactor + so2Variation);

								// Simulate CO
								double coVariation = random.NextDouble() * config.VariationCo - (config.VariationCo / 2);
								double co = Math.Max(0, config.BaselineCo * timeFactor + coVariation);

								var reading = new AqiReading
								{
									SensorId = sensor.SensorId,
									Aqi = aqi,
									Pm25 = Math.Round(pm25, 2), // Round to 2 decimal places
									Pm10 = Math.Round(pm10, 2),
									O3 = Math.Round(o3, 2),
									No2 = Math.Round(no2, 2),
									So2 = Math.Round(so2, 2),
									Co = Math.Round(co, 2),
									RecordedAt = DateTime.UtcNow
								};
								dbContext.AqiReadings.Add(reading);
								_logger.LogInformation($"Generated reading for sensor {sensor.SensorId}: AQI={aqi}, PM2.5={pm25}, PM10={pm10}, O3={o3}, NO2={no2}, SO2={so2}, CO={co}, RecordedAt={reading.RecordedAt:yyyy-MM-dd HH:mm:ss}");
							}

							await dbContext.SaveChangesAsync(stoppingToken);
							_logger.LogInformation($"Successfully generated data for {sensors.Count} sensors.");
						}
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Error during AQI simulation.");
					}
				}

				using var delayScope = _serviceProvider.CreateScope();
				var delayConfigService = delayScope.ServiceProvider.GetRequiredService<ISimulationConfigService>();
				await Task.Delay(TimeSpan.FromMinutes(delayConfigService.GetConfig().FrequencyMinutes), stoppingToken);
			}

			_logger.LogInformation("AqiSimulationService is stopping.");
		}

		public void StartSimulation()
		{
			_isRunning = true;
			_logger.LogInformation("AQI simulation started.");
		}

		public void StopSimulation()
		{
			_isRunning = false;
			_logger.LogInformation("AQI simulation stopped.");
		}
	}
}