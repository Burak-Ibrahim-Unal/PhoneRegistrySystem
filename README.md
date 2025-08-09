# Phone Registry System - Backend

Bu proje, telefon rehberi yönetimi için CQRS mimarisi ile geliştirilmiş bir .NET 8 Web API sistemidir.

## Mimari

- **Domain Layer**: Entity'ler, Value Objects, Repository Interface'leri
- **Application Layer**: CQRS Commands/Queries, Handlers, DTOs, Validators
- **Infrastructure Layer**: EF Core, PostgreSQL, Redis, RabbitMQ implementasyonları
- **Services Layer**: Business logic servisleri
- **Web API Layer**: RESTful endpoints

## Teknolojiler

- .NET 8.0
- ASP.NET Core Web API
- Entity Framework Core 8.0
- PostgreSQL
- Redis (Cache)
- RabbitMQ (Message Queue)
- AutoMapper
- FluentValidation
- Serilog
- Swagger/OpenAPI

## CQRS Pattern

### Commands (Write Operations)
- `CreatePersonCommand` - Kişi oluşturma
- `DeletePersonCommand` - Kişi silme
- `AddContactInfoCommand` - İletişim bilgisi ekleme
- `RequestReportCommand` - Rapor talebi

### Queries (Read Operations)
- `GetPersonByIdQuery` - Kişi detayı getirme
- `GetAllPersonsQuery` - Tüm kişileri listeleme

## Kurulum

### Gereksinimler
- .NET 8 SDK
- PostgreSQL
- Redis
- RabbitMQ (opsiyonel)

### Veritabanı Kurulumu
```bash
# PostgreSQL connection string
Host=localhost;Database=PhoneRegistryDb;Username=postgres;Password=postgres

# Migration çalıştırma
dotnet ef database update --project PhoneRegistry.Infrastructure --startup-project PhoneRegistry.ContactApi
```

### Çalıştırma
```bash
# Contact API
dotnet run --project PhoneRegistry.ContactApi

# Report API (ayrı port)
dotnet run --project PhoneRegistry.ReportApi
```

## API Endpoints

### Contact API (Port: 5000)
- `GET /api/persons` - Kişi listesi
- `GET /api/persons/{id}` - Kişi detayı
- `POST /api/persons` - Kişi oluşturma
- `DELETE /api/persons/{id}` - Kişi silme
- `POST /api/persons/{id}/contact-infos` - İletişim bilgisi ekleme

### Report API (Port: 5001)
- `POST /api/reports` - Rapor talebi
- `GET /api/reports` - Rapor listesi
- `GET /api/reports/{id}` - Rapor detayı

## Özellikler

- ✅ CQRS + Clean Architecture
- ✅ Domain-Driven Design
- ✅ Repository Pattern + Unit of Work
- ✅ Dependency Injection
- ✅ Validation (FluentValidation)
- ✅ AutoMapper
- ✅ Structured Logging (Serilog)
- ✅ Swagger Documentation
- ✅ CORS Support
- ✅ Redis Cache
- ✅ PostgreSQL Database
- ⏳ RabbitMQ Message Queue
- ⏳ Unit Tests (%60+ coverage)

## Geliştiriciler İçin

### Yeni Command Ekleme
1. `Application/Features/{Feature}/Commands/{CommandName}/` klasörü oluştur
2. Command, Validator ve Handler sınıflarını oluştur
3. DI container'a kaydet

### Yeni Query Ekleme
1. `Application/Features/{Feature}/Queries/{QueryName}/` klasörü oluştur
2. Query ve Handler sınıflarını oluştur
3. DI container'a kaydet

## Test Etme
```bash
# Unit testleri çalıştırma
dotnet test

# Coverage raporu
dotnet test --collect:"XPlat Code Coverage"
```

## Docker
```bash
# Docker Compose ile tüm servisleri başlatma
docker-compose up -d
```

Bu proje, modern .NET geliştirme pratikleri ve SOLID prensiplerine uygun olarak geliştirilmiştir.
