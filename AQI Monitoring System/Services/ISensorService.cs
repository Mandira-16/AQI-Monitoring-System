// Services/ISensorService.cs
using AQI_Monitoring_System.Models;
using System.Collections.Generic;

namespace AQI_Monitoring_System.Services
{
    public interface ISensorService
    {
        List<Sensor> GetAllSensors();
        Sensor? GetSensorById(string id);
        void AddSensor(Sensor sensor);
        void UpdateSensor(Sensor sensor);
        void DeactivateSensor(string id);
        void DeleteSensor(string id);
    }
}