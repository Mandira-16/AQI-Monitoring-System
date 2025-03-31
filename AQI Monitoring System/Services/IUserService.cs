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

        List<User> GetAllUsers();
        User? GetUserById(int id);
        void UpdateUser(User user);
        void DeleteUser(int id);
    }
}