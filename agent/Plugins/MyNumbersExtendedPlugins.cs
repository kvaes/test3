using System.ComponentModel;
using System.Text.Json;
using Agent.Common;
using Agent.Models;
using Agent.Models.MyNumbers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

namespace Agent.Plugins;

/// <summary>
/// Plugin for BICS MyNumbers Address Management API
/// </summary>
[Description("BICS MyNumbers Address Management API plugin for managing address information associated with telephone numbers")]
public class MyNumbersAddressManagementPlugin
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MyNumbersAddressManagementPlugin> _logger;
    private readonly string _baseUrl;

    public MyNumbersAddressManagementPlugin(HttpClient httpClient, ILogger<MyNumbersAddressManagementPlugin> logger, IOptions<ApiSettings> apiSettings)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _baseUrl = apiSettings.Value.MyNumbersAddressManagementApi.BaseUrl;
        
        if (string.IsNullOrEmpty(_baseUrl))
        {
            throw new ArgumentException("MyNumbers Address Management API base URL is not configured", nameof(apiSettings));
        }
    }

    [KernelFunction, Description("Get address information for a specific telephone number")]
    public async Task<string> GetAddressAsync(
        [Description("The telephone number to get address information for")] string phoneNumber)
    {
        try
        {
            ErrorHandler.ValidateRequired(nameof(phoneNumber), phoneNumber);
            _logger.LogInformation("Getting address for number {PhoneNumber}", phoneNumber);

            var response = await _httpClient.GetAsync($"{_baseUrl}/addresses/{phoneNumber}");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "GetAddress", errorContent);
            }

            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "GetAddress", phoneNumber);
        }
    }

    [KernelFunction, Description("Update address information for a telephone number")]
    public async Task<string> UpdateAddressAsync(
        [Description("The telephone number to update")] string phoneNumber,
        [Description("JSON string containing address information")] string addressJson)
    {
        try
        {
            ErrorHandler.ValidateRequired(nameof(phoneNumber), phoneNumber);
            ErrorHandler.ValidateJson(nameof(addressJson), addressJson);
            
            _logger.LogInformation("Updating address for number {PhoneNumber}", phoneNumber);

            var content = new StringContent(addressJson, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_baseUrl}/addresses/{phoneNumber}", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "UpdateAddress", errorContent);
            }

            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "UpdateAddress", phoneNumber);
        }
    }

    [KernelFunction, Description("Validate an address for emergency services compliance")]
    public async Task<string> ValidateAddressAsync(
        [Description("JSON string containing address to validate")] string addressJson)
    {
        try
        {
            ErrorHandler.ValidateJson(nameof(addressJson), addressJson);
            
            _logger.LogInformation("Validating address");

            var content = new StringContent(addressJson, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/addresses/validate", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "ValidateAddress", errorContent);
            }

            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "ValidateAddress");
        }
    }
}

/// <summary>
/// Plugin for BICS MyNumbers CDR API
/// </summary>
[Description("BICS MyNumbers CDR API plugin for accessing call detail records and usage analytics")]
public class MyNumbersCDRPlugin
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MyNumbersCDRPlugin> _logger;
    private readonly string _baseUrl;

    public MyNumbersCDRPlugin(HttpClient httpClient, ILogger<MyNumbersCDRPlugin> logger, IOptions<ApiSettings> apiSettings)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _baseUrl = apiSettings.Value.MyNumbersCDRApi.BaseUrl;
        
        if (string.IsNullOrEmpty(_baseUrl))
        {
            throw new ArgumentException("MyNumbers CDR API base URL is not configured", nameof(apiSettings));
        }
    }

    [KernelFunction, Description("Get call detail records for a specific telephone number")]
    public async Task<string> GetCallDetailRecordsAsync(
        [Description("The telephone number to get CDR data for")] string phoneNumber,
        [Description("Start date for CDR query (YYYY-MM-DD)")] string startDate,
        [Description("End date for CDR query (YYYY-MM-DD)")] string endDate)
    {
        try
        {
            ErrorHandler.ValidateRequired(nameof(phoneNumber), phoneNumber);
            ErrorHandler.ValidateRequired(nameof(startDate), startDate);
            ErrorHandler.ValidateRequired(nameof(endDate), endDate);
            
            _logger.LogInformation("Getting CDR for number {PhoneNumber} from {StartDate} to {EndDate}", phoneNumber, startDate, endDate);

            var response = await _httpClient.GetAsync($"{_baseUrl}/cdr/{phoneNumber}?startDate={startDate}&endDate={endDate}");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "GetCallDetailRecords", errorContent);
            }

            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "GetCallDetailRecords", phoneNumber);
        }
    }

    [KernelFunction, Description("Get usage analytics and statistics for telephone numbers")]
    public async Task<string> GetUsageAnalyticsAsync(
        [Description("The telephone number or range to analyze")] string phoneNumber,
        [Description("Time period for analytics (daily, weekly, monthly)")] string period)
    {
        try
        {
            ErrorHandler.ValidateRequired(nameof(phoneNumber), phoneNumber);
            ErrorHandler.ValidateRequired(nameof(period), period);
            
            _logger.LogInformation("Getting usage analytics for {PhoneNumber} with period {Period}", phoneNumber, period);

            var response = await _httpClient.GetAsync($"{_baseUrl}/analytics/{phoneNumber}?period={period}");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "GetUsageAnalytics", errorContent);
            }

            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "GetUsageAnalytics", phoneNumber);
        }
    }
}

/// <summary>
/// Plugin for BICS MyNumbers Disconnection API
/// </summary>
[Description("BICS MyNumbers Disconnection API plugin for managing number disconnection processes")]
public class MyNumbersDisconnectionPlugin
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MyNumbersDisconnectionPlugin> _logger;
    private readonly string _baseUrl;

    public MyNumbersDisconnectionPlugin(HttpClient httpClient, ILogger<MyNumbersDisconnectionPlugin> logger, IOptions<ApiSettings> apiSettings)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _baseUrl = apiSettings.Value.MyNumbersDisconnectionApi.BaseUrl;
        
        if (string.IsNullOrEmpty(_baseUrl))
        {
            throw new ArgumentException("MyNumbers Disconnection API base URL is not configured", nameof(apiSettings));
        }
    }

    [KernelFunction, Description("Request disconnection of a telephone number")]
    public async Task<string> RequestDisconnectionAsync(
        [Description("JSON string containing disconnection request details")] string disconnectionRequestJson)
    {
        try
        {
            ErrorHandler.ValidateJson(nameof(disconnectionRequestJson), disconnectionRequestJson);
            
            _logger.LogInformation("Requesting number disconnection");

            var content = new StringContent(disconnectionRequestJson, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/disconnection-requests", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "RequestDisconnection", errorContent);
            }

            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "RequestDisconnection");
        }
    }

    [KernelFunction, Description("Get status of a disconnection request")]
    public async Task<string> GetDisconnectionStatusAsync(
        [Description("The disconnection request ID")] string requestId)
    {
        try
        {
            ErrorHandler.ValidateRequired(nameof(requestId), requestId);
            
            _logger.LogInformation("Getting disconnection status for request {RequestId}", requestId);

            var response = await _httpClient.GetAsync($"{_baseUrl}/disconnection-requests/{requestId}");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "GetDisconnectionStatus", errorContent);
            }

            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "GetDisconnectionStatus", requestId);
        }
    }
}

/// <summary>
/// Plugin for BICS MyNumbers Emergency Services API
/// </summary>
[Description("BICS MyNumbers Emergency Services API plugin for configuring emergency services and E911/E112 compliance")]
public class MyNumbersEmergencyServicesPlugin
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MyNumbersEmergencyServicesPlugin> _logger;
    private readonly string _baseUrl;

    public MyNumbersEmergencyServicesPlugin(HttpClient httpClient, ILogger<MyNumbersEmergencyServicesPlugin> logger, IOptions<ApiSettings> apiSettings)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _baseUrl = apiSettings.Value.MyNumbersEmergencyServicesApi.BaseUrl;
        
        if (string.IsNullOrEmpty(_baseUrl))
        {
            throw new ArgumentException("MyNumbers Emergency Services API base URL is not configured", nameof(apiSettings));
        }
    }

    [KernelFunction, Description("Configure emergency services for a telephone number")]
    public async Task<string> ConfigureEmergencyServicesAsync(
        [Description("The telephone number to configure")] string phoneNumber,
        [Description("JSON string containing emergency services configuration")] string configurationJson)
    {
        try
        {
            ErrorHandler.ValidateRequired(nameof(phoneNumber), phoneNumber);
            ErrorHandler.ValidateJson(nameof(configurationJson), configurationJson);
            
            _logger.LogInformation("Configuring emergency services for number {PhoneNumber}", phoneNumber);

            var content = new StringContent(configurationJson, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_baseUrl}/emergency-services/{phoneNumber}", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "ConfigureEmergencyServices", errorContent);
            }

            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "ConfigureEmergencyServices", phoneNumber);
        }
    }

    [KernelFunction, Description("Get emergency services configuration for a telephone number")]
    public async Task<string> GetEmergencyServicesConfigurationAsync(
        [Description("The telephone number to query")] string phoneNumber)
    {
        try
        {
            ErrorHandler.ValidateRequired(nameof(phoneNumber), phoneNumber);
            
            _logger.LogInformation("Getting emergency services configuration for number {PhoneNumber}", phoneNumber);

            var response = await _httpClient.GetAsync($"{_baseUrl}/emergency-services/{phoneNumber}");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "GetEmergencyServicesConfiguration", errorContent);
            }

            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "GetEmergencyServicesConfiguration", phoneNumber);
        }
    }
}

/// <summary>
/// Plugin for BICS MyNumbers Number Porting API
/// </summary>
[Description("BICS MyNumbers Number Porting API plugin for managing number portability operations")]
public class MyNumbersNumberPortingPlugin
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MyNumbersNumberPortingPlugin> _logger;
    private readonly string _baseUrl;

    public MyNumbersNumberPortingPlugin(HttpClient httpClient, ILogger<MyNumbersNumberPortingPlugin> logger, IOptions<ApiSettings> apiSettings)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _baseUrl = apiSettings.Value.MyNumbersNumberPortingApi.BaseUrl;
        
        if (string.IsNullOrEmpty(_baseUrl))
        {
            throw new ArgumentException("MyNumbers Number Porting API base URL is not configured", nameof(apiSettings));
        }
    }

    [KernelFunction, Description("Submit a number porting request")]
    public async Task<string> SubmitPortingRequestAsync(
        [Description("JSON string containing porting request details")] string portingRequestJson)
    {
        try
        {
            ErrorHandler.ValidateJson(nameof(portingRequestJson), portingRequestJson);
            
            _logger.LogInformation("Submitting number porting request");

            var content = new StringContent(portingRequestJson, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/porting-requests", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "SubmitPortingRequest", errorContent);
            }

            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "SubmitPortingRequest");
        }
    }

    [KernelFunction, Description("Get status of a number porting request")]
    public async Task<string> GetPortingStatusAsync(
        [Description("The porting request ID")] string requestId)
    {
        try
        {
            ErrorHandler.ValidateRequired(nameof(requestId), requestId);
            
            _logger.LogInformation("Getting porting status for request {RequestId}", requestId);

            var response = await _httpClient.GetAsync($"{_baseUrl}/porting-requests/{requestId}");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "GetPortingStatus", errorContent);
            }

            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "GetPortingStatus", requestId);
        }
    }

    [KernelFunction, Description("Cancel a pending number porting request")]
    public async Task<string> CancelPortingRequestAsync(
        [Description("The porting request ID to cancel")] string requestId)
    {
        try
        {
            ErrorHandler.ValidateRequired(nameof(requestId), requestId);
            
            _logger.LogInformation("Cancelling porting request {RequestId}", requestId);

            var response = await _httpClient.DeleteAsync($"{_baseUrl}/porting-requests/{requestId}");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "CancelPortingRequest", errorContent);
            }

            return $"Porting request {requestId} has been successfully cancelled";
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "CancelPortingRequest", requestId);
        }
    }
}