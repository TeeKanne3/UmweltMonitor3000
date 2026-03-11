using UmweltMonitor3000.Application.Interfaces;

namespace UmweltMonitor3000.Application.Repositories;

public class UmweltMonitorRepository : IUmweltMonitorRepository
{
    public Task SaveSensorDataAsync(string sensorId, string data)
    {
        throw new NotImplementedException();
    }
    public Task<string> GetSensorDataAsync(string sensorId)
    {
        throw new NotImplementedException();
    }
}