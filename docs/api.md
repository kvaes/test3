# API Documentation

This document provides comprehensive information about the BICS API integrations available in the Test3 Semantic Kernel Agent.

## Overview

The agent integrates with 8 different BICS APIs through native Semantic Kernel plugins. Each API is implemented as a separate plugin with full error handling, logging, and configuration management.

## Supported APIs

### 1. Connect API

**Base URL:** `https://api.bics.com/connect/v1`

**Description:** The Cloud Connect Service provides multi-cloud connectivity solutions with layer 2 transport and dedicated connectivity to Cloud Service Provider (CSP) infrastructure.

**Available Functions:**

- `GetConnectionAsync(connectionId)` - Get details of a specific connection
- `GetConnectionsAsync()` - Get list of all customer connections
- `CreateConnectionAsync(connectionRequestJson)` - Create a new connection
- `ModifyConnectionAsync(connectionId, modificationRequestJson)` - Modify existing connection
- `DeleteConnectionAsync(connectionId)` - Disconnect a connection
- `GetInterconnectsAsync()` - Get list of available interconnects
- `GetReferenceDataAsync()` - Get reference data for configuration

**Example Usage:**
```csharp
// Get connection details
var connectionDetails = await connectPlugin.GetConnectionAsync("12345");

// Create new connection
var connectionRequest = @"{
    ""serviceType"": ""CLOUD_CONNECT"",
    ""cloudServiceProvider"": ""AZURE_ER_C3"",
    ""cloudServiceRegion"": ""WESTEUROPE"",
    ""bandwidth"": ""50MBS"",
    ""contractTerm"": ""1MONTH"",
    ""companyName"": ""Example Company"",
    ""contactEmailAddress"": ""admin@example.com""
}";
var result = await connectPlugin.CreateConnectionAsync(connectionRequest);
```

### 2. MyNumbers API

**Base URL:** `https://api.bics.com/mynumbers/v1`

**Description:** Core number management API for telephone number provisioning, configuration, and lifecycle management.

**Status:** ⏳ Implementation in progress

### 3. MyNumbers Address Management API

**Base URL:** `https://api.bics.com/mynumbers/v1/address-management`

**Description:** API for managing address information associated with telephone numbers for emergency services and regulatory compliance.

**Status:** ⏳ Implementation in progress

### 4. MyNumbers CDR API

**Base URL:** `https://api.bics.com/mynumbers/v1/cdr`

**Description:** Call Detail Records (CDR) API for accessing call usage data, billing information, and usage analytics.

**Status:** ⏳ Implementation in progress

### 5. MyNumbers Disconnection API

**Base URL:** `https://api.bics.com/mynumbers/v1/disconnection`

**Description:** API for managing number disconnection processes, including temporary and permanent disconnections.

**Status:** ⏳ Implementation in progress

### 6. MyNumbers Emergency Services API

**Base URL:** `https://api.bics.com/mynumbers/v1/emergency-services`

**Description:** API for configuring emergency services settings and managing E911/E112 compliance requirements.

**Status:** ⏳ Implementation in progress

### 7. MyNumbers Number Porting API

**Base URL:** `https://api.bics.com/mynumbers/v1/number-porting`

**Description:** API for managing number portability operations including port-in and port-out processes.

**Status:** ⏳ Implementation in progress

### 8. SMS API

**Base URL:** `https://api.bics.com/sms/v1`

**Description:** SMS messaging API for sending and receiving text messages through BICS SMS gateway.

**Status:** ⏳ Implementation in progress

## Configuration

### API Settings

All API base URLs are configured in `appsettings.json`:

```json
{
  "ApiSettings": {
    "ConnectApi": {
      "BaseUrl": "https://api.bics.com/connect/v1"
    },
    "MyNumbersApi": {
      "BaseUrl": "https://api.bics.com/mynumbers/v1"
    },
    // ... other API configurations
  }
}
```

### Environment Variables

You can override API settings using environment variables:

- `ApiSettings__ConnectApi__BaseUrl`
- `ApiSettings__MyNumbersApi__BaseUrl`
- etc.

### Authentication

Currently, the APIs expect authentication to be handled through:

1. **API Keys** - Set via HTTP headers
2. **OAuth 2.0** - Bearer tokens
3. **Client Certificates** - For enterprise customers

Configure authentication in your deployment environment or extend the plugins to support specific authentication methods.

## Error Handling

All plugins implement comprehensive error handling:

### Error Types

1. **Validation Errors** - Invalid parameters or missing required fields
2. **Network Errors** - Connection timeouts, DNS resolution failures
3. **HTTP Errors** - 400, 401, 403, 404, 500, etc.
4. **Serialization Errors** - Invalid JSON responses or malformed data

### Error Response Format

```json
{
  "error": "Error type and description",
  "details": "Additional context information",
  "timestamp": "2024-01-01T00:00:00Z",
  "operation": "Function that failed"
}
```

## Data Models

### Connect API Models

**Connection Model:**
```csharp
public class Connection
{
    public string Id { get; set; }
    public string ServiceType { get; set; }
    public string CloudServiceProvider { get; set; }
    public string CloudServiceRegion { get; set; }
    public string Bandwidth { get; set; }
    public string Status { get; set; }
    public List<ConnectionLine> Lines { get; set; }
    // ... additional properties
}
```

**Interconnect Model:**
```csharp
public class Interconnect
{
    public string Id { get; set; }
    public string Type { get; set; }
    public string City { get; set; }
    public string Datacenter { get; set; }
    public string Capacity { get; set; }
    public string Status { get; set; }
    // ... additional properties
}
```

## Usage Examples

### Semantic Kernel Integration

```csharp
// Register plugins
kernel.Plugins.AddFromType<ConnectPlugin>();
kernel.Plugins.AddFromType<MyNumbersPlugin>();
// ... other plugins

// Use in a conversation
var result = await kernel.InvokeAsync("ConnectPlugin", "GetConnectionAsync", 
    new KernelArguments { ["connectionId"] = "12345" });
```

### Direct Plugin Usage

```csharp
// Inject plugin via DI
var connectPlugin = serviceProvider.GetRequiredService<ConnectPlugin>();

// Call functions directly
var connections = await connectPlugin.GetConnectionsAsync();
```

## Rate Limiting

Each API has different rate limits:

- **Connect API**: 100 requests/minute
- **MyNumbers APIs**: 200 requests/minute
- **SMS API**: 1000 requests/hour

The plugins automatically handle rate limit responses (HTTP 429) and can be configured with retry policies.

## Monitoring and Logging

### Built-in Logging

All plugins use structured logging with these levels:

- **Information**: Successful operations
- **Warning**: Recoverable errors, retries
- **Error**: Failed operations, exceptions
- **Debug**: Detailed request/response data

### Health Checks

Implement health checks for each API:

```csharp
services.AddHealthChecks()
    .AddCheck<ConnectApiHealthCheck>("connect-api")
    .AddCheck<MyNumbersApiHealthCheck>("mynumbers-api");
```

### Metrics

Track key metrics:

- Request count per API
- Response times
- Error rates
- Rate limit hits

## Testing

### Unit Tests

```csharp
[Test]
public async Task GetConnection_ValidId_ReturnsConnection()
{
    // Arrange
    var plugin = new ConnectPlugin(mockHttpClient, mockLogger, mockOptions);
    
    // Act
    var result = await plugin.GetConnectionAsync("12345");
    
    // Assert
    Assert.IsNotNull(result);
    // ... additional assertions
}
```

### Integration Tests

```csharp
[Test]
public async Task ConnectPlugin_RealApi_Integration()
{
    // Test against sandbox/staging environment
    var plugin = CreateRealPlugin();
    var result = await plugin.GetReferenceDataAsync();
    Assert.IsNotNull(result);
}
```

## Security Considerations

1. **API Keys**: Never commit API keys to source control
2. **TLS**: All APIs use HTTPS with TLS 1.2+
3. **Input Validation**: All inputs are validated before sending to APIs
4. **Output Sanitization**: API responses are validated and sanitized
5. **Audit Logging**: All API calls are logged for security auditing

## Troubleshooting

### Common Issues

1. **Configuration Errors**
   - Check `appsettings.json` for correct base URLs
   - Verify environment variables are set correctly

2. **Authentication Failures**
   - Verify API keys or certificates
   - Check token expiration

3. **Network Issues**
   - Test connectivity to API endpoints
   - Check firewall and proxy settings

4. **Rate Limiting**
   - Implement exponential backoff
   - Consider request batching

### Debug Mode

Enable detailed logging in development:

```json
{
  "Logging": {
    "LogLevel": {
      "Agent.Plugins": "Debug",
      "System.Net.Http": "Debug"
    }
  }
}
```

This will log all HTTP requests and responses for debugging purposes.

## Future Enhancements

- [ ] Implement retry policies with exponential backoff
- [ ] Add request/response caching
- [ ] Implement API versioning support
- [ ] Add batch operation support
- [ ] Implement webhook handlers for async operations
- [ ] Add API usage analytics and reporting
- [ ] Implement circuit breaker pattern for resilience