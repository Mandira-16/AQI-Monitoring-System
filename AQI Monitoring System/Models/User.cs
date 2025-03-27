namespace AQI_Monitoring_System.Models
{
    public class User
    {
        public int Id { get; set; } // Primary key
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        // Other user properties
    }
}