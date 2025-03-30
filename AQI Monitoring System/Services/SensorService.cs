// Services/SensorService.cs
using AQI_Monitoring_System.Data;
using AQI_Monitoring_System.Models;
using System.Collections.Generic;
using System.Linq;

namespace AQI_Monitoring_System.Services
{
    public class SensorService : ISensorService
    {
        private readonly ApplicationDbContext _dbContext;

        public SensorService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Sensor> GetAllSensors()
        {
            return _dbContext.Sensors.ToList();
        }

        public Sensor? GetSensorById(string id)
        {
            return _dbContext.Sensors.FirstOrDefault(s => s.SensorId == id);
        }

        public void AddSensor(Sensor sensor)
        {
            _dbContext.Sensors.Add(sensor);
            _dbContext.SaveChanges();
        }

        public void UpdateSensor(Sensor sensor)
        {
            _dbContext.Sensors.Update(sensor);
            _dbContext.SaveChanges();
        }

        public void DeactivateSensor(string id)
        {
            var sensor = _dbContext.Sensors.FirstOrDefault(s => s.SensorId == id);
            if (sensor != null)
            {
                sensor.IsActive = false;
                _dbContext.SaveChanges();
            }
        }
        public void DeleteSensor(string id)
        {
            var sensor = _dbContext.Sensors.FirstOrDefault(s => s.SensorId == id);
            if (sensor != null)
            {
                _dbContext.Sensors.Remove(sensor);
                // Optional: Remove related readings
                // var readings = _dbContext.AqiReadings.Where(r => r.SensorId == id);
                // _dbContext.AqiReadings.RemoveRange(readings);
                _dbContext.SaveChanges();
            }
        }
    }
}