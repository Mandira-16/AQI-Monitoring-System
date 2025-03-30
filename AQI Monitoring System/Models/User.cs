namespace AQI_Monitoring_System.Models
{
    public class User
    {
        public int Id { get; set; } // Primary key
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // Added Role property
    }
}