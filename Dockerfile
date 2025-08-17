# Multi-stage build for production
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["PresentationApp/PresentationApp.csproj", "PresentationApp/"]
COPY ["ApplicationLayer/ApplicationLayer.csproj", "ApplicationLayer/"]
COPY ["DomainLayer/DomainLayer.csproj", "DomainLayer/"]
COPY ["InfrastructureLayer/InfrastructureLayer.csproj", "InfrastructureLayer/"]

# Restore dependencies
RUN dotnet restore "PresentationApp/PresentationApp.csproj"

# Copy source code
COPY . .

# Build the application
WORKDIR "/src/PresentationApp"
RUN dotnet build "PresentationApp.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "PresentationApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create non-root user
RUN addgroup --system --gid 1001 appgroup \
    && adduser --system --uid 1001 --gid 1001 --shell /bin/false appuser

# Copy published app
COPY --from=publish /app/publish .

# Create uploads directory and set permissions
RUN mkdir -p /app/wwwroot/uploads \
    && chown -R appuser:appgroup /app \
    && chmod -R 755 /app

# Switch to non-root user
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_USE_POLLING_FILE_WATCHER=true

# Entry point
ENTRYPOINT ["dotnet", "PresentationApp.dll"]