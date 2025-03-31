// Models/SimulationConfig.cs
namespace AQI_Monitoring_System.Models
{
    public class SimulationConfig
    {
        public int Id { get; set; }
        public int FrequencyMinutes { get; set; }
        public int BaselineAqi { get; set; }
        public int VariationRange { get; set; }
    }
}