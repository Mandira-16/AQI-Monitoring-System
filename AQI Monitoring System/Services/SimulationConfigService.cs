using AQI_Monitoring_System.Data;
using AQI_Monitoring_System.Models;
using Microsoft.Extensions.Logging;

namespace AQI_Monitoring_System.Services
{
    public class SimulationConfigService : ISimulationConfigService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<SimulationConfigService> _logger;

        public SimulationConfigService(ApplicationDbContext dbContext, ILogger<SimulationConfigService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public SimulationConfig GetConfig()
        {
            var config = _dbContext.SimulationConfigs.FirstOrDefault();
            bool isInvalid = config == null ||
                             config.BaselinePm25 <= 0 || config.VariationPm25 <= 0 ||
                             config.BaselinePm10 <= 0 || config.VariationPm10 <= 0 ||
                             config.BaselineO3 <= 0 || config.VariationO3 <= 0 ||
                             config.BaselineNo2 <= 0 || config.VariationNo2 <= 0 ||
                             config.BaselineSo2 <= 0 || config.VariationSo2 <= 0 ||
                             config.BaselineCo <= 0 || config.VariationCo <= 0;

            if (isInvalid)
            {
                config = new SimulationConfig
                {
                    FrequencyMinutes = 1,
                    BaselineAqi = 50,
                    VariationRange = 20,
                    BaselinePm25 = 20.0,
                    VariationPm25 = 10.0,
                    BaselinePm10 = 30.0,
                    VariationPm10 = 15.0,
                    BaselineO3 = 40.0,
                    VariationO3 = 20.0,
                    BaselineNo2 = 15.0,
                    VariationNo2 = 10.0,
                    BaselineSo2 = 5.0,
                    VariationSo2 = 5.0,
                    BaselineCo = 1.0,
                    VariationCo = 0.5
                };
                if (_dbContext.SimulationConfigs.Any())
                {
                    var existingConfig = _dbContext.SimulationConfigs.First();
                    existingConfig.FrequencyMinutes = config.FrequencyMinutes;
                    existingConfig.BaselineAqi = config.BaselineAqi;
                    existingConfig.VariationRange = config.VariationRange;
                    existingConfig.BaselinePm25 = config.BaselinePm25;
                    existingConfig.VariationPm25 = config.VariationPm25;
                    existingConfig.BaselinePm10 = config.BaselinePm10;
                    existingConfig.VariationPm10 = config.VariationPm10;
                    existingConfig.BaselineO3 = config.BaselineO3;
                    existingConfig.VariationO3 = config.VariationO3;
                    existingConfig.BaselineNo2 = config.BaselineNo2;
                    existingConfig.VariationNo2 = config.VariationNo2;
                    existingConfig.BaselineSo2 = config.BaselineSo2;
                    existingConfig.VariationSo2 = config.VariationSo2;
                    existingConfig.BaselineCo = config.BaselineCo;
                    existingConfig.VariationCo = config.VariationCo;
                    _dbContext.SimulationConfigs.Update(existingConfig);
                }
                else
                {
                    _dbContext.SimulationConfigs.Add(config);
                }
                _dbContext.SaveChanges();
                _logger.LogInformation("Reset simulation config to defaults due to invalid values.");
            }
            return config;
        }
        public void UpdateConfig(SimulationConfig config)
        {
            var existingConfig = _dbContext.SimulationConfigs.FirstOrDefault();
            if (existingConfig == null)
            {
                config.Id = 1; // Assuming a single config with ID 1
                _dbContext.SimulationConfigs.Add(config);
            }
            else
            {
                existingConfig.FrequencyMinutes = config.FrequencyMinutes;
                existingConfig.BaselineAqi = config.BaselineAqi;
                existingConfig.VariationRange = config.VariationRange;
                existingConfig.BaselinePm25 = config.BaselinePm25;
                existingConfig.VariationPm25 = config.VariationPm25;
                existingConfig.BaselinePm10 = config.BaselinePm10;
                existingConfig.VariationPm10 = config.VariationPm10;
                existingConfig.BaselineO3 = config.BaselineO3;
                existingConfig.VariationO3 = config.VariationO3;
                existingConfig.BaselineNo2 = config.BaselineNo2;
                existingConfig.VariationNo2 = config.VariationNo2;
                existingConfig.BaselineSo2 = config.BaselineSo2;
                existingConfig.VariationSo2 = config.VariationSo2;
                existingConfig.BaselineCo = config.BaselineCo;
                existingConfig.VariationCo = config.VariationCo;
                _dbContext.SimulationConfigs.Update(existingConfig);
            }
            _dbContext.SaveChanges();
            _logger.LogInformation("Simulation config updated successfully.");
        }

    }
}