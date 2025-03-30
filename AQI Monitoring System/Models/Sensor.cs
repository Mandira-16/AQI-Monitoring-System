namespace AQI_Monitoring_System.Models
{
    public class Sensor
    {
        public int Id { get; set; }
        public string SensorId { get; set; } = string.Empty; // Unique identifier for the sensor
        public string Location { get; set; } = string.Empty; // e.g., "Colombo Fort"
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsActive { get; set; } = true; // Active or inactive status
    }
}