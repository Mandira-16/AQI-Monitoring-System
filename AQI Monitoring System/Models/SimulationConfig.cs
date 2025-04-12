// Models/SimulationConfig.cs
namespace AQI_Monitoring_System.Models
{
    public class SimulationConfig
    {
        public int Id { get; set; }
        public int FrequencyMinutes { get; set; }
        public int BaselineAqi { get; set; }
        public int VariationRange { get; set; }
		public double BaselinePm25 { get; set; }
		public double VariationPm25 { get; set; }
		public double BaselinePm10 { get; set; }
		public double VariationPm10 { get; set; }
		public double BaselineO3 { get; set; }
		public double VariationO3 { get; set; }
		public double BaselineNo2 { get; set; }
		public double VariationNo2 { get; set; }
		public double BaselineSo2 { get; set; }
		public double VariationSo2 { get; set; }
		public double BaselineCo { get; set; }
		public double VariationCo { get; set; }
	}
}