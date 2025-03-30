// Models/AlertThreshold.cs
namespace AQI_Monitoring_System.Models
{
    public class AlertThreshold
    {
        public int Id { get; set; }
        public string Category { get; set; } = string.Empty; // e.g., "Good", "Moderate", "Unhealthy"
        public int MinAqi { get; set; }     // Minimum AQI value for this category
        public int MaxAqi { get; set; }     // Maximum AQI value for this category
        public string Color { get; set; } = string.Empty;   // CSS color for display (e.g., "green", "#FFA500")
    }
}