using Microsoft.Extensions.Logging;

namespace Agent.Common;

/// <summary>
/// Helper class for consistent error handling across plugins
/// </summary>
public static class ErrorHandler
{
    /// <summary>
    /// Handles exceptions and returns a consistent error response
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="ex">Exception to handle</param>
    /// <param name="operation">Operation that failed</param>
    /// <param name="context">Additional context information</param>
    /// <returns>Formatted error message</returns>
    public static string HandleException(ILogger logger, Exception ex, string operation, string? context = null)
    {
        var contextInfo = string.IsNullOrEmpty(context) ? "" : $" (Context: {context})";
        logger.LogError(ex, "Error during {Operation}{Context}", operation, contextInfo);
        
        return ex switch
        {
            ArgumentNullException => $"Error: Required parameter is missing for {operation}. {ex.Message}",
            ArgumentException => $"Error: Invalid argument for {operation}. {ex.Message}",
            HttpRequestException => $"Error: Network request failed for {operation}. {ex.Message}",
            TaskCanceledException => $"Error: Request timeout for {operation}. {ex.Message}",
            UnauthorizedAccessException => $"Error: Unauthorized access for {operation}. Please check your API credentials.",
            _ => $"Error: {operation} failed. {ex.Message}"
        };
    }

    /// <summary>
    /// Handles HTTP response errors
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="statusCode">HTTP status code</param>
    /// <param name="operation">Operation that failed</param>
    /// <param name="responseContent">Optional response content</param>
    /// <returns>Formatted error message</returns>
    public static string HandleHttpError(ILogger logger, System.Net.HttpStatusCode statusCode, string operation, string? responseContent = null)
    {
        logger.LogError("HTTP error during {Operation}. Status: {StatusCode}, Content: {Content}", 
            operation, statusCode, responseContent ?? "No content");
        
        var errorMessage = statusCode switch
        {
            System.Net.HttpStatusCode.Unauthorized => "Unauthorized: Please check your API credentials",
            System.Net.HttpStatusCode.Forbidden => "Forbidden: You don't have permission to perform this operation",
            System.Net.HttpStatusCode.NotFound => "Not found: The requested resource was not found",
            System.Net.HttpStatusCode.BadRequest => "Bad request: The request parameters are invalid",
            System.Net.HttpStatusCode.InternalServerError => "Internal server error: The API encountered an error",
            System.Net.HttpStatusCode.ServiceUnavailable => "Service unavailable: The API is temporarily unavailable",
            System.Net.HttpStatusCode.TooManyRequests => "Rate limit exceeded: Too many requests, please try again later",
            _ => $"HTTP error {(int)statusCode}: {statusCode}"
        };

        var details = !string.IsNullOrEmpty(responseContent) ? $" Details: {responseContent}" : "";
        return $"Error: {operation} failed. {errorMessage}.{details}";
    }

    /// <summary>
    /// Validates required parameters
    /// </summary>
    /// <param name="parameterName">Name of the parameter</param>
    /// <param name="parameterValue">Value of the parameter</param>
    /// <exception cref="ArgumentException">Thrown when parameter is null or empty</exception>
    public static void ValidateRequired(string parameterName, string? parameterValue)
    {
        if (string.IsNullOrWhiteSpace(parameterValue))
        {
            throw new ArgumentException($"Parameter '{parameterName}' is required and cannot be null or empty", parameterName);
        }
    }

    /// <summary>
    /// Validates JSON input
    /// </summary>
    /// <param name="parameterName">Name of the parameter</param>
    /// <param name="jsonValue">JSON string to validate</param>
    /// <exception cref="ArgumentException">Thrown when JSON is invalid</exception>
    public static void ValidateJson(string parameterName, string jsonValue)
    {
        ValidateRequired(parameterName, jsonValue);
        
        try
        {
            System.Text.Json.JsonDocument.Parse(jsonValue);
        }
        catch (System.Text.Json.JsonException ex)
        {
            throw new ArgumentException($"Parameter '{parameterName}' contains invalid JSON: {ex.Message}", parameterName, ex);
        }
    }
}