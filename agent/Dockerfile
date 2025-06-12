# Use the official .NET 8 runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["agent/Agent.csproj", "agent/"]
RUN dotnet restore "agent/Agent.csproj"

# Copy the source code and build the application
COPY agent/ agent/
WORKDIR "/src/agent"
RUN dotnet build "Agent.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Agent.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create a non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser
RUN chown -R appuser:appuser /app
USER appuser

ENTRYPOINT ["dotnet", "Agent.dll"]