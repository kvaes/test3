using System.Text.Json.Serialization;

namespace Agent.Models.Sms;

public class SmsMessage
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("address")]
    public List<string> Address { get; set; } = new();

    [JsonPropertyName("senderAddress")]
    public string SenderAddress { get; set; } = string.Empty;

    [JsonPropertyName("outboundSMSMessageRequest")]
    public OutboundSmsMessageRequest OutboundSmsMessageRequest { get; set; } = new();
}

public class OutboundSmsMessageRequest
{
    [JsonPropertyName("address")]
    public List<string> Address { get; set; } = new();

    [JsonPropertyName("senderAddress")]
    public string SenderAddress { get; set; } = string.Empty;

    [JsonPropertyName("outboundSMSTextMessage")]
    public OutboundSmsTextMessage OutboundSmsTextMessage { get; set; } = new();

    [JsonPropertyName("receiptRequest")]
    public ReceiptRequest? ReceiptRequest { get; set; }

    [JsonPropertyName("senderName")]
    public string? SenderName { get; set; }

    [JsonPropertyName("clientCorrelator")]
    public string? ClientCorrelator { get; set; }
}

public class OutboundSmsTextMessage
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

public class ReceiptRequest
{
    [JsonPropertyName("notifyURL")]
    public string NotifyUrl { get; set; } = string.Empty;

    [JsonPropertyName("callbackData")]
    public string? CallbackData { get; set; }
}

public class InboundMessage
{
    [JsonPropertyName("dateTime")]
    public DateTime DateTime { get; set; }

    [JsonPropertyName("destinationAddress")]
    public string DestinationAddress { get; set; } = string.Empty;

    [JsonPropertyName("messageId")]
    public string MessageId { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("senderAddress")]
    public string SenderAddress { get; set; } = string.Empty;
}

public class DeliveryInfo
{
    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;

    [JsonPropertyName("deliveryStatus")]
    public string DeliveryStatus { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

public class Subscription
{
    [JsonPropertyName("callbackReference")]
    public CallbackReference CallbackReference { get; set; } = new();

    [JsonPropertyName("criteria")]
    public string? Criteria { get; set; }

    [JsonPropertyName("resourceURL")]
    public string? ResourceUrl { get; set; }
}

public class CallbackReference
{
    [JsonPropertyName("notifyURL")]
    public string NotifyUrl { get; set; } = string.Empty;

    [JsonPropertyName("callbackData")]
    public string? CallbackData { get; set; }
}

public class SubscriptionRequest
{
    [JsonPropertyName("subscription")]
    public Subscription Subscription { get; set; } = new();
}

public class ConversionInfo
{
    [JsonPropertyName("conversions")]
    public List<Conversion> Conversions { get; set; } = new();
}

public class Conversion
{
    [JsonPropertyName("messageId")]
    public string MessageId { get; set; } = string.Empty;

    [JsonPropertyName("conversionMetrics")]
    public ConversionMetrics ConversionMetrics { get; set; } = new();
}

public class ConversionMetrics
{
    [JsonPropertyName("conversionType")]
    public string ConversionType { get; set; } = string.Empty;

    [JsonPropertyName("conversionValue")]
    public decimal ConversionValue { get; set; }

    [JsonPropertyName("conversionCurrency")]
    public string? ConversionCurrency { get; set; }
}

public class SmsResponse
{
    [JsonPropertyName("resourceReference")]
    public ResourceReference? ResourceReference { get; set; }

    [JsonPropertyName("requestId")]
    public string? RequestId { get; set; }
}

public class ResourceReference
{
    [JsonPropertyName("resourceURL")]
    public string ResourceUrl { get; set; } = string.Empty;
}

public class InboundMessageList
{
    [JsonPropertyName("inboundSMSMessage")]
    public List<InboundMessage> InboundSmsMessages { get; set; } = new();

    [JsonPropertyName("numberOfMessagesInThisBatch")]
    public int NumberOfMessagesInThisBatch { get; set; }

    [JsonPropertyName("resourceURL")]
    public string? ResourceUrl { get; set; }

    [JsonPropertyName("totalNumberOfPendingMessages")]
    public int? TotalNumberOfPendingMessages { get; set; }
}

public class DeliveryInfoList
{
    [JsonPropertyName("deliveryInfo")]
    public List<DeliveryInfo> DeliveryInfos { get; set; } = new();
}