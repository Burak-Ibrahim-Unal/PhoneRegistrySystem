FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["PhoneRegistry.WorkerService/PhoneRegistry.WorkerService.csproj", "PhoneRegistry.WorkerService/"]
COPY ["PhoneRegistry.Domain/PhoneRegistry.Domain.csproj", "PhoneRegistry.Domain/"]
COPY ["PhoneRegistry.Infrastructure/PhoneRegistry.Infrastructure.csproj", "PhoneRegistry.Infrastructure/"]
COPY ["PhoneRegistry.Application/PhoneRegistry.Application.csproj", "PhoneRegistry.Application/"]

# Restore dependencies
RUN dotnet restore "PhoneRegistry.WorkerService/PhoneRegistry.WorkerService.csproj"

# Copy source code
COPY . .

# Build
WORKDIR "/src/PhoneRegistry.WorkerService"
RUN dotnet build "PhoneRegistry.WorkerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PhoneRegistry.WorkerService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PhoneRegistry.WorkerService.dll"]
