// Services/SimulationConfigService.cs
using AQI_Monitoring_System.Data;
using AQI_Monitoring_System.Models;

namespace AQI_Monitoring_System.Services
{
    public class SimulationConfigService : ISimulationConfigService
    {
        private readonly ApplicationDbContext _dbContext;

        public SimulationConfigService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public SimulationConfig GetConfig()
        {
            // Return the first config (assuming only one global config for simplicity)
            return _dbContext.SimulationConfigs.FirstOrDefault() ??
                   new SimulationConfig { FrequencyMinutes = 10, BaselineAqi = 50, VariationRange = 20 };
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
                _dbContext.SimulationConfigs.Update(existingConfig);
            }
            _dbContext.SaveChanges();
        }
    }
}