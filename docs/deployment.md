# Deployment Guide

This guide provides comprehensive instructions for deploying the Test3 Semantic Kernel Agent project in various environments.

## 🏗️ Deployment Architecture

The Test3 agent can be deployed in multiple configurations:

1. **Standalone Container** - Single container deployment
2. **Kubernetes Cluster** - Scalable orchestrated deployment
3. **Azure Container Instances** - Serverless container deployment
4. **Docker Swarm** - Container orchestration
5. **Local Development** - Development and testing

## 📦 Container Deployment

### Building the Container

```bash
# Build the container image
docker build -t test3-agent:latest -f agent/Dockerfile .

# Tag for registry
docker tag test3-agent:latest ghcr.io/kvaes/test3/agent:latest

# Push to registry
docker push ghcr.io/kvaes/test3/agent:latest
```

### Running the Container

```bash
# Basic container run
docker run --rm test3-agent:latest

# With environment variables
docker run --rm \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ApiSettings__ConnectApi__BaseUrl=https://api.bics.com/connect/v1 \
  -e Logging__LogLevel__Default=Information \
  test3-agent:latest

# With volume mounting for configuration
docker run --rm \
  -v $(pwd)/config/appsettings.Production.json:/app/appsettings.Production.json \
  -e ASPNETCORE_ENVIRONMENT=Production \
  test3-agent:latest
```

### Docker Compose Deployment

```yaml
# docker-compose.yml
version: '3.8'

services:
  agent:
    image: ghcr.io/kvaes/test3/agent:latest
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - Logging__LogLevel__Default=Information
    volumes:
      - ./config/appsettings.Production.json:/app/appsettings.Production.json
    restart: unless-stopped
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:5000/health"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s

  # Optional: Add monitoring services
  prometheus:
    image: prom/prometheus:latest
    ports:
      - "9090:9090"
    volumes:
      - ./monitoring/prometheus.yml:/etc/prometheus/prometheus.yml

  grafana:
    image: grafana/grafana:latest
    ports:
      - "3000:3000"
    environment:
      - GF_SECURITY_ADMIN_PASSWORD=admin
    volumes:
      - grafana-storage:/var/lib/grafana

volumes:
  grafana-storage:
```

## ☸️ Kubernetes Deployment

### Basic Deployment

```yaml
# k8s/namespace.yaml
apiVersion: v1
kind: Namespace
metadata:
  name: test3-agent

---
# k8s/deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: test3-agent
  namespace: test3-agent
  labels:
    app: test3-agent
spec:
  replicas: 3
  selector:
    matchLabels:
      app: test3-agent
  template:
    metadata:
      labels:
        app: test3-agent
    spec:
      containers:
      - name: agent
        image: ghcr.io/kvaes/test3/agent:latest
        ports:
        - containerPort: 5000
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: ASPNETCORE_URLS
          value: "http://+:5000"
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /health
            port: 5000
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 5000
          initialDelaySeconds: 5
          periodSeconds: 5

---
# k8s/service.yaml
apiVersion: v1
kind: Service
metadata:
  name: test3-agent-service
  namespace: test3-agent
spec:
  selector:
    app: test3-agent
  ports:
  - port: 80
    targetPort: 5000
  type: LoadBalancer

---
# k8s/configmap.yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: test3-agent-config
  namespace: test3-agent
data:
  appsettings.Production.json: |
    {
      "Logging": {
        "LogLevel": {
          "Default": "Information",
          "Microsoft": "Warning"
        }
      },
      "ApiSettings": {
        "ConnectApi": {
          "BaseUrl": "https://api.bics.com/connect/v1"
        },
        "MyNumbersApi": {
          "BaseUrl": "https://api.bics.com/mynumbers/v1"
        },
        "SmsApi": {
          "BaseUrl": "https://api.bics.com/sms/v3"
        }
      }
    }

---
# k8s/secret.yaml
apiVersion: v1
kind: Secret
metadata:
  name: test3-agent-secrets
  namespace: test3-agent
type: Opaque
data:
  # Base64 encoded API keys
  connect-api-key: <base64-encoded-key>
  mynumbers-api-key: <base64-encoded-key>
  sms-api-key: <base64-encoded-key>
```

### Deploy to Kubernetes

```bash
# Apply all manifests
kubectl apply -f k8s/

# Check deployment status
kubectl get pods -n test3-agent
kubectl get services -n test3-agent

# View logs
kubectl logs -f deployment/test3-agent -n test3-agent

# Scale deployment
kubectl scale deployment test3-agent --replicas=5 -n test3-agent
```

## ☁️ Azure Container Instances

### Deploy to ACI

```bash
# Create resource group
az group create --name test3-rg --location eastus

# Deploy container instance
az container create \
  --resource-group test3-rg \
  --name test3-agent \
  --image ghcr.io/kvaes/test3/agent:latest \
  --cpu 1 \
  --memory 1 \
  --environment-variables \
    ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:5000 \
  --ports 5000 \
  --dns-name-label test3-agent-unique

# Check status
az container show --resource-group test3-rg --name test3-agent --query "{FQDN:ipAddress.fqdn,ProvisioningState:provisioningState}" --out table

# View logs
az container logs --resource-group test3-rg --name test3-agent
```

### Azure Container Apps

```bash
# Create Container Apps environment
az containerapp env create \
  --name test3-env \
  --resource-group test3-rg \
  --location eastus

# Deploy container app
az containerapp create \
  --name test3-agent \
  --resource-group test3-rg \
  --environment test3-env \
  --image ghcr.io/kvaes/test3/agent:latest \
  --target-port 5000 \
  --ingress external \
  --min-replicas 1 \
  --max-replicas 5 \
  --cpu 0.5 \
  --memory 1.0Gi \
  --env-vars \
    ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:5000
```

## 🔧 Configuration Management

### Environment Variables

```bash
# Production environment variables
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS=http://+:5000
export Logging__LogLevel__Default=Information
export ApiSettings__ConnectApi__BaseUrl=https://api.bics.com/connect/v1
export ApiSettings__MyNumbersApi__BaseUrl=https://api.bics.com/mynumbers/v1
export ApiSettings__SmsApi__BaseUrl=https://api.bics.com/sms/v3

# Security-related
export API_KEY_CONNECT=your-connect-api-key
export API_KEY_MYNUMBERS=your-mynumbers-api-key
export API_KEY_SMS=your-sms-api-key
```

### Configuration Files

```json
// config/appsettings.Production.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "ApiSettings": {
    "ConnectApi": {
      "BaseUrl": "https://api.bics.com/connect/v1"
    },
    "MyNumbersApi": {
      "BaseUrl": "https://api.bics.com/mynumbers/v1"
    },
    "MyNumbersAddressManagementApi": {
      "BaseUrl": "https://api.bics.com/mynumbers/v1/address-management"
    },
    "MyNumbersCDRApi": {
      "BaseUrl": "https://api.bics.com/mynumbers/v1/cdr"
    },
    "MyNumbersDisconnectionApi": {
      "BaseUrl": "https://api.bics.com/mynumbers/v1/disconnection"
    },
    "MyNumbersEmergencyServicesApi": {
      "BaseUrl": "https://api.bics.com/mynumbers/v1/emergency-services"
    },
    "MyNumbersNumberPortingApi": {
      "BaseUrl": "https://api.bics.com/mynumbers/v1/number-porting"
    },
    "SmsApi": {
      "BaseUrl": "https://api.bics.com/sms/v3"
    }
  }
}
```

## 📊 Monitoring and Observability

### Health Checks

```csharp
// Program.cs additions for health checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy())
    .AddUrlGroup(new Uri("https://api.bics.com/connect/v1/health"), "connect-api")
    .AddUrlGroup(new Uri("https://api.bics.com/mynumbers/v1/health"), "mynumbers-api")
    .AddUrlGroup(new Uri("https://api.bics.com/sms/v3/health"), "sms-api");

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new HealthCheckOptions()
{
    Predicate = check => check.Tags.Contains("ready")
});
```

### Logging Configuration

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Agent.Plugins": "Debug",
      "System.Net.Http": "Information"
    },
    "Console": {
      "IncludeScopes": true,
      "TimestampFormat": "yyyy-MM-dd HH:mm:ss "
    }
  }
}
```

### Metrics Collection

```bash
# Prometheus configuration
# monitoring/prometheus.yml
global:
  scrape_interval: 15s

scrape_configs:
  - job_name: 'test3-agent'
    static_configs:
      - targets: ['agent:5000']
    metrics_path: /metrics
    scrape_interval: 5s
```

## 🚀 CI/CD Integration

### GitHub Actions Deployment

```yaml
# .github/workflows/deploy.yml
name: Deploy to Production

on:
  push:
    branches: [main]
    tags: [v*]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v4
    
    - name: Login to Container Registry
      uses: docker/login-action@v3
      with:
        registry: ghcr.io
        username: ${{ github.actor }}
        password: ${{ secrets.GITHUB_TOKEN }}
    
    - name: Build and Push
      uses: docker/build-push-action@v5
      with:
        context: .
        file: ./agent/Dockerfile
        push: true
        tags: ghcr.io/kvaes/test3/agent:latest
    
    - name: Deploy to Azure
      uses: azure/container-apps-deploy-action@v1
      with:
        containerAppName: test3-agent
        resourceGroup: test3-rg
        imageToDeploy: ghcr.io/kvaes/test3/agent:latest
```

## 🔒 Security Considerations

### Secrets Management

```bash
# Kubernetes secrets
kubectl create secret generic test3-secrets \
  --from-literal=connect-api-key=your-key \
  --from-literal=mynumbers-api-key=your-key \
  --from-literal=sms-api-key=your-key \
  -n test3-agent

# Azure Key Vault integration
az keyvault create --name test3-keyvault --resource-group test3-rg
az keyvault secret set --vault-name test3-keyvault --name connect-api-key --value your-key
```

### Network Security

```yaml
# Network policies for Kubernetes
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: test3-agent-netpol
  namespace: test3-agent
spec:
  podSelector:
    matchLabels:
      app: test3-agent
  policyTypes:
  - Ingress
  - Egress
  ingress:
  - from: []
    ports:
    - protocol: TCP
      port: 5000
  egress:
  - to: []
    ports:
    - protocol: TCP
      port: 443  # HTTPS only
```

## 📈 Scaling Strategies

### Horizontal Pod Autoscaler

```yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: test3-agent-hpa
  namespace: test3-agent
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: test3-agent
  minReplicas: 2
  maxReplicas: 10
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
  - type: Resource
    resource:
      name: memory
      target:
        type: Utilization
        averageUtilization: 80
```

## 🛠️ Troubleshooting

### Common Deployment Issues

1. **Container Won't Start**
   ```bash
   # Check container logs
   docker logs container-id
   kubectl logs deployment/test3-agent -n test3-agent
   ```

2. **Configuration Issues**
   ```bash
   # Validate configuration
   kubectl describe configmap test3-agent-config -n test3-agent
   kubectl get secret test3-agent-secrets -n test3-agent -o yaml
   ```

3. **Network Connectivity**
   ```bash
   # Test API connectivity from pod
   kubectl exec -it deployment/test3-agent -n test3-agent -- curl https://api.bics.com/connect/v1/health
   ```

4. **Resource Constraints**
   ```bash
   # Check resource usage
   kubectl top pods -n test3-agent
   kubectl describe pod <pod-name> -n test3-agent
   ```

### Performance Tuning

```json
{
  "Kestrel": {
    "Limits": {
      "MaxConcurrentConnections": 100,
      "MaxConcurrentUpgradedConnections": 100,
      "MaxRequestBodySize": 10240000,
      "KeepAliveTimeout": "00:02:00",
      "RequestHeadersTimeout": "00:00:30"
    }
  }
}
```

## 📚 Additional Resources

- [Docker Documentation](https://docs.docker.com/)
- [Kubernetes Documentation](https://kubernetes.io/docs/)
- [Azure Container Apps](https://docs.microsoft.com/en-us/azure/container-apps/)
- [.NET Hosting and Deployment](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/)

This deployment guide provides comprehensive instructions for deploying the Test3 agent in production environments with proper security, monitoring, and scaling configurations.