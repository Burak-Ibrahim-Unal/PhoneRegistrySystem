# Migration için özel container
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Project dosyalarını kopyala
COPY ["PhoneRegistry.ContactApi/PhoneRegistry.ContactApi.csproj", "PhoneRegistry.ContactApi/"]
COPY ["PhoneRegistry.Application/PhoneRegistry.Application.csproj", "PhoneRegistry.Application/"]
COPY ["PhoneRegistry.Infrastructure/PhoneRegistry.Infrastructure.csproj", "PhoneRegistry.Infrastructure/"]
COPY ["PhoneRegistry.Domain/PhoneRegistry.Domain.csproj", "PhoneRegistry.Domain/"]

# Restore dependencies
RUN dotnet restore "PhoneRegistry.ContactApi/PhoneRegistry.ContactApi.csproj"

# Copy all source code
COPY . .

# Install EF Core tools
RUN dotnet tool install --global dotnet-ef --version 8.0.8
ENV PATH="$PATH:/root/.dotnet/tools"

# Install PostgreSQL client
RUN apt-get update && apt-get install -y postgresql-client && rm -rf /var/lib/apt/lists/*

# Build the application
WORKDIR "/src/PhoneRegistry.ContactApi"
RUN dotnet build "PhoneRegistry.ContactApi.csproj" -c Release -o /app/build

# Copy scripts
COPY scripts/ /app/scripts/
RUN chmod +x /app/scripts/init-db.sh

WORKDIR /app/build
ENTRYPOINT ["/app/scripts/init-db.sh"]
