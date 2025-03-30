using AQI_Monitoring_System.Data;
using AQI_Monitoring_System.Models;
using BCrypt.Net;
using System.Linq;

namespace AQI_Monitoring_System.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;

        public UserService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public User? GetUserByUsername(string username)
        {
            return _dbContext.Users.FirstOrDefault(u => u.Username == username);
        }

        public bool VerifyPassword(User user, string password)
        {
            if (user == null || string.IsNullOrEmpty(user.PasswordHash)) return false;
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

        public bool UsernameExists(string username)
        {
            return _dbContext.Users.Any(u => u.Username == username);
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public void AddUser(User user)
        {
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
        }
        public List<User> GetAllUsers()
        {
            return _dbContext.Users.ToList();
        }

        public User? GetUserById(int id)
        {
            return _dbContext.Users.Find(id);
        }

        public void UpdateUser(User user)
        {
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
        }

        public void DeleteUser(int id)
        {
            var user = _dbContext.Users.Find(id);
            if (user != null)
            {
                _dbContext.Users.Remove(user);
                _dbContext.SaveChanges();
            }
        }
    }
}