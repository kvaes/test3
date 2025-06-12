# Test3 - Semantic Kernel Agent Project

This repository contains a C# Semantic Kernel-powered agent that integrates with multiple BICS APIs to provide intelligent telecommunications service management capabilities.

## 🚀 Features

- **Semantic Kernel Integration**: Powered by Microsoft's Semantic Kernel framework
- **Multi-API Support**: Integrates with 8 different BICS APIs
- **Containerized Deployment**: Docker-based deployment with CI/CD pipeline
- **Secure Configuration**: Parameterized API endpoints and secure secret management
- **Comprehensive Documentation**: Full development and deployment guides
- **GitHub Codespaces Ready**: Pre-configured development environment

## 📋 Supported APIs

The agent includes plugins for the following BICS APIs:

1. **Connect API** - Connection management and network services
2. **MyNumbers API** - Number management and configuration
3. **MyNumbers Address Management API** - Address and location services
4. **MyNumbers CDR API** - Call Detail Records management
5. **MyNumbers Disconnection API** - Service disconnection handling
6. **MyNumbers Emergency Services API** - Emergency service configuration
7. **MyNumbers Number Porting API** - Number portability services
8. **SMS API** - SMS messaging services

## 🏗️ Repository Structure

```
├── .github/
│   ├── workflows/          # CI/CD workflows
│   ├── dependabot.yml      # Dependency management
│   └── SECURITY.md         # Security policy
├── agent/                  # C# Semantic Kernel agent
│   ├── Functions/          # Kernel function definitions
│   ├── Models/             # Data models for APIs
│   ├── Plugins/            # API plugin implementations
│   ├── Processes/          # Defined processes
│   ├── Steps/              # Reusable steps
│   ├── Program.cs          # Application entry point
│   ├── Agent.csproj        # Project file
│   ├── Dockerfile          # Container definition
│   └── appsettings.json    # Configuration
├── docs/                   # Project documentation
├── LICENSE                 # MIT License
└── README.md              # This file
```

## 🛠️ Development Setup

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started)
- [Git](https://git-scm.com/)

### Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/kvaes/test3.git
   cd test3
   ```

2. **Restore dependencies**
   ```bash
   cd agent
   dotnet restore
   ```

3. **Build the project**
   ```bash
   dotnet build
   ```

4. **Run the agent**
   ```bash
   dotnet run
   ```

### Docker Development

1. **Build the Docker image**
   ```bash
   docker build -t test3-agent -f agent/Dockerfile .
   ```

2. **Run the container**
   ```bash
   docker run --rm test3-agent
   ```

## ☁️ GitHub Codespaces

This repository is pre-configured for GitHub Codespaces development:

1. **Open in Codespaces**
   - Click the "Code" button in GitHub
   - Select "Open with Codespaces"
   - Choose "New codespace"

2. **Development Environment**
   - Pre-installed .NET 8.0 SDK
   - Docker support
   - VS Code extensions for C# development
   - All dependencies ready to use

3. **End-to-End Testing**
   - Both front-end and back-end can run simultaneously
   - Integrated debugging and testing capabilities
   - Full API integration testing environment

## 🔧 Configuration

### API Settings

Configure API endpoints in `agent/appsettings.json`:

```json
{
  "ApiSettings": {
    "ConnectApi": {
      "BaseUrl": "https://api.bics.com/connect"
    },
    "MyNumbersApi": {
      "BaseUrl": "https://api.bics.com/mynumbers"
    }
    // ... other APIs
  }
}
```

### Environment Variables

Set environment-specific configurations:

- `ASPNETCORE_ENVIRONMENT`: Development, Staging, Production
- `API_KEY`: Your BICS API key (if required)
- `LOG_LEVEL`: Logging level (Information, Warning, Error)

## 🧪 Testing

### Unit Tests

```bash
cd agent
dotnet test
```

### Integration Tests

```bash
# Run with specific configuration
dotnet test --configuration Release
```

## 🚀 Deployment

### CI/CD Pipeline

The project includes automated CI/CD via GitHub Actions:

- **Build**: Compiles and tests the application
- **Container**: Builds and pushes Docker images
- **Security**: Automated dependency scanning
- **Quality**: Code analysis and linting

### Manual Deployment

1. **Build and push container**
   ```bash
   docker build -t ghcr.io/kvaes/test3/agent:latest -f agent/Dockerfile .
   docker push ghcr.io/kvaes/test3/agent:latest
   ```

2. **Deploy to your environment**
   ```bash
   docker run -d --name test3-agent ghcr.io/kvaes/test3/agent:latest
   ```

## 📚 Documentation

- [API Documentation](./docs/api.md) - Detailed API integration guides
- [Development Guide](./docs/development.md) - Comprehensive development setup
- [Deployment Guide](./docs/deployment.md) - Production deployment instructions
- [Contributing Guide](./docs/contributing.md) - How to contribute to the project

## 🔒 Security

- Review our [Security Policy](./.github/SECURITY.md)
- Report vulnerabilities responsibly
- Keep dependencies updated via Dependabot
- Follow secure coding practices

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for your changes
5. Ensure all tests pass
6. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙋‍♂️ Support

- Create an issue for bug reports or feature requests
- Check existing documentation in the `docs/` folder
- Review the FAQ section in the documentation

## 🏷️ Version

Current version: 1.0.0

For version history and changelog, see [releases](https://github.com/kvaes/test3/releases).