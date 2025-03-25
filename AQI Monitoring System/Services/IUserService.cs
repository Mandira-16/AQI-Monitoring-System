using AQI_Monitoring_System.Models;

namespace AQI_Monitoring_System.Services
{
    public interface IUserService
    {
        User GetUserByUsername(string username);
        bool VerifyPassword(User user, string password);
    }
}