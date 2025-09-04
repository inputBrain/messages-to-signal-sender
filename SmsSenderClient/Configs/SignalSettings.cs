namespace SmsSenderClient.Configs;

public class SignalSettings
{
    public string BaseUrl { get; set; } = "http://localhost:8080";
    public string SendMessageFrom { get; set; } = "";
    
    public string SendMessageTo { get; set; } = "";
}