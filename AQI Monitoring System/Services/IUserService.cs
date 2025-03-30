using AQI_Monitoring_System.Models;

namespace AQI_Monitoring_System.Services
{
    public interface IUserService
    {
        User? GetUserByUsername(string username);
        bool VerifyPassword(User user, string password);
        bool UsernameExists(string username);
        string HashPassword(string password);
        void AddUser(User user);
    }
    //CHANGED A FEW TO FIX THE ERRORS - TEMPORARY 
}