using System.ComponentModel;
using System.Text.Json;
using Agent.Common;
using Agent.Models;
using Agent.Models.Sms;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

namespace Agent.Plugins;

/// <summary>
/// Plugin for BICS SMS API - SMS messaging services
/// </summary>
[Description("BICS SMS API plugin for sending and receiving SMS messages, managing subscriptions, and tracking delivery status")]
public class SmsPlugin
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SmsPlugin> _logger;
    private readonly string _baseUrl;

    public SmsPlugin(HttpClient httpClient, ILogger<SmsPlugin> logger, IOptions<ApiSettings> apiSettings)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _baseUrl = apiSettings.Value.SmsApi.BaseUrl;
        
        if (string.IsNullOrEmpty(_baseUrl))
        {
            throw new ArgumentException("SMS API base URL is not configured", nameof(apiSettings));
        }
    }

    /// <summary>
    /// Sends an SMS message to one or more recipients
    /// </summary>
    /// <param name="senderAddress">The sender address/phone number</param>
    /// <param name="messageRequestJson">JSON string containing the SMS message request details</param>
    /// <returns>Result of the SMS send operation including request ID</returns>
    [KernelFunction, Description("Send an SMS message to one or more recipients with optional delivery receipt request")]
    public async Task<string> SendMessageAsync(
        [Description("The sender address or phone number")] string senderAddress,
        [Description("JSON string containing SMS message request with addresses, message text, and optional receipt settings")] string messageRequestJson)
    {
        try
        {
            ErrorHandler.ValidateRequired(nameof(senderAddress), senderAddress);
            ErrorHandler.ValidateJson(nameof(messageRequestJson), messageRequestJson);
            
            _logger.LogInformation("Sending SMS message from {SenderAddress}", senderAddress);

            var content = new StringContent(messageRequestJson, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/outbound/{senderAddress}/requests", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "SendMessage", errorContent);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "SendMessage", senderAddress);
        }
    }

    /// <summary>
    /// Gets delivery information for a specific SMS request
    /// </summary>
    /// <param name="senderAddress">The sender address used for the original message</param>
    /// <param name="requestId">The request ID of the SMS message</param>
    /// <returns>Delivery status information for the SMS message</returns>
    [KernelFunction, Description("Get delivery status information for a previously sent SMS message")]
    public async Task<string> GetDeliveryInfosAsync(
        [Description("The sender address used for the original message")] string senderAddress,
        [Description("The request ID of the SMS message")] string requestId)
    {
        try
        {
            ErrorHandler.ValidateRequired(nameof(senderAddress), senderAddress);
            ErrorHandler.ValidateRequired(nameof(requestId), requestId);
            
            _logger.LogInformation("Getting delivery info for request {RequestId} from {SenderAddress}", requestId, senderAddress);

            var response = await _httpClient.GetAsync($"{_baseUrl}/outbound/{senderAddress}/requests/{requestId}/deliveryInfos");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "GetDeliveryInfos", errorContent);
            }

            var content = await response.Content.ReadAsStringAsync();
            var deliveryInfoList = JsonSerializer.Deserialize<DeliveryInfoList>(content);
            
            return JsonSerializer.Serialize(deliveryInfoList, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "GetDeliveryInfos", $"{senderAddress}/{requestId}");
        }
    }

    /// <summary>
    /// Gets inbound messages for a specific registered number
    /// </summary>
    /// <param name="inboundNumber">The inbound number to check for messages</param>
    /// <returns>List of inbound SMS messages received for the specified number</returns>
    [KernelFunction, Description("Retrieve inbound SMS messages for a specific registered number")]
    public async Task<string> GetInboundMessagesAsync(
        [Description("The inbound number to check for received messages")] string inboundNumber)
    {
        try
        {
            ErrorHandler.ValidateRequired(nameof(inboundNumber), inboundNumber);
            
            _logger.LogInformation("Getting inbound messages for number {InboundNumber}", inboundNumber);

            var response = await _httpClient.GetAsync($"{_baseUrl}/inbound/registrations/{inboundNumber}/messages");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "GetInboundMessages", errorContent);
            }

            var content = await response.Content.ReadAsStringAsync();
            var messageList = JsonSerializer.Deserialize<InboundMessageList>(content);
            
            return JsonSerializer.Serialize(messageList, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "GetInboundMessages", inboundNumber);
        }
    }

    /// <summary>
    /// Creates a subscription for inbound message notifications
    /// </summary>
    /// <param name="subscriptionRequestJson">JSON string containing subscription details including callback URL</param>
    /// <returns>Result of the subscription creation</returns>
    [KernelFunction, Description("Create a subscription to receive notifications when inbound SMS messages arrive")]
    public async Task<string> CreateInboundSubscriptionAsync(
        [Description("JSON string containing subscription request with callback URL and optional criteria")] string subscriptionRequestJson)
    {
        try
        {
            ErrorHandler.ValidateJson(nameof(subscriptionRequestJson), subscriptionRequestJson);
            
            _logger.LogInformation("Creating inbound message subscription");

            var content = new StringContent(subscriptionRequestJson, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/inbound/subscriptions", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "CreateInboundSubscription", errorContent);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "CreateInboundSubscription");
        }
    }

    /// <summary>
    /// Gets the current inbound message subscription details
    /// </summary>
    /// <returns>Current inbound message subscription configuration</returns>
    [KernelFunction, Description("Get details of the current inbound message subscription")]
    public async Task<string> GetInboundSubscriptionAsync()
    {
        try
        {
            _logger.LogInformation("Getting inbound message subscription");

            var response = await _httpClient.GetAsync($"{_baseUrl}/inbound/subscriptions");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "GetInboundSubscription", errorContent);
            }

            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "GetInboundSubscription");
        }
    }

    /// <summary>
    /// Deletes the inbound message subscription
    /// </summary>
    /// <returns>Result of the subscription deletion</returns>
    [KernelFunction, Description("Delete the current inbound message subscription")]
    public async Task<string> DeleteInboundSubscriptionAsync()
    {
        try
        {
            _logger.LogInformation("Deleting inbound message subscription");

            var response = await _httpClient.DeleteAsync($"{_baseUrl}/inbound/subscriptions");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "DeleteInboundSubscription", errorContent);
            }

            return "Inbound message subscription successfully deleted";
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "DeleteInboundSubscription");
        }
    }

    /// <summary>
    /// Creates a subscription for delivery information notifications
    /// </summary>
    /// <param name="subscriptionRequestJson">JSON string containing delivery subscription details</param>
    /// <returns>Result of the delivery subscription creation</returns>
    [KernelFunction, Description("Create a subscription to receive delivery status notifications for sent SMS messages")]
    public async Task<string> CreateDeliverySubscriptionAsync(
        [Description("JSON string containing delivery subscription request with callback URL")] string subscriptionRequestJson)
    {
        try
        {
            ErrorHandler.ValidateJson(nameof(subscriptionRequestJson), subscriptionRequestJson);
            
            _logger.LogInformation("Creating delivery info subscription");

            var content = new StringContent(subscriptionRequestJson, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/outbound/subscriptions", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "CreateDeliverySubscription", errorContent);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "CreateDeliverySubscription");
        }
    }

    /// <summary>
    /// Gets the current delivery information subscription details
    /// </summary>
    /// <returns>Current delivery information subscription configuration</returns>
    [KernelFunction, Description("Get details of the current delivery information subscription")]
    public async Task<string> GetDeliverySubscriptionAsync()
    {
        try
        {
            _logger.LogInformation("Getting delivery info subscription");

            var response = await _httpClient.GetAsync($"{_baseUrl}/outbound/subscriptions");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "GetDeliverySubscription", errorContent);
            }

            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "GetDeliverySubscription");
        }
    }

    /// <summary>
    /// Deletes the delivery information subscription
    /// </summary>
    /// <returns>Result of the delivery subscription deletion</returns>
    [KernelFunction, Description("Delete the current delivery information subscription")]
    public async Task<string> DeleteDeliverySubscriptionAsync()
    {
        try
        {
            _logger.LogInformation("Deleting delivery info subscription");

            var response = await _httpClient.DeleteAsync($"{_baseUrl}/outbound/subscriptions");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "DeleteDeliverySubscription", errorContent);
            }

            return "Delivery information subscription successfully deleted";
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "DeleteDeliverySubscription");
        }
    }

    /// <summary>
    /// Processes SMS conversion information for analytics and tracking
    /// </summary>
    /// <param name="conversionInfoJson">JSON string containing conversion metrics and data</param>
    /// <returns>Result of the conversion processing</returns>
    [KernelFunction, Description("Process SMS conversion information for tracking campaign effectiveness and analytics")]
    public async Task<string> ProcessConversionInfoAsync(
        [Description("JSON string containing conversion information including message IDs, conversion types, and values")] string conversionInfoJson)
    {
        try
        {
            ErrorHandler.ValidateJson(nameof(conversionInfoJson), conversionInfoJson);
            
            _logger.LogInformation("Processing SMS conversion info");

            var content = new StringContent(conversionInfoJson, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/conversions", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "ProcessConversionInfo", errorContent);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "ProcessConversionInfo");
        }
    }

    /// <summary>
    /// Processes batch SMS conversion information for multiple messages
    /// </summary>
    /// <param name="conversionInfosJson">JSON string containing batch conversion data</param>
    /// <returns>Result of the batch conversion processing</returns>
    [KernelFunction, Description("Process batch SMS conversion information for multiple messages and campaigns")]
    public async Task<string> ProcessConversionInfosAsync(
        [Description("JSON string containing batch conversion information for multiple messages")] string conversionInfosJson)
    {
        try
        {
            ErrorHandler.ValidateJson(nameof(conversionInfosJson), conversionInfosJson);
            
            _logger.LogInformation("Processing batch SMS conversion infos");

            var content = new StringContent(conversionInfosJson, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/conversionsinfos", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "ProcessConversionInfos", errorContent);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "ProcessConversionInfos");
        }
    }
}