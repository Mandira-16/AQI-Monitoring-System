// Models/AqiReading.cs
namespace AQI_Monitoring_System.Models
{
    public class AqiReading
    {
        public int Id { get; set; }
        public string SensorId { get; set; } = string.Empty; //Foreign key string
        public int Aqi { get; set; }
        public DateTime RecordedAt { get; set; }
        public virtual Sensor? Sensor { get; set; } // Navigation property
        //public double? Pm25 { get; set; } // Added for pollutant support
    }
}