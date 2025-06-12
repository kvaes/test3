# Contributing Guide

Thank you for your interest in contributing to the Test3 Semantic Kernel Agent project! This guide outlines the development process, coding standards, and contribution workflow.

## 🚀 Getting Started

### Prerequisites

Ensure you have the following installed:
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Git](https://git-scm.com/downloads)
- [Visual Studio Code](https://code.visualstudio.com/) or [Visual Studio 2022](https://visualstudio.microsoft.com/)

### Setting Up Development Environment

1. **Fork and Clone the Repository**
   ```bash
   git clone https://github.com/your-username/test3.git
   cd test3
   ```

2. **Set Up Development Environment**
   ```bash
   # Restore dependencies
   cd agent
   dotnet restore
   
   # Build the project
   dotnet build
   
   # Run tests
   dotnet test
   ```

3. **Use GitHub Codespaces (Optional)**
   - Click "Code" → "Open with Codespaces" in GitHub
   - Pre-configured environment with all dependencies

## 📝 Development Workflow

### Branch Strategy

- `main` - Production-ready code
- `develop` - Integration branch for features
- `feature/*` - Feature development branches
- `bugfix/*` - Bug fix branches
- `hotfix/*` - Critical production fixes

### Creating a Feature Branch

```bash
# Create and switch to feature branch
git checkout -b feature/your-feature-name

# Make your changes
# ... development work ...

# Commit your changes
git add .
git commit -m "feat: add new feature description"

# Push to your fork
git push origin feature/your-feature-name

# Create pull request on GitHub
```

## 🏗️ Project Structure

Understanding the codebase organization:

```
test3/
├── .github/                    # GitHub workflows and templates
│   ├── workflows/             # CI/CD pipelines
│   ├── dependabot.yml         # Dependency management
│   └── SECURITY.md            # Security policy
├── .devcontainer/             # Codespaces configuration
├── agent/                     # Main C# Semantic Kernel application
│   ├── Common/                # Shared utilities and helpers
│   ├── Functions/             # Kernel function definitions
│   ├── Models/                # Data models for APIs
│   │   ├── Connect/           # Connect API models
│   │   ├── MyNumbers/         # MyNumbers API models
│   │   └── Sms/               # SMS API models
│   ├── Plugins/               # Semantic Kernel plugins
│   ├── Processes/             # Business process definitions
│   ├── Steps/                 # Reusable step implementations
│   ├── Agent.csproj           # Project file
│   ├── Dockerfile             # Container definition
│   ├── Program.cs             # Application entry point
│   └── appsettings.json       # Configuration
├── docs/                      # Project documentation
└── README.md                  # Main project documentation
```

## 🔧 Coding Standards

### C# Guidelines

Follow [.NET coding conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions):

```csharp
// Use PascalCase for public members
public class MyClass
{
    // Use camelCase for private fields with underscore prefix
    private readonly ILogger _logger;
    
    // Use PascalCase for properties
    public string PropertyName { get; set; }
    
    // Use descriptive method names with PascalCase
    public async Task<string> GetUserDataAsync(string userId)
    {
        // Use var when type is obvious
        var result = await SomeAsyncOperation();
        
        // Use explicit types when not obvious
        HttpResponseMessage response = await _httpClient.GetAsync(url);
        
        return result;
    }
}
```

### Plugin Development Guidelines

When creating new plugins:

```csharp
/// <summary>
/// Plugin for [API Name] - Brief description
/// </summary>
[Description("Detailed description of what this plugin does")]
public class YourApiPlugin
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<YourApiPlugin> _logger;
    private readonly string _baseUrl;

    public YourApiPlugin(HttpClient httpClient, ILogger<YourApiPlugin> logger, IOptions<ApiSettings> apiSettings)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _baseUrl = apiSettings.Value.YourApi.BaseUrl;
        
        if (string.IsNullOrEmpty(_baseUrl))
        {
            throw new ArgumentException("Your API base URL is not configured", nameof(apiSettings));
        }
    }

    /// <summary>
    /// Detailed description of what this function does
    /// </summary>
    /// <param name="parameter">Description of parameter</param>
    /// <returns>Description of return value</returns>
    [KernelFunction, Description("Brief description for Semantic Kernel")]
    public async Task<string> YourFunctionAsync(
        [Description("Parameter description for Semantic Kernel")] string parameter)
    {
        try
        {
            ErrorHandler.ValidateRequired(nameof(parameter), parameter);
            
            _logger.LogInformation("Performing operation with {Parameter}", parameter);

            var response = await _httpClient.GetAsync($"{_baseUrl}/endpoint/{parameter}");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "YourFunction", errorContent);
            }

            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
        catch (Exception ex)
        {
            return ErrorHandler.HandleException(_logger, ex, "YourFunction", parameter);
        }
    }
}
```

### Error Handling

Always use the centralized error handling:

```csharp
// Validate required parameters
ErrorHandler.ValidateRequired(nameof(parameter), parameter);

// Validate JSON input
ErrorHandler.ValidateJson(nameof(jsonParameter), jsonParameter);

// Handle exceptions
catch (Exception ex)
{
    return ErrorHandler.HandleException(_logger, ex, "OperationName", context);
}

// Handle HTTP errors
if (!response.IsSuccessStatusCode)
{
    var errorContent = await response.Content.ReadAsStringAsync();
    return ErrorHandler.HandleHttpError(_logger, response.StatusCode, "OperationName", errorContent);
}
```

## 🧪 Testing Guidelines

### Unit Tests

Create comprehensive unit tests for new functionality:

```csharp
[Test]
public async Task YourFunction_ValidInput_ReturnsExpectedResult()
{
    // Arrange
    var mockHttpClient = CreateMockHttpClient();
    var mockLogger = Mock.Of<ILogger<YourPlugin>>();
    var mockOptions = CreateMockOptions();
    
    var plugin = new YourPlugin(mockHttpClient, mockLogger, mockOptions);
    
    // Act
    var result = await plugin.YourFunctionAsync("validInput");
    
    // Assert
    Assert.IsNotNull(result);
    Assert.Contains("expected", result);
}

[Test]
public async Task YourFunction_InvalidInput_ReturnsErrorMessage()
{
    // Arrange
    var plugin = CreatePluginWithMocks();
    
    // Act
    var result = await plugin.YourFunctionAsync("");
    
    // Assert
    Assert.That(result, Does.StartWith("Error:"));
}
```

### Integration Tests

Test real API interactions (against sandbox environments):

```csharp
[Test]
[Category("Integration")]
public async Task YourPlugin_RealApiCall_Integration()
{
    // Only run against test/sandbox endpoints
    var plugin = CreateRealPlugin();
    
    var result = await plugin.YourFunctionAsync("testParameter");
    
    Assert.IsNotNull(result);
}
```

## 📚 Documentation Requirements

### Code Documentation

- Add XML documentation comments for all public methods
- Include parameter descriptions for Semantic Kernel functions
- Document complex business logic

### API Documentation

When adding new plugins:

1. Update `docs/api.md` with new API details
2. Include usage examples
3. Document error scenarios
4. Add configuration requirements

### README Updates

Update README.md if adding:
- New dependencies
- New configuration requirements
- New deployment options

## 🔄 Pull Request Process

### Before Submitting

1. **Ensure Code Quality**
   ```bash
   # Build without warnings
   dotnet build --configuration Release
   
   # Run all tests
   dotnet test
   
   # Check formatting
   dotnet format --verify-no-changes
   ```

2. **Update Documentation**
   - Update relevant documentation files
   - Add/update API documentation
   - Include usage examples

3. **Test Your Changes**
   - Test locally with `dotnet run`
   - Test container build with `docker build`
   - Verify CI pipeline passes

### Pull Request Template

```markdown
## Description
Brief description of changes made.

## Type of Change
- [ ] Bug fix (non-breaking change which fixes an issue)
- [ ] New feature (non-breaking change which adds functionality)
- [ ] Breaking change (fix or feature that would cause existing functionality to not work as expected)
- [ ] Documentation update

## Testing
- [ ] Unit tests added/updated
- [ ] Integration tests added/updated
- [ ] Manual testing completed

## Documentation
- [ ] Code documentation updated
- [ ] API documentation updated
- [ ] README updated (if applicable)

## Checklist
- [ ] Code follows project coding standards
- [ ] Self-review of code completed
- [ ] Tests pass locally
- [ ] Documentation updated
```

### Review Process

1. **Automated Checks**
   - CI pipeline must pass
   - All tests must pass
   - No merge conflicts

2. **Code Review**
   - At least one approving review required
   - Address all review comments
   - Maintain code quality standards

3. **Merge Requirements**
   - Squash commits for clean history
   - Update CHANGELOG.md if applicable
   - Delete feature branch after merge

## 🐛 Bug Reports

When reporting bugs, include:

1. **Environment Information**
   - .NET version
   - Operating system
   - Container runtime (if applicable)

2. **Reproduction Steps**
   - Minimal code to reproduce
   - Expected vs actual behavior
   - Error messages and stack traces

3. **Additional Context**
   - Configuration files (sanitized)
   - Log outputs
   - Screenshots (if applicable)

## 💡 Feature Requests

When requesting features:

1. **Use Case Description**
   - What problem does this solve?
   - Who would benefit from this feature?

2. **Proposed Solution**
   - High-level implementation approach
   - API design considerations
   - Potential challenges

3. **Alternatives Considered**
   - Other ways to solve the problem
   - Why this approach is preferred

## 🏷️ Release Process

### Version Numbering

We follow [Semantic Versioning](https://semver.org/):
- `MAJOR.MINOR.PATCH`
- MAJOR: Breaking changes
- MINOR: New features (backward compatible)
- PATCH: Bug fixes (backward compatible)

### Release Checklist

1. Update version numbers
2. Update CHANGELOG.md
3. Create release tag
4. Build and publish container images
5. Update documentation
6. Create GitHub release

## 🤝 Community Guidelines

- Be respectful and inclusive
- Help newcomers get started
- Share knowledge and best practices
- Follow the [Code of Conduct](CODE_OF_CONDUCT.md)

## 📞 Getting Help

- Create GitHub issues for bugs and feature requests
- Use GitHub Discussions for questions and ideas
- Check existing documentation first
- Provide detailed information when asking for help

Thank you for contributing to the Test3 project! Your contributions help make this project better for everyone.