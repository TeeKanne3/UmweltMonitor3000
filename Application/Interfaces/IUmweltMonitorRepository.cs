using UmweltMonitor3000.Application.Models;

namespace UmweltMonitor3000.Application.Interfaces;

public interface IUmweltMonitorRepository
{
    event Action<string>? LogMessage;
    Task SaveSensorDataAsync(string sensorId, string data);
    Task<string> GetSensorDataAsync(string sensorId);
    Task<List<SensorData>> GetAllSensorDataByIdAsync(string sensorId);
}