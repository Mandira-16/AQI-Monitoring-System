// Models/SimulationConfig.cs
namespace AQI_Monitoring_System.Models
{
    public class SimulationConfig
    {
        public int Id { get; set; }
        public int FrequencyMinutes { get; set; } // Frequency of data generation in minutes (e.g., 5, 10, 15)
        public int BaselineAqi { get; set; }      // Starting AQI value
        public int VariationRange { get; set; }   // Maximum deviation from baseline (e.g., ±20)
    }
}