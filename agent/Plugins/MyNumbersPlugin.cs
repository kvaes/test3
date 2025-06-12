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
/// Plugin for BICS MyNumbers API - Core number management services
/// </summary>
[Description("BICS MyNumbers API plugin for telephone number provisioning, configuration, and lifecycle management")]
public class MyNumbersPlugin
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MyNumbersPlugin> _logger;
    private readonly string _baseUrl;

    public MyNumbersPlugin(HttpClient httpClient, ILogger<MyNumbersPlugin> logger, IOptions<ApiSettings> apiSettings)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _baseUrl = apiSettings.Value.MyNumbersApi.BaseUrl;
        
        if (string.IsNullOrEmpty(_baseUrl))
        {
            throw new ArgumentException("MyNumbers API base URL is not configured", nameof(apiSettings));
        }
    }

    /// <summary>
    /// Gets a list of all telephone numbers for the customer
    /// </summary>
    /// <returns>List of telephone numbers with their status and configuration</returns>
    [KernelFunction, Description("Get a list of all telephone numbers managed by the customer")]
    public async Task<string> GetNumbersAsync()
    {
        try
        {
            _logger.LogInformation("Getting all numbers");

            var response = await _httpClient.GetAsync($"{_baseUrl}/numbers");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "GetNumbers", errorContent);
            }

            var content = await response.Content.ReadAsStringAsync();
            var numberList = JsonSerializer.Deserialize<NumberListResponse>(content);
            
            return JsonSerializer.Serialize(numberList, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "GetNumbers");
        }
    }

    /// <summary>
    /// Gets detailed information about a specific telephone number
    /// </summary>
    /// <param name="phoneNumber">The telephone number to query</param>
    /// <returns>Detailed number information including configuration and features</returns>
    [KernelFunction, Description("Get detailed information about a specific telephone number including configuration and enabled features")]
    public async Task<string> GetNumberAsync(
        [Description("The telephone number to query (e.g., +1234567890)")] string phoneNumber)
    {
        try
        {
            ErrorHandler.ValidateRequired(nameof(phoneNumber), phoneNumber);
            
            _logger.LogInformation("Getting number details for {PhoneNumber}", phoneNumber);

            var response = await _httpClient.GetAsync($"{_baseUrl}/numbers/{phoneNumber}");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "GetNumber", errorContent);
            }

            var content = await response.Content.ReadAsStringAsync();
            var number = JsonSerializer.Deserialize<Number>(content);
            
            return JsonSerializer.Serialize(number, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "GetNumber", phoneNumber);
        }
    }

    /// <summary>
    /// Provisions a new telephone number with specified configuration
    /// </summary>
    /// <param name="numberRequestJson">JSON string containing number provisioning request</param>
    /// <returns>Result of the number provisioning operation</returns>
    [KernelFunction, Description("Provision a new telephone number with specified region, service type, and capabilities")]
    public async Task<string> ProvisionNumberAsync(
        [Description("JSON string containing number provisioning request with region, serviceType, capabilities, and other configuration")] string numberRequestJson)
    {
        try
        {
            ErrorHandler.ValidateJson(nameof(numberRequestJson), numberRequestJson);
            
            _logger.LogInformation("Provisioning new number");

            var content = new StringContent(numberRequestJson, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/numbers", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "ProvisionNumber", errorContent);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "ProvisionNumber");
        }
    }

    /// <summary>
    /// Updates the configuration of an existing telephone number
    /// </summary>
    /// <param name="phoneNumber">The telephone number to update</param>
    /// <param name="configurationJson">JSON string containing updated configuration</param>
    /// <returns>Result of the configuration update</returns>
    [KernelFunction, Description("Update the configuration of an existing telephone number including features and routing settings")]
    public async Task<string> UpdateNumberConfigurationAsync(
        [Description("The telephone number to update")] string phoneNumber,
        [Description("JSON string containing updated configuration including features, routing, and other settings")] string configurationJson)
    {
        try
        {
            ErrorHandler.ValidateRequired(nameof(phoneNumber), phoneNumber);
            ErrorHandler.ValidateJson(nameof(configurationJson), configurationJson);
            
            _logger.LogInformation("Updating configuration for number {PhoneNumber}", phoneNumber);

            var content = new StringContent(configurationJson, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_baseUrl}/numbers/{phoneNumber}/configuration", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "UpdateNumberConfiguration", errorContent);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "UpdateNumberConfiguration", phoneNumber);
        }
    }

    /// <summary>
    /// Releases a telephone number from service
    /// </summary>
    /// <param name="phoneNumber">The telephone number to release</param>
    /// <returns>Result of the number release operation</returns>
    [KernelFunction, Description("Release a telephone number from service and return it to the available pool")]
    public async Task<string> ReleaseNumberAsync(
        [Description("The telephone number to release from service")] string phoneNumber)
    {
        try
        {
            ErrorHandler.ValidateRequired(nameof(phoneNumber), phoneNumber);
            
            _logger.LogInformation("Releasing number {PhoneNumber}", phoneNumber);

            var response = await _httpClient.DeleteAsync($"{_baseUrl}/numbers/{phoneNumber}");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "ReleaseNumber", errorContent);
            }

            return $"Number {phoneNumber} has been successfully released from service";
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "ReleaseNumber", phoneNumber);
        }
    }

    /// <summary>
    /// Gets available telephone numbers for provisioning in a specific region
    /// </summary>
    /// <param name="region">The region or area code to search for available numbers</param>
    /// <param name="count">Number of available numbers to return (optional, default 10)</param>
    /// <returns>List of available telephone numbers for provisioning</returns>
    [KernelFunction, Description("Search for available telephone numbers that can be provisioned in a specific region or area code")]
    public async Task<string> GetAvailableNumbersAsync(
        [Description("The region, area code, or location to search for available numbers")] string region,
        [Description("Number of available numbers to return (optional, default 10)")] int count = 10)
    {
        try
        {
            ErrorHandler.ValidateRequired(nameof(region), region);
            
            _logger.LogInformation("Getting available numbers in region {Region}", region);

            var response = await _httpClient.GetAsync($"{_baseUrl}/available-numbers?region={region}&count={count}");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "GetAvailableNumbers", errorContent);
            }

            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "GetAvailableNumbers", region);
        }
    }

    /// <summary>
    /// Gets the current status and health of the MyNumbers service
    /// </summary>
    /// <returns>Service status and health information</returns>
    [KernelFunction, Description("Get the current operational status and health of the MyNumbers service")]
    public async Task<string> GetServiceStatusAsync()
    {
        try
        {
            _logger.LogInformation("Getting service status");

            var response = await _httpClient.GetAsync($"{_baseUrl}/status");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "GetServiceStatus", errorContent);
            }

            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "GetServiceStatus");
        }
    }
}