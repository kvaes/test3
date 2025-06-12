using System.Text.Json.Serialization;

namespace Agent.Models.MyNumbers;

// Core MyNumbers API Models
public class Number
{
    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("serviceType")]
    public string ServiceType { get; set; } = string.Empty;

    [JsonPropertyName("region")]
    public string Region { get; set; } = string.Empty;

    [JsonPropertyName("capabilities")]
    public List<string> Capabilities { get; set; } = new();

    [JsonPropertyName("activationDate")]
    public DateTime? ActivationDate { get; set; }

    [JsonPropertyName("lastModified")]
    public DateTime? LastModified { get; set; }
}

public class NumberConfiguration
{
    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; } = string.Empty;

    [JsonPropertyName("configuration")]
    public Dictionary<string, object> Configuration { get; set; } = new();

    [JsonPropertyName("features")]
    public NumberFeatures Features { get; set; } = new();
}

public class NumberFeatures
{
    [JsonPropertyName("voiceEnabled")]
    public bool VoiceEnabled { get; set; }

    [JsonPropertyName("smsEnabled")]
    public bool SmsEnabled { get; set; }

    [JsonPropertyName("faxEnabled")]
    public bool FaxEnabled { get; set; }

    [JsonPropertyName("emergencyServicesEnabled")]
    public bool EmergencyServicesEnabled { get; set; }
}

// Address Management Models
public class Address
{
    [JsonPropertyName("addressId")]
    public string AddressId { get; set; } = string.Empty;

    [JsonPropertyName("street")]
    public string Street { get; set; } = string.Empty;

    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    [JsonPropertyName("postalCode")]
    public string PostalCode { get; set; } = string.Empty;

    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;

    [JsonPropertyName("addressType")]
    public string AddressType { get; set; } = string.Empty;
}

// CDR Models
public class CallDetailRecord
{
    [JsonPropertyName("callId")]
    public string CallId { get; set; } = string.Empty;

    [JsonPropertyName("startTime")]
    public DateTime StartTime { get; set; }

    [JsonPropertyName("endTime")]
    public DateTime EndTime { get; set; }

    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    [JsonPropertyName("fromNumber")]
    public string FromNumber { get; set; } = string.Empty;

    [JsonPropertyName("toNumber")]
    public string ToNumber { get; set; } = string.Empty;

    [JsonPropertyName("callType")]
    public string CallType { get; set; } = string.Empty;

    [JsonPropertyName("cost")]
    public decimal Cost { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;
}

// Disconnection Models
public class DisconnectionRequest
{
    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; } = string.Empty;

    [JsonPropertyName("disconnectionType")]
    public string DisconnectionType { get; set; } = string.Empty;

    [JsonPropertyName("requestDate")]
    public DateTime RequestDate { get; set; }

    [JsonPropertyName("effectiveDate")]
    public DateTime EffectiveDate { get; set; }

    [JsonPropertyName("reason")]
    public string Reason { get; set; } = string.Empty;
}

// Emergency Services Models
public class EmergencyServiceConfiguration
{
    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; } = string.Empty;

    [JsonPropertyName("emergencyAddress")]
    public Address EmergencyAddress { get; set; } = new();

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("serviceProvider")]
    public string ServiceProvider { get; set; } = string.Empty;
}

// Number Porting Models
public class PortingRequest
{
    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; } = string.Empty;

    [JsonPropertyName("portingType")]
    public string PortingType { get; set; } = string.Empty;

    [JsonPropertyName("currentProvider")]
    public string CurrentProvider { get; set; } = string.Empty;

    [JsonPropertyName("targetProvider")]
    public string TargetProvider { get; set; } = string.Empty;

    [JsonPropertyName("requestDate")]
    public DateTime RequestDate { get; set; }

    [JsonPropertyName("desiredPortDate")]
    public DateTime DesiredPortDate { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}

// Common Response Models
public class NumberListResponse
{
    [JsonPropertyName("numbers")]
    public List<Number> Numbers { get; set; } = new();

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    [JsonPropertyName("currentPage")]
    public int CurrentPage { get; set; }
}

public class OperationResult
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("operationId")]
    public string? OperationId { get; set; }

    [JsonPropertyName("errors")]
    public List<string> Errors { get; set; } = new();
}