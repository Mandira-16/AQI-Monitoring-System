using AQI_Monitoring_System.Models;
using AQI_Monitoring_System.Data; // Adjust namespace for your DbContext

namespace AQI_Monitoring_System.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public User GetUserByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }

        public bool VerifyPassword(User user, string password)
        {
            // In a real application, use a proper password verification method
            // For example with BCrypt:
            // return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            // Simple example (NOT for production)
            return user.PasswordHash == password;
        }
    }
}