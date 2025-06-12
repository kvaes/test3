using System.ComponentModel;
using System.Text.Json;
using Agent.Common;
using Agent.Models;
using Agent.Models.Connect;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

namespace Agent.Plugins;

/// <summary>
/// Plugin for BICS Connect API - Cloud Connect Service management
/// </summary>
[Description("BICS Connect API plugin for managing cloud connectivity services, connections, and interconnects")]
public class ConnectPlugin
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ConnectPlugin> _logger;
    private readonly string _baseUrl;

    public ConnectPlugin(HttpClient httpClient, ILogger<ConnectPlugin> logger, IOptions<ApiSettings> apiSettings)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _baseUrl = apiSettings.Value.ConnectApi.BaseUrl;
        
        if (string.IsNullOrEmpty(_baseUrl))
        {
            throw new ArgumentException("Connect API base URL is not configured", nameof(apiSettings));
        }
    }

    /// <summary>
    /// Gets details of a specific connection by its ID
    /// </summary>
    /// <param name="connectionId">The unique identifier of the connection</param>
    /// <returns>Connection details including service type, provider, region, bandwidth, and status</returns>
    [KernelFunction, Description("Get detailed information about a specific connection including configuration, status, and line details")]
    public async Task<string> GetConnectionAsync(
        [Description("The unique identifier of the connection")] string connectionId)
    {
        try
        {
            ErrorHandler.ValidateRequired(nameof(connectionId), connectionId);
            
            _logger.LogInformation("Getting connection details for ID: {ConnectionId}", connectionId);

            var response = await _httpClient.GetAsync($"{_baseUrl}/connections/{connectionId}");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "GetConnection", errorContent);
            }

            var content = await response.Content.ReadAsStringAsync();
            var connection = JsonSerializer.Deserialize<Connection>(content);
            
            return JsonSerializer.Serialize(connection, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "GetConnection", connectionId);
        }
    }

    /// <summary>
    /// Gets a list of all connections for the customer
    /// </summary>
    /// <returns>List of all customer connections with their basic information</returns>
    [KernelFunction, Description("Get a list of all connections for the current customer")]
    public async Task<string> GetConnectionsAsync()
    {
        try
        {
            _logger.LogInformation("Getting all connections");

            var response = await _httpClient.GetAsync($"{_baseUrl}/connections");
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to get connections. Status: {StatusCode}", response.StatusCode);
                return $"Error: Failed to retrieve connections. Status: {response.StatusCode}";
            }

            var content = await response.Content.ReadAsStringAsync();
            var connectionList = JsonSerializer.Deserialize<ConnectionListResponse>(content);
            
            return JsonSerializer.Serialize(connectionList, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting connections");
            return $"Error: {ex.Message}";
        }
    }

    /// <summary>
    /// Creates a new connection with the specified configuration
    /// </summary>
    /// <param name="connectionRequestJson">JSON string containing the connection request details</param>
    /// <returns>Result of the connection creation request</returns>
    [KernelFunction, Description("Create a new connection with specified service type, provider, region, bandwidth, and other configuration details")]
    public async Task<string> CreateConnectionAsync(
        [Description("JSON string containing connection request details including serviceType, cloudServiceProvider, cloudServiceRegion, bandwidth, contractTerm, protectionScheme, companyName, customerComment, customerPurchaseId, contactEmailAddress, and lines array")] 
        string connectionRequestJson)
    {
        try
        {
            _logger.LogInformation("Creating new connection");

            var connectionRequest = JsonSerializer.Deserialize<ConnectionRequest>(connectionRequestJson);
            if (connectionRequest == null)
            {
                return "Error: Invalid connection request JSON";
            }

            var json = JsonSerializer.Serialize(connectionRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/connections", content);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to create connection. Status: {StatusCode}", response.StatusCode);
                var errorContent = await response.Content.ReadAsStringAsync();
                return $"Error: Failed to create connection. Status: {response.StatusCode}. Details: {errorContent}";
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating connection");
            return $"Error: {ex.Message}";
        }
    }

    /// <summary>
    /// Modifies an existing connection configuration
    /// </summary>
    /// <param name="connectionId">The unique identifier of the connection to modify</param>
    /// <param name="modificationRequestJson">JSON string containing the modification details</param>
    /// <returns>Result of the connection modification request</returns>
    [KernelFunction, Description("Modify an existing connection's configuration such as bandwidth, contract terms, or other attributes")]
    public async Task<string> ModifyConnectionAsync(
        [Description("The unique identifier of the connection to modify")] string connectionId,
        [Description("JSON string containing modification request details")] string modificationRequestJson)
    {
        try
        {
            _logger.LogInformation("Modifying connection {ConnectionId}", connectionId);

            var content = new StringContent(modificationRequestJson, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/connections/{connectionId}/modify", content);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to modify connection {ConnectionId}. Status: {StatusCode}", 
                    connectionId, response.StatusCode);
                var errorContent = await response.Content.ReadAsStringAsync();
                return $"Error: Failed to modify connection {connectionId}. Status: {response.StatusCode}. Details: {errorContent}";
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error modifying connection {ConnectionId}", connectionId);
            return $"Error: {ex.Message}";
        }
    }

    /// <summary>
    /// Disconnects and removes a connection
    /// </summary>
    /// <param name="connectionId">The unique identifier of the connection to disconnect</param>
    /// <returns>Result of the disconnection request</returns>
    [KernelFunction, Description("Disconnect and remove a connection from service")]
    public async Task<string> DeleteConnectionAsync(
        [Description("The unique identifier of the connection to disconnect")] string connectionId)
    {
        try
        {
            _logger.LogInformation("Deleting connection {ConnectionId}", connectionId);

            var response = await _httpClient.DeleteAsync($"{_baseUrl}/connections/{connectionId}");
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to delete connection {ConnectionId}. Status: {StatusCode}", 
                    connectionId, response.StatusCode);
                var errorContent = await response.Content.ReadAsStringAsync();
                return $"Error: Failed to delete connection {connectionId}. Status: {response.StatusCode}. Details: {errorContent}";
            }

            return $"Connection {connectionId} has been successfully scheduled for disconnection";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting connection {ConnectionId}", connectionId);
            return $"Error: {ex.Message}";
        }
    }

    /// <summary>
    /// Gets a list of all available interconnects for the customer
    /// </summary>
    /// <returns>List of interconnects with their locations, capacities, and availability status</returns>
    [KernelFunction, Description("Get a list of available interconnects showing locations, datacenters, capacities, and current status")]
    public async Task<string> GetInterconnectsAsync()
    {
        try
        {
            _logger.LogInformation("Getting interconnects");

            var response = await _httpClient.GetAsync($"{_baseUrl}/interconnects");
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to get interconnects. Status: {StatusCode}", response.StatusCode);
                return $"Error: Failed to retrieve interconnects. Status: {response.StatusCode}";
            }

            var content = await response.Content.ReadAsStringAsync();
            var interconnectList = JsonSerializer.Deserialize<InterconnectListResponse>(content);
            
            return JsonSerializer.Serialize(interconnectList, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting interconnects");
            return $"Error: {ex.Message}";
        }
    }

    /// <summary>
    /// Gets reference data including available service types, providers, regions, bandwidths, and other configuration options
    /// </summary>
    /// <returns>Reference data for configuring connections</returns>
    [KernelFunction, Description("Get reference data including available service types, cloud providers, regions, bandwidth options, contract terms, and protection schemes")]
    public async Task<string> GetReferenceDataAsync()
    {
        try
        {
            _logger.LogInformation("Getting reference data");

            var response = await _httpClient.GetAsync($"{_baseUrl}/refdata");
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to get reference data. Status: {StatusCode}", response.StatusCode);
                return $"Error: Failed to retrieve reference data. Status: {response.StatusCode}";
            }

            var content = await response.Content.ReadAsStringAsync();
            var referenceData = JsonSerializer.Deserialize<ReferenceData>(content);
            
            return JsonSerializer.Serialize(referenceData, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reference data");
            return $"Error: {ex.Message}";
        }
    }
}