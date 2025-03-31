// Services/ISimulationConfigService.cs
using AQI_Monitoring_System.Models;
namespace AQI_Monitoring_System.Services
{
    public interface ISimulationConfigService
    {
        SimulationConfig GetConfig();
        void UpdateConfig(SimulationConfig config);
    }
}