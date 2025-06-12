namespace Agent.Models;

public class ApiSettings
{
    public ConnectApiSettings ConnectApi { get; set; } = new();
    public MyNumbersApiSettings MyNumbersApi { get; set; } = new();
    public MyNumbersAddressManagementApiSettings MyNumbersAddressManagementApi { get; set; } = new();
    public MyNumbersCDRApiSettings MyNumbersCDRApi { get; set; } = new();
    public MyNumbersDisconnectionApiSettings MyNumbersDisconnectionApi { get; set; } = new();
    public MyNumbersEmergencyServicesApiSettings MyNumbersEmergencyServicesApi { get; set; } = new();
    public MyNumbersNumberPortingApiSettings MyNumbersNumberPortingApi { get; set; } = new();
    public SmsApiSettings SmsApi { get; set; } = new();
}

public class ConnectApiSettings
{
    public string BaseUrl { get; set; } = string.Empty;
}

public class MyNumbersApiSettings
{
    public string BaseUrl { get; set; } = string.Empty;
}

public class MyNumbersAddressManagementApiSettings
{
    public string BaseUrl { get; set; } = string.Empty;
}

public class MyNumbersCDRApiSettings
{
    public string BaseUrl { get; set; } = string.Empty;
}

public class MyNumbersDisconnectionApiSettings
{
    public string BaseUrl { get; set; } = string.Empty;
}

public class MyNumbersEmergencyServicesApiSettings
{
    public string BaseUrl { get; set; } = string.Empty;
}

public class MyNumbersNumberPortingApiSettings
{
    public string BaseUrl { get; set; } = string.Empty;
}

public class SmsApiSettings
{
    public string BaseUrl { get; set; } = string.Empty;
}