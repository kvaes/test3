# Development Guide

This guide provides comprehensive instructions for setting up and developing with the Test3 Semantic Kernel Agent project.

## 🛠️ Local Development Setup

### Prerequisites

Ensure you have the following installed on your development machine:

- **.NET 8.0 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Docker Desktop** - [Download here](https://www.docker.com/products/docker-desktop)
- **Git** - [Download here](https://git-scm.com/downloads)
- **Visual Studio Code** or **Visual Studio 2022** (recommended IDEs)

### Initial Setup

1. **Clone the Repository**
   ```bash
   git clone https://github.com/kvaes/test3.git
   cd test3
   ```

2. **Install .NET Dependencies**
   ```bash
   cd agent
   dotnet restore
   ```

3. **Verify Installation**
   ```bash
   dotnet --version  # Should show 8.0.x
   docker --version  # Should show Docker version
   ```

### IDE Configuration

#### Visual Studio Code

1. **Install Required Extensions**
   - C# Dev Kit
   - Docker
   - GitLens
   - .NET Install Tool

2. **Open Workspace**
   ```bash
   code .
   ```

3. **Configure Launch Settings**
   The project includes pre-configured launch settings in `.vscode/launch.json`.

#### Visual Studio 2022

1. **Open Solution**
   - Open `agent/Agent.csproj` directly
   - Or use "Open Folder" to open the entire repository

2. **Set Startup Project**
   - Right-click on the Agent project
   - Select "Set as Startup Project"

### Environment Configuration

1. **Copy Configuration Template**
   ```bash
   cp agent/appsettings.json agent/appsettings.Development.json
   ```

2. **Update Development Settings**
   ```json
   {
     "Logging": {
       "LogLevel": {
         "Default": "Debug",
         "Microsoft": "Information"
       }
     },
     "ApiSettings": {
       "ConnectApi": {
         "BaseUrl": "https://sandbox-api.bics.com/connect"
       }
       // Update other APIs for sandbox/development endpoints
     }
   }
   ```

## 🔧 Building and Running

### Command Line

1. **Build the Project**
   ```bash
   cd agent
   dotnet build
   ```

2. **Run the Application**
   ```bash
   dotnet run
   ```

3. **Run with Specific Configuration**
   ```bash
   dotnet run --environment Development
   ```

### Docker Development

1. **Build Docker Image**
   ```bash
   docker build -t test3-agent:dev -f agent/Dockerfile .
   ```

2. **Run with Docker**
   ```bash
   docker run --rm \
     -e ASPNETCORE_ENVIRONMENT=Development \
     -v $(pwd)/agent/appsettings.Development.json:/app/appsettings.Development.json \
     test3-agent:dev
   ```

3. **Docker Compose (if needed)**
   ```yaml
   # docker-compose.dev.yml
   version: '3.8'
   services:
     agent:
       build:
         context: .
         dockerfile: agent/Dockerfile
       environment:
         - ASPNETCORE_ENVIRONMENT=Development
       volumes:
         - ./agent/appsettings.Development.json:/app/appsettings.Development.json
   ```

## 🧪 Testing

### Unit Testing

1. **Run All Tests**
   ```bash
   cd agent
   dotnet test
   ```

2. **Run Tests with Coverage**
   ```bash
   dotnet test --collect:"XPlat Code Coverage"
   ```

3. **Run Specific Test Category**
   ```bash
   dotnet test --filter Category=Unit
   ```

### Integration Testing

1. **Setup Test Environment**
   ```bash
   # Copy test configuration
   cp agent/appsettings.json agent/appsettings.Testing.json
   ```

2. **Run Integration Tests**
   ```bash
   dotnet test --environment Testing --filter Category=Integration
   ```

### Manual Testing

1. **Test API Plugins**
   ```bash
   # Use the agent CLI to test specific plugins
   dotnet run -- --plugin Connect --action GetStatus
   ```

2. **Test with Semantic Kernel**
   ```bash
   # Interactive testing mode
   dotnet run -- --interactive
   ```

## 🐛 Debugging

### Visual Studio Code

1. **Set Breakpoints** - Click in the gutter next to line numbers
2. **Start Debugging** - Press F5 or use the Debug panel
3. **Use Debug Console** - Evaluate expressions and view variables

### Visual Studio 2022

1. **Set Breakpoints** - Click in the margin or press F9
2. **Start Debugging** - Press F5 or click Debug > Start Debugging
3. **Use Immediate Window** - Debug > Windows > Immediate

### Docker Debugging

1. **Enable Docker Debugging**
   ```bash
   docker run -it --rm \
     -p 5000:5000 \
     -e ASPNETCORE_ENVIRONMENT=Development \
     test3-agent:dev
   ```

2. **Attach to Process** - Use VS Code or Visual Studio remote debugging

## 🔄 Development Workflow

### Git Workflow

1. **Create Feature Branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Make Changes and Commit**
   ```bash
   git add .
   git commit -m "feat: add new feature"
   ```

3. **Push and Create PR**
   ```bash
   git push origin feature/your-feature-name
   ```

### Code Quality

1. **Run Code Analysis**
   ```bash
   dotnet build --verbosity normal
   ```

2. **Format Code**
   ```bash
   dotnet format
   ```

3. **Security Scan**
   ```bash
   # This runs automatically in CI, but you can run locally
   dotnet list package --vulnerable
   ```

### Adding New Plugins

1. **Create Plugin Class**
   ```csharp
   // agent/Plugins/MyNewPlugin.cs
   [Description("Description of what this plugin does")]
   public class MyNewPlugin
   {
       [KernelFunction, Description("Function description")]
       public async Task<string> MyFunction([Description("Parameter description")] string input)
       {
           // Implementation
       }
   }
   ```

2. **Register Plugin**
   ```csharp
   // In Program.cs
   kernel.Plugins.AddFromType<MyNewPlugin>();
   ```

3. **Add Configuration**
   ```json
   // In appsettings.json
   "ApiSettings": {
     "MyNewApi": {
       "BaseUrl": "https://api.example.com"
     }
   }
   ```

### Adding New Models

1. **Create Model Class**
   ```csharp
   // agent/Models/MyApiModels.cs
   public class MyApiResponse
   {
       public string Id { get; set; } = string.Empty;
       public string Status { get; set; } = string.Empty;
       // Add properties based on API response
   }
   ```

2. **Use in Plugin**
   ```csharp
   public async Task<MyApiResponse> GetStatus()
   {
       // Implementation using the model
   }
   ```

## 📊 Performance Monitoring

### Local Monitoring

1. **Enable Detailed Logging**
   ```json
   {
     "Logging": {
       "LogLevel": {
         "Microsoft.SemanticKernel": "Debug",
         "Agent": "Debug"
       }
     }
   }
   ```

2. **Monitor Performance**
   ```bash
   # Use built-in metrics
   dotnet run -- --metrics
   ```

### Memory Profiling

1. **Use dotMemory Unit** (JetBrains)
2. **Use PerfView** (Microsoft)
3. **Monitor with Application Insights** (Production)

## 🚨 Troubleshooting

### Common Issues

1. **Build Failures**
   ```bash
   # Clear NuGet cache
   dotnet nuget locals all --clear
   dotnet restore --force
   ```

2. **Docker Issues**
   ```bash
   # Clean Docker cache
   docker system prune -a
   
   # Rebuild without cache
   docker build --no-cache -t test3-agent:dev -f agent/Dockerfile .
   ```

3. **Plugin Loading Issues**
   - Check plugin registration in Program.cs
   - Verify API configuration in appsettings.json
   - Review logs for specific error messages

### Getting Help

1. **Check Logs** - Always start with application logs
2. **Review Documentation** - Check other docs in the `docs/` folder
3. **Create Issue** - Use GitHub issues for bugs or questions
4. **Discussion** - Use GitHub Discussions for general questions

## 📚 Additional Resources

- [Semantic Kernel Documentation](https://learn.microsoft.com/en-us/semantic-kernel/)
- [.NET 8 Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Docker Documentation](https://docs.docker.com/)
- [BICS API Documentation](https://developer.bics.com/portal/apis)

This development guide should help you get started quickly and efficiently with the Test3 project. For specific API documentation, refer to the [API Documentation](./api.md).