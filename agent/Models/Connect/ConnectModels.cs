using System.Text.Json.Serialization;

namespace Agent.Models.Connect;

public class Connection
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("serviceType")]
    public string ServiceType { get; set; } = string.Empty;

    [JsonPropertyName("cloudServiceProvider")]
    public string CloudServiceProvider { get; set; } = string.Empty;

    [JsonPropertyName("cloudServiceProviderLabel")]
    public string CloudServiceProviderLabel { get; set; } = string.Empty;

    [JsonPropertyName("cloudServiceRegion")]
    public string CloudServiceRegion { get; set; } = string.Empty;

    [JsonPropertyName("cloudServiceRegionLabel")]
    public string CloudServiceRegionLabel { get; set; } = string.Empty;

    [JsonPropertyName("orderId")]
    public int OrderId { get; set; }

    [JsonPropertyName("price")]
    public string Price { get; set; } = string.Empty;

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonPropertyName("currencyLabel")]
    public string CurrencyLabel { get; set; } = string.Empty;

    [JsonPropertyName("bandwidth")]
    public string Bandwidth { get; set; } = string.Empty;

    [JsonPropertyName("bandwidthLabel")]
    public string BandwidthLabel { get; set; } = string.Empty;

    [JsonPropertyName("contractTerm")]
    public string ContractTerm { get; set; } = string.Empty;

    [JsonPropertyName("contractTermLabel")]
    public string ContractTermLabel { get; set; } = string.Empty;

    [JsonPropertyName("protectionScheme")]
    public string ProtectionScheme { get; set; } = string.Empty;

    [JsonPropertyName("protectionSchemeLabel")]
    public string ProtectionSchemeLabel { get; set; } = string.Empty;

    [JsonPropertyName("companyName")]
    public string CompanyName { get; set; } = string.Empty;

    [JsonPropertyName("customerComment")]
    public string CustomerComment { get; set; } = string.Empty;

    [JsonPropertyName("customerPurchaseId")]
    public string CustomerPurchaseId { get; set; } = string.Empty;

    [JsonPropertyName("contactEmailAddress")]
    public string ContactEmailAddress { get; set; } = string.Empty;

    [JsonPropertyName("activationDate")]
    public string ActivationDate { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("additionalAttributes")]
    public Dictionary<string, object> AdditionalAttributes { get; set; } = new();

    [JsonPropertyName("lines")]
    public List<ConnectionLine> Lines { get; set; } = new();
}

public class ConnectionLine
{
    [JsonPropertyName("lineType")]
    public string LineType { get; set; } = string.Empty;

    [JsonPropertyName("customerReference")]
    public string CustomerReference { get; set; } = string.Empty;

    [JsonPropertyName("additionalAttributes")]
    public Dictionary<string, object> AdditionalAttributes { get; set; } = new();

    [JsonPropertyName("interconnect")]
    public Interconnect Interconnect { get; set; } = new();

    [JsonPropertyName("vlan")]
    public int Vlan { get; set; }

    [JsonPropertyName("cloudServiceKey")]
    public string CloudServiceKey { get; set; } = string.Empty;
}

public class Interconnect
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;

    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    [JsonPropertyName("countryIso3")]
    public string CountryIso3 { get; set; } = string.Empty;

    [JsonPropertyName("houser")]
    public string Houser { get; set; } = string.Empty;

    [JsonPropertyName("datacenter")]
    public string Datacenter { get; set; } = string.Empty;

    [JsonPropertyName("capacity")]
    public string Capacity { get; set; } = string.Empty;

    [JsonPropertyName("capacityLabel")]
    public string CapacityLabel { get; set; } = string.Empty;

    [JsonPropertyName("usage")]
    public string? Usage { get; set; }

    [JsonPropertyName("portMode")]
    public string? PortMode { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("portTag")]
    public string? PortTag { get; set; }

    [JsonPropertyName("additionalData")]
    public Dictionary<string, object>? AdditionalData { get; set; }
}

public class ConnectionRequest
{
    [JsonPropertyName("serviceType")]
    public string ServiceType { get; set; } = string.Empty;

    [JsonPropertyName("cloudServiceProvider")]
    public string CloudServiceProvider { get; set; } = string.Empty;

    [JsonPropertyName("cloudServiceRegion")]
    public string CloudServiceRegion { get; set; } = string.Empty;

    [JsonPropertyName("bandwidth")]
    public string Bandwidth { get; set; } = string.Empty;

    [JsonPropertyName("contractTerm")]
    public string ContractTerm { get; set; } = string.Empty;

    [JsonPropertyName("protectionScheme")]
    public string ProtectionScheme { get; set; } = string.Empty;

    [JsonPropertyName("companyName")]
    public string CompanyName { get; set; } = string.Empty;

    [JsonPropertyName("customerComment")]
    public string CustomerComment { get; set; } = string.Empty;

    [JsonPropertyName("customerPurchaseId")]
    public string CustomerPurchaseId { get; set; } = string.Empty;

    [JsonPropertyName("contactEmailAddress")]
    public string ContactEmailAddress { get; set; } = string.Empty;

    [JsonPropertyName("lines")]
    public List<ConnectionLineRequest> Lines { get; set; } = new();
}

public class ConnectionLineRequest
{
    [JsonPropertyName("lineType")]
    public string LineType { get; set; } = string.Empty;

    [JsonPropertyName("customerReference")]
    public string CustomerReference { get; set; } = string.Empty;

    [JsonPropertyName("interconnectId")]
    public string InterconnectId { get; set; } = string.Empty;

    [JsonPropertyName("vlan")]
    public int Vlan { get; set; }

    [JsonPropertyName("additionalAttributes")]
    public Dictionary<string, object> AdditionalAttributes { get; set; } = new();
}

public class ReferenceData
{
    [JsonPropertyName("data")]
    public Dictionary<string, object> Data { get; set; } = new();
}

public class ConnectionListResponse
{
    [JsonPropertyName("connections")]
    public List<Connection> Connections { get; set; } = new();

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }
}

public class InterconnectListResponse
{
    [JsonPropertyName("interconnects")]
    public List<Interconnect> Interconnects { get; set; } = new();

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }
}