# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["PhoneRegistry.ReportApi/PhoneRegistry.ReportApi.csproj", "PhoneRegistry.ReportApi/"]
COPY ["PhoneRegistry.Application/PhoneRegistry.Application.csproj", "PhoneRegistry.Application/"]
COPY ["PhoneRegistry.Infrastructure/PhoneRegistry.Infrastructure.csproj", "PhoneRegistry.Infrastructure/"]
COPY ["PhoneRegistry.Domain/PhoneRegistry.Domain.csproj", "PhoneRegistry.Domain/"]

# Restore dependencies
RUN dotnet restore "PhoneRegistry.ReportApi/PhoneRegistry.ReportApi.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/PhoneRegistry.ReportApi"
RUN dotnet build "PhoneRegistry.ReportApi.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "PhoneRegistry.ReportApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create logs directory
RUN mkdir -p /app/logs

EXPOSE 80
ENTRYPOINT ["dotnet", "PhoneRegistry.ReportApi.dll"]
