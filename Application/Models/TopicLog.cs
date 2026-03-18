namespace UmweltMonitor3000.Application.Models;

public class TopicLog
{
    public Guid Id { get; set; }
    public DateOnly TimeStamp { get; set; }
    public string LogTyp { get; set; }
    public string Direction { get; set; }
    public string Topic { get; set; }
    public string Payload { get; set; }
}