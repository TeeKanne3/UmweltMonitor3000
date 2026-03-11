using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UmweltMonitor3000.Application.Interfaces;

public interface IUmweltMonitorRepository
{
    Task SaveSensorDataAsync(string sensorId, string data);
    Task<string> GetSensorDataAsync(string sensorId);
}