# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY SlackChannelReader.csproj .
RUN dotnet restore

# Copy source code and build
COPY . .
RUN dotnet build --configuration Release --no-restore

# Publish stage
FROM build AS publish
RUN dotnet publish --configuration Release --no-build --output /app/publish \
    --runtime linux-x64 --self-contained false

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS runtime
WORKDIR /app

# Create non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copy published application
COPY --from=publish /app/publish .

# Create output directory and set permissions
RUN mkdir -p /app/slack-archive && chown -R appuser:appuser /app
USER appuser

# Set environment variables
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV ASPNETCORE_URLS=

# Default command
ENTRYPOINT ["dotnet", "SlackChannelReader.dll"]