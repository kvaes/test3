# GitHub Codespaces Guide

This guide explains how to set up and use GitHub Codespaces for end-to-end development and testing of the Test3 Semantic Kernel Agent project.

## 🚀 Getting Started with Codespaces

### Creating a Codespace

1. **Navigate to the Repository**
   - Go to https://github.com/kvaes/test3

2. **Create New Codespace**
   - Click the green "Code" button
   - Select the "Codespaces" tab
   - Click "Create codespace on main"

3. **Wait for Setup**
   - Codespace will automatically provision with all dependencies
   - Initial setup takes 2-3 minutes

### Pre-configured Environment

Your Codespace comes with:

- ✅ .NET 8.0 SDK
- ✅ Docker and Docker Compose
- ✅ Visual Studio Code with C# extensions
- ✅ Git configuration
- ✅ All project dependencies pre-installed

## 🏗️ Development Environment Setup

### Automatic Configuration

The Codespace automatically configures:

```json
{
  "name": "Test3 Development",
  "image": "mcr.microsoft.com/devcontainers/dotnet:8.0",
  "features": {
    "ghcr.io/devcontainers/features/docker-in-docker:2": {},
    "ghcr.io/devcontainers/features/github-cli:1": {}
  },
  "customizations": {
    "vscode": {
      "extensions": [
        "ms-dotnettools.csharp",
        "ms-dotnettools.vscode-dotnet-runtime",
        "ms-vscode.vscode-json"
      ]
    }
  },
  "postCreateCommand": "cd agent && dotnet restore"
}
```

### Manual Setup (if needed)

If you need to manually set up the environment:

```bash
# Restore .NET dependencies
cd agent
dotnet restore

# Verify installation
dotnet --version
docker --version
```

## 🔧 Running the Agent

### Development Mode

1. **Open Terminal** (Ctrl+Shift+`)

2. **Navigate to Agent Directory**
   ```bash
   cd agent
   ```

3. **Run the Agent**
   ```bash
   dotnet run
   ```

4. **Monitor Logs**
   The agent will start and display logs in the terminal.

### Docker Mode

1. **Build Container**
   ```bash
   docker build -t test3-agent -f agent/Dockerfile .
   ```

2. **Run Container**
   ```bash
   docker run --rm test3-agent
   ```

## 🌐 End-to-End Testing Setup

### Full Environment Configuration

1. **Configure Environment Variables**
   ```bash
   # Create .env file for Codespaces
   cat > .env << EOF
   ASPNETCORE_ENVIRONMENT=Development
   API_KEY=your-test-api-key
   LOG_LEVEL=Debug
   EOF
   ```

2. **Update Configuration**
   ```bash
   # Edit development settings
   code agent/appsettings.Development.json
   ```

### Multi-Service Testing

#### Option 1: Docker Compose

1. **Create Docker Compose File**
   ```yaml
   # docker-compose.codespaces.yml
   version: '3.8'
   services:
     agent:
       build:
         context: .
         dockerfile: agent/Dockerfile
       environment:
         - ASPNETCORE_ENVIRONMENT=Development
       ports:
         - "5000:5000"
       volumes:
         - ./agent/appsettings.Development.json:/app/appsettings.Development.json
   
     # Add frontend service when available
     frontend:
       image: nginx:alpine
       ports:
         - "3000:80"
       # Configure when frontend is added
   ```

2. **Run All Services**
   ```bash
   docker-compose -f docker-compose.codespaces.yml up -d
   ```

#### Option 2: Parallel Terminal Sessions

1. **Split Terminal** (Ctrl+Shift+5)

2. **Terminal 1: Run Agent**
   ```bash
   cd agent
   dotnet run
   ```

3. **Terminal 2: Frontend** (when available)
   ```bash
   cd frontend
   npm start
   ```

4. **Terminal 3: Testing**
   ```bash
   # Run integration tests
   cd agent
   dotnet test --filter Category=Integration
   ```

### Port Forwarding

Codespaces automatically forwards ports, but you can manually configure:

1. **View Ports Panel** (Ctrl+Shift+P → "Ports: Focus on Ports View")

2. **Add Port Forward**
   - Agent: Port 5000
   - Frontend: Port 3000 (when available)

3. **Make Public** (if needed for external testing)
   - Right-click port → "Port Visibility" → "Public"

## 🧪 Testing Scenarios

### API Integration Testing

1. **Test Individual Plugins**
   ```bash
   # Test Connect API plugin
   cd agent
   dotnet run -- --test-plugin Connect
   ```

2. **Test API Endpoints**
   ```bash
   # Use curl to test forwarded ports
   curl http://localhost:5000/health
   ```

3. **Run Comprehensive Tests**
   ```bash
   # Run all tests with detailed output
   dotnet test --logger "console;verbosity=detailed"
   ```

### End-to-End Workflows

1. **Semantic Kernel Testing**
   ```bash
   # Interactive testing mode
   dotnet run -- --interactive
   ```

2. **Plugin Chain Testing**
   ```bash
   # Test multiple plugins in sequence
   dotnet run -- --workflow "connect,mynumbers,sms"
   ```

3. **Load Testing** (when applicable)
   ```bash
   # Use built-in load testing
   dotnet run -- --load-test --concurrent-users 10
   ```

## 🔧 Development Workflows

### Live Development

1. **Enable Hot Reload**
   ```bash
   cd agent
   dotnet watch run
   ```

2. **File Watching**
   - Changes automatically trigger rebuilds
   - Console shows reload status

### Debugging

1. **Attach Debugger**
   - Set breakpoints in VS Code
   - Press F5 to start debugging
   - Use Debug Console for evaluation

2. **Remote Debugging**
   ```bash
   # For Docker containers
   docker run -p 5000:5000 -p 5001:5001 test3-agent
   ```

### Git Integration

1. **Configure Git** (if needed)
   ```bash
   git config --global user.name "Your Name"
   git config --global user.email "your.email@example.com"
   ```

2. **Create Feature Branch**
   ```bash
   git checkout -b feature/your-feature
   ```

3. **Commit and Push**
   ```bash
   git add .
   git commit -m "feat: add new feature"
   git push origin feature/your-feature
   ```

## 📊 Monitoring and Observability

### Built-in Monitoring

1. **Application Insights** (when configured)
   ```bash
   # View telemetry in Codespaces
   dotnet run -- --telemetry
   ```

2. **Health Checks**
   ```bash
   # Monitor application health
   curl http://localhost:5000/health
   ```

### Log Analysis

1. **Real-time Logs**
   ```bash
   # Follow logs in real-time
   dotnet run | tee logs.txt
   ```

2. **Structured Logging**
   ```bash
   # Export logs for analysis
   dotnet run --environment Development > detailed-logs.json
   ```

## 🚨 Troubleshooting

### Common Codespaces Issues

1. **Port Not Accessible**
   - Check port forwarding configuration
   - Ensure application is binding to 0.0.0.0, not localhost

2. **Slow Performance**
   - Choose higher-spec machine type
   - Close unnecessary browser tabs
   - Use Docker efficiently

3. **Build Failures**
   ```bash
   # Clear and restore
   cd agent
   dotnet clean
   dotnet restore --force
   ```

### Environment Issues

1. **Missing Dependencies**
   ```bash
   # Reinstall packages
   cd agent
   dotnet restore --force
   ```

2. **Docker Issues**
   ```bash
   # Restart Docker service
   sudo service docker restart
   
   # Clean Docker cache
   docker system prune -f
   ```

3. **Git Issues**
   ```bash
   # Reset git configuration
   git config --global --unset-all user.name
   git config --global --unset-all user.email
   ```

## 💡 Best Practices

### Resource Management

1. **Stop Codespace When Not in Use**
   - Codespaces auto-suspend after 30 minutes
   - Manually stop to save compute time

2. **Efficient Development**
   - Use incremental builds
   - Leverage hot reload features
   - Close unused terminal sessions

### Security

1. **Environment Variables**
   - Use Codespace secrets for API keys
   - Never commit sensitive data

2. **Port Access**
   - Keep ports private unless needed
   - Use authentication for public endpoints

### Collaboration

1. **Share Codespace** (when needed)
   - Use Live Share extension
   - Share specific ports for testing

2. **Code Reviews**
   - Create PRs directly from Codespace
   - Use GitHub's review features

## 📚 Additional Resources

- [GitHub Codespaces Documentation](https://docs.github.com/en/codespaces)
- [VS Code in Codespaces](https://code.visualstudio.com/docs/remote/codespaces)
- [Docker in Codespaces](https://docs.github.com/en/codespaces/developing-in-codespaces/using-docker-in-a-codespace)

This guide provides everything you need to develop and test the Test3 project effectively in GitHub Codespaces. The pre-configured environment makes it easy to get started quickly with full end-to-end testing capabilities.