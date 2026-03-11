using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UmweltMonitor3000.Application.Interfaces;

namespace UmweltMonitor3000.Application.Services;

public class MqttService : IMqttService
{
    public Task ConnectAsync(string brokerAddress, int brokerPort, string clientId, string username, string password)
    {
        throw new NotImplementedException();
    }

    public Task DisconnectAsync()
    {
        throw new NotImplementedException();
    }

    public Task PublishAsync(string topic, string payload)
    {
        throw new NotImplementedException();
    }

    public Task SubscribeAsync(string topic)
    {
        throw new NotImplementedException();
    }
}
