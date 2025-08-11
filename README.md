## Phone Registry System (Microservices‑ready)

- Backend: .NET 8 (ASP.NET Core, EF Core, RabbitMQ, Redis, Polly, OpenTelemetry)
- Frontend: Angular (dizin: `PhoneRegistrySystemClient`)
- Mesajlaşma: RabbitMQ (Docker)
- Caching: Redis (Docker)
- Veritabanı: PostgreSQL (local, tek DB içinde `contact` ve `report` şemaları)

### Mimari
- ContactApi: `Persons`, `ContactInfos`, `Cities`, `Outbox` (şema: `contact`)
- ReportApi: `Reports`, `LocationStatistics` (şema: `report`)
- WorkerService: `report-processing-queue` ve `contact-events` tüketir
- `PhoneRegistry.Messaging`: mesaj sözleşmeleri + RabbitMQ publisher/consumer
- `PhoneRegistry.Caching`: `ICacheService` Redis implementasyonu

### Portlar (launchSettings.json)
- ContactApi: `http://localhost:5297`
- ReportApi: `http://localhost:5142`
- Worker -> ContactApi BaseUrl: `PhoneRegistry.WorkerService/appsettings.json` → `ContactApi:BaseUrl`

Not: Portlar makinede farklı olabilir; gerçek değerler ilgili `launchSettings.json` içindedir.

### Event‑driven Read Model + Outbox
- Contact değişiklikleri Outbox’a aynı transaksiyonda yazılır, `OutboxPublisher` RabbitMQ’ya yayınlar (`contact-events`).
- Worker bu event’leri tüketip `report.LocationStatistics` projeksiyonunu günceller.

### Dayanıklılık ve Gözlemlenebilirlik
- Polly: Retry (200ms, 500ms, 1s) + Circuit Breaker (5 hata/30s) Worker HTTP client’ında
- Health checks: `/health`
- OpenTelemetry: Console exporter, ASP.NET/HttpClient instrumentation

---

## Kurulum
Önkoşullar: .NET 8 SDK, Docker Desktop (RabbitMQ+Redis), PostgreSQL local kurulu

1) Docker altyapısı (RabbitMQ + Redis)
```powershell
docker compose up -d rabbitmq redis
```
RabbitMQ UI: `http://localhost:15672` (admin/admin123)

2) Restore & Build
```powershell
dotnet restore
dotnet build
```

3) Migration/DB (gerekirse)
```powershell
# Migration ekle (mevcut migrasyonlar yeterliyse atlayın)
dotnet ef migrations add Init --project PhoneRegistry.Infrastructure --startup-project PhoneRegistry.ContactApi --context PhoneRegistry.Infrastructure.Data.PhoneRegistryDbContext

# DB update
dotnet ef database update --project PhoneRegistry.Infrastructure --startup-project PhoneRegistry.ContactApi --context PhoneRegistry.Infrastructure.Data.PhoneRegistryDbContext
```
İlk çalıştırmada seeder şehirleri (Ankara, İstanbul, İzmir), demo verileri ve sabit raporu ekler.

---

## Çalıştırma
```powershell
# Contact API
dotnet run --project PhoneRegistry.ContactApi

# Report API
dotnet run --project PhoneRegistry.ReportApi

# Worker Service
dotnet run --project PhoneRegistry.WorkerService
```

Frontend (opsiyonel):
```powershell
cd PhoneRegistrySystemClient
npm install
npx ng serve -o --port 4300
```

---

## Konfigürasyon Anahtarları
- PostgreSQL: `ConnectionStrings:DefaultConnection`
- RabbitMQ: `ConnectionStrings:RabbitMQ`
- Redis: `Redis:ConnectionString`, `Redis:Database`
- Worker Contact API Base URL: `ContactApi:BaseUrl`

Caching’i etkinleştirme:
```csharp
services.AddCaching(configuration); // PhoneRegistry.Caching
```

---

## API Uçları
- ContactApi: `/api/persons`, `/api/persons/{id}`, `/api/persons/{id}/contact-infos`
- ReportApi: `/api/reports`, `/api/reports/{id}`

---

## Faydalı Komutlar
```powershell
# Temiz ve derle
dotnet clean && dotnet build

# Takılan dotnet süreçlerini kapatma
taskkill /F /IM dotnet.exe
``` 
