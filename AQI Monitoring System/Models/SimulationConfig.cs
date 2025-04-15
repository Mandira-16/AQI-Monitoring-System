using System.ComponentModel.DataAnnotations; // Required for validation attributes

namespace AQI_Monitoring_System.Models
{
	public class SimulationConfig
	{
		public int Id { get; set; }

		[Range(1, 60, ErrorMessage = "Frequency must be between 1 and 60 minutes.")]
		public int FrequencyMinutes { get; set; }

		[Range(0, 500, ErrorMessage = "Baseline AQI must be between 0 and 500.")]
		public int BaselineAqi { get; set; }

		[Range(0, 500, ErrorMessage = "Variation Range must be between 0 and 500.")]
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