// Models/AqiReading.cs
namespace AQI_Monitoring_System.Models
{
	public class AqiReading
	{
		public int Id { get; set; }
		public string SensorId { get; set; } = string.Empty; //Foreign key string
		public int Aqi { get; set; }
		public double? Pm25 { get; set; } // Add PM2.5
		public double? Pm10 { get; set; } // Add PM10
		public double? O3 { get; set; }   // Add O3
		public double? No2 { get; set; }  // Add NO2
		public double? So2 { get; set; }  // Add SO2
		public double? Co { get; set; }   // Add CO
		public DateTime RecordedAt { get; set; }
		public virtual Sensor? Sensor { get; set; } // Navigation property
	}
}