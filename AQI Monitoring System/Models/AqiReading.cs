// Models/AqiReading.cs
namespace AQI_Monitoring_System.Models
{
    public class AqiReading
    {
        public int Id { get; set; }
        public int SensorId { get; set; }
        public int Aqi { get; set; }
        public DateTime RecordedAt { get; set; }
        public Sensor? Sensor { get; set; } // Navigation property
    }
}