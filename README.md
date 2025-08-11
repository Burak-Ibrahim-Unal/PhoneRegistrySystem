## Phone Registry System (Microservices‑ready)

Modern, modüler ve mikroservis mimarisine uygunlaştırılmış telefon rehberi sistemi. Olay güdümlü okuma modeli, transactional outbox, Polly, OpenTelemetry ve Redis cache ile üretim koşullarına uygun bir altyapı örneği sunar.

- Backend: .NET 8 (ASP.NET Core, EF Core, MediatR)
- Messaging: RabbitMQ (Docker)
- Cache: Redis (Docker) – `PhoneRegistry.Caching`
- DB: PostgreSQL (tek DB, iki şema: `contact`, `report`)
- Observability: OpenTelemetry (console), Health Checks
- Resilience: Polly (retry + circuit breaker)
- Frontend: Angular + Material (`PhoneRegistrySystemClient`)

---

## İçindekiler
- Mimari Genel Bakış
- Bounded Context / Şemalar
- Event-driven Read Model ve Outbox
- Projeler ve Katmanlar
- Çalıştırma Ortamı ve Portlar
- Kurulum (Docker altyapısı + Local servisler)
- Veritabanı, Migration ve Seed
- Konfigürasyon Anahtarları
- Geliştirme Komutları
- API Uçları
- İsteğe Bağlı: API Gateway (YARP)
- Sorun Giderme (Troubleshooting)

---

## Mimari Genel Bakış
Sistem iki bounded context’e ayrılır: Kayıt (Contact) ve Rapor (Report).

- ContactApi (yazma + kaynak veri)
  - Şema: `contact`
  - Tablolar: `Persons`, `ContactInfos`, `Cities`, `Outbox`
  - Outbox pattern ile her değişiklikte integration event üretir.
- ReportApi (okuma + rapor)
  - Şema: `report`
  - Tablolar: `Reports`, `LocationStatistics`
  - Sorgularını kendi read model’inden yapar; Contact’a runtime çağrı yapmaz.
- WorkerService
  - Kuyruklar: `report-processing-queue` (rapor tetikleri), `contact-events` (projeksiyon güncellemeleri)
  - ContactApi’yi HTTP üzerinden çağırır (Polly ile retry + circuit breaker).

Yardımcı Class Library’ler:
- `PhoneRegistry.Messaging`: RabbitMQ bağlantısı, consumer/publisher ve mesaj sözleşmeleri
- `PhoneRegistry.Caching`: `ICacheService` ve Redis implementasyonu
- `PhoneRegistry.Services`: Application (MediatR) üstü servis katmanı

---

## Bounded Context / Şemalar (tek DB)
PostgreSQL tek veritabanı altında iki şema kullanılır.

- `contact`: `Persons`, `ContactInfos`, `Cities`, `Outbox`
- `report`: `Reports`, `LocationStatistics`

DbContext’ler:
- `ContactDbContext`: `contact` şeması
- `ReportDbContext`: `report` şeması
- `PhoneRegistryDbContext`: birleşik context (migration/seed kolaylığı için)

---

## Event‑driven Read Model ve Outbox
- ContactApi’deki yazma işlemleri aynı DB transaksiyonu içinde `contact.Outbox` tablosuna event kaydı ekler (Transactional Outbox).
- `OutboxPublisher` background service, Outbox’taki bekleyen kayıtları güvenilir şekilde RabbitMQ’ya yayınlar (durable + manual ack, idempotent tasarıma uygun).
- WorkerService `contact-events` kuyruğundan olayları tüketir ve `report.LocationStatistics` read model’ini günceller.
- Avantajlar: Gevşek bağ, düşük gecikme, başka servise runtime bağımlılık yok. Dezavantaj: eventual consistency ve event versiyonlama ihtiyacı.

Üretilen event örnekleri: `PersonUpserted`, `ContactInfoUpserted`, `ContactInfoDeleted`.

---

## Projeler ve Katmanlar
- `PhoneRegistry.ContactApi`: Kişi/iletişim/şehir CRUD; Outbox yazımı + publisher host eder.
- `PhoneRegistry.ReportApi`: Rapor talebi, rapor listesi ve detay sorguları.
- `PhoneRegistry.WorkerService`: Rapor işleme ve read model projeksiyonu (contact-events tüketir).
- `PhoneRegistry.Application`: CQRS (MediatR) komut/sorgu handler’ları.
- `PhoneRegistry.Domain`: Varlıklar, value object’ler, repository arayüzleri.
- `PhoneRegistry.Infrastructure`: EF Core context/migration, repository implementasyonları, OutboxWriter/Publisher DI.
- `PhoneRegistry.Messaging`: RabbitMQ consumer/publisher, connection service, message modelleri.
- `PhoneRegistry.Caching`: Redis cache servisi ve DI uzantısı.
- `PhoneRegistry.Services`: Uygulama servisleri (MediatR üzerinden domain işlemleri).

---

## Çalıştırma Ortamı ve Portlar
Portlar `launchSettings.json`’dan çekilir (lokalde varsayılanlar):
- ContactApi: HTTP `http://localhost:5297`, HTTPS `https://localhost:7065`
- ReportApi: HTTP `http://localhost:5142`, HTTPS `https://localhost:7239`
- Angular: `http://localhost:4300`
- RabbitMQ UI: `http://localhost:15672` (admin/admin123)

Frontend `environment.ts`:
```ts
export const environment = {
  production: false,
  contactApiUrl: 'https://localhost:7065',
  reportApiUrl: 'https://localhost:7239'
};
```

WorkerService Contact API Base URL (`PhoneRegistry.WorkerService/appsettings.json`):
```json
{
  "ContactApi": { "BaseUrl": "http://localhost:5297" }
}
```

---

## Kurulum
Önkoşullar: .NET 8 SDK, Node 18+, Docker Desktop, PostgreSQL (local)

1) Docker altyapısı (sadece RabbitMQ + Redis)
```powershell
docker compose up -d rabbitmq redis
```
`docker-compose.yml` içinde servisler tanımlıdır. Yoksa minimal içerik:
```yaml
services:
  rabbitmq:
    image: rabbitmq:3.12-management
    container_name: phoneregistry-rabbitmq
    hostname: rabbitmq
    ports: ["5672:5672", "15672:15672"]
    environment:
      - RABBITMQ_DEFAULT_USER=admin
      - RABBITMQ_DEFAULT_PASS=admin123
      - RABBITMQ_DEFAULT_VHOST=/
    volumes:
      - rabbitmq_data:/var/lib/rabbitmq
    networks: [phoneregistry-network]

  redis:
    image: redis:7-alpine
    container_name: phoneregistry-redis
    ports: ["6379:6379"]
    command: ["redis-server", "--appendonly", "yes"]
    volumes:
      - redis_data:/data
    networks: [phoneregistry-network]

volumes:
  rabbitmq_data:
  redis_data:

networks:
  phoneregistry-network:
    driver: bridge
```

2) Restore & Build
```powershell
dotnet restore
dotnet build
```

3) Servisleri çalıştırma
```powershell
# Contact API
dotnet run --project PhoneRegistry.ContactApi

# Report API
dotnet run --project PhoneRegistry.ReportApi

# Worker Service
dotnet run --project PhoneRegistry.WorkerService
```

4) Frontend (opsiyonel)
```powershell
cd PhoneRegistrySystemClient
npm install
npx ng serve -o --port 4300
```

---

## Veritabanı, Migration ve Seed
Uygulama açılışında `ContactDbContext` ve `ReportDbContext` için `Database.Migrate()` çağrılır. İlk çalıştırmada Seed:
- Şehirler: Ankara, İstanbul, İzmir (contact.Cities)
- Demo Kişiler + İletişim Bilgileri
- Raporlar: 1 completed, 1 preparing, 1 failed
- Sabit rapor girdisi (belirtilen sabit `Id` ile)

Outbox tablosu `contact.Outbox` Contact context içinde yer alır. Eğer migration’larınızda bu tablo yoksa aşağıdaki komutlarla ek migration oluşturup DB’yi güncelleyin:
```powershell
# Contact context (Outbox dahil)
dotnet ef migrations add AddOutboxTableContact \
  --project PhoneRegistry.Infrastructure \
  --startup-project PhoneRegistry.ContactApi \
  --context PhoneRegistry.Infrastructure.Data.ContactDbContext

dotnet ef database update \
  --project PhoneRegistry.Infrastructure \
  --startup-project PhoneRegistry.ContactApi \
  --context PhoneRegistry.Infrastructure.Data.ContactDbContext

# Report context (gerekirse)
dotnet ef migrations add UpdateReportSchema \
  --project PhoneRegistry.Infrastructure \
  --startup-project PhoneRegistry.ContactApi \
  --context PhoneRegistry.Infrastructure.Data.ReportDbContext

dotnet ef database update \
  --project PhoneRegistry.Infrastructure \
  --startup-project PhoneRegistry.ContactApi \
  --context PhoneRegistry.Infrastructure.Data.ReportDbContext
```
Not: `PhoneRegistry.ContactApi` startup’ı altında çalıştırmak, doğru connection string’leri kullanır.

---

## Konfigürasyon Anahtarları
- PostgreSQL: `ConnectionStrings:DefaultConnection` (ör.: `Host=localhost;Database=PhoneRegistryDb;Username=postgres;Password=1876`)
- RabbitMQ: `ConnectionStrings:RabbitMQ` (ör.: `amqp://admin:admin123@localhost:5672/`)
- Redis: `Redis:ConnectionString`, `Redis:Database`
- Worker Contact API: `ContactApi:BaseUrl`

Caching’i etkinleştirme (`Infrastructure.DependencyInjection` içinde çağrılır):
```csharp
services.AddCaching(configuration);
```

---

## Geliştirme Komutları
```powershell
# Temiz ve derle
dotnet clean && dotnet build

# EF Tools (kurulu değilse)
dotnet tool install --global dotnet-ef

# Sağlık uçları
# ContactApi: http://localhost:5297/health
# ReportApi : http://localhost:5142/health

# RabbitMQ UI
# http://localhost:15672 (admin/admin123)
```

---

## API Uçları (Özet)
- ContactApi
  - `GET /api/persons` (skip/take)
  - `GET /api/persons/{id}`
  - `POST /api/persons` (Kişi oluştur)
  - `DELETE /api/persons/{id}` (Kişi sil)
  - `POST /api/persons/{id}/contact-infos` (İletişim ekle; Location için `cityId` zorunlu)
  - `DELETE /api/persons/{id}/contact-infos/{contactInfoId}` (İletişim sil)
  - `GET /api/cities` (şehir listesi)
- ReportApi
  - `POST /api/reports` (Rapor talep et)
  - `GET /api/reports` (liste)
  - `GET /api/reports/{id}` (detay)

---

## İsteğe Bağlı: API Gateway (YARP)
Tek giriş noktası, ortak CORS/Auth/Rate‑Limit vs. istenirse YARP ile basit bir gateway eklenebilir.
- Örnek rotalar: `/contact/*` → ContactApi, `/report/*` → ReportApi
- Neden YARP: Modern, performanslı, .NET ekosisteminde destekli (Ocelot’a alternatif olarak önerilir).

---

## Sorun Giderme (Troubleshooting)
- 500: `relation "contact.Outbox" does not exist`
  - Çözüm: Contact context için yeni migration ekleyip `Outbox` tablosunu oluşturun ve `dotnet ef database update` çalıştırın (yukarıdaki komutlar). `Migrate()` yalnızca var olan migration’ları uygular; tablo modelde olup migration yoksa yeni migration şarttır.
- 500: ContactInfo (Location) eklerken hata
  - Frontend’de Tür=Konum seçildiğinde `cityId` zorunlu; formda şehir seçip gönderin.
- RabbitMQ bağlantı hatası
  - `docker compose up -d rabbitmq` ve connection string (`amqp://admin:admin123@localhost:5672/`) kontrol edin.
- Port uyuşmazlığı
  - Frontend `environment.ts` değerlerini Contact/Report `launchSettings.json` ile eşitleyin.

---

## Notlar
- Read model projeksiyonu basitleştirilmiştir; kişi‑şehir decrement için projection mapping store eklenmesi önerilir.
- Orta vadede her bounded context için ayrı DB ve gateway ile tek giriş noktası hedeflenebilir. 
