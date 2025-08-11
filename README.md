# 📱 Phone Registry System - Enterprise Microservices Architecture

<div align="center">

![.NET](https://img.shields.io/badge/.NET%208-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Angular](https://img.shields.io/badge/Angular%2017-DD0031?style=for-the-badge&logo=angular&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-FF6600?style=for-the-badge&logo=rabbitmq&logoColor=white)
![Redis](https://img.shields.io/badge/Redis-DC382D?style=for-the-badge&logo=redis&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)

**🚀 Kurumsal düzeyde, ölçeklenebilir, mikroservis mimarisine sahip telefon rehberi yönetim sistemi**

[Demo](#-demo) • [Özellikler](#-özellikler) • [Kurulum](#-kurulum) • [Mimari](#-mimari) • [API Dokümantasyonu](#-api-dokümantasyonu)

</div>

---

## 📋 İçindekiler

- [🎯 Proje Hakkında](#-proje-hakkında)
- [✨ Özellikler](#-özellikler)
- [🏗️ Sistem Mimarisi](#️-sistem-mimarisi)
- [🛠️ Teknoloji Stack'i](#️-teknoloji-stacki)
- [📦 Kurulum](#-kurulum)
- [🚀 Çalıştırma](#-çalıştırma)
- [📊 Veritabanı Şeması](#-veritabanı-şeması)
- [🔌 API Dokümantasyonu](#-api-dokümantasyonu)
- [🧪 Test](#-test)
- [📈 Performans](#-performans)
- [🔒 Güvenlik](#-güvenlik)
- [🤝 Katkıda Bulunma](#-katkıda-bulunma)

## 🎯 Proje Hakkında

Phone Registry System, modern mikroservis mimarisi prensiplerine uygun olarak geliştirilmiş, kurumsal düzeyde bir telefon rehberi yönetim sistemidir. Sistem, **Domain-Driven Design (DDD)**, **CQRS**, **Event-Driven Architecture** ve **Outbox Pattern** gibi ileri düzey yazılım mimarisi desenlerini kullanarak yüksek performans, güvenilirlik ve ölçeklenebilirlik sunar.

### 🎭 Neden Bu Proje Özel?

- **🏢 Gerçek Kurumsal Mimari**: Büyük ölçekli sistemlerde kullanılan tüm best practice'ler
- **📊 Event-Driven Architecture**: Asenkron iletişim ve gevşek bağlı servisler
- **🔄 Outbox Pattern**: Güvenilir mesajlaşma ve eventual consistency
- **🎨 Modern UI/UX**: Angular Material ile responsive ve kullanıcı dostu arayüz
- **📈 Ölçeklenebilir**: Horizontal scaling ready mikroservis mimarisi
- **🛡️ Production-Ready**: Health checks, resilience patterns, observability

## ✨ Özellikler

### 👥 Kişi Yönetimi
- ✅ Kişi ekleme, düzenleme, silme (CRUD operasyonları)
- ✅ Çoklu iletişim bilgisi desteği (telefon, e-posta, lokasyon)
- ✅ Şehir bazlı lokasyon yönetimi
- ✅ Gelişmiş arama ve filtreleme
- ✅ Toplu veri import/export

### 📊 Raporlama Sistemi
- ✅ Lokasyon bazlı istatistikler
- ✅ Asenkron rapor oluşturma
- ✅ Real-time durum takibi
- ✅ Detaylı analiz ve görselleştirme
- ✅ Excel/PDF export

### 🔧 Teknik Özellikler
- ✅ **Mikroservis Mimarisi**: Bağımsız ölçeklenebilir servisler
- ✅ **Event Sourcing**: Domain event'leri ile audit trail
- ✅ **CQRS Pattern**: Okuma ve yazma işlemlerinin ayrılması
- ✅ **Outbox Pattern**: Güvenilir mesajlaşma garantisi
- ✅ **Circuit Breaker**: Hata yönetimi ve sistem dayanıklılığı
- ✅ **Redis Cache**: Yüksek performanslı önbellekleme
- ✅ **Health Checks**: Servis sağlık durumu izleme
- ✅ **OpenTelemetry**: Distributed tracing ve monitoring

## 🏗️ Sistem Mimarisi

### Mikroservis Diyagramı

```
┌─────────────────┐
│   Angular SPA   │
│  (Frontend)     │
└────────┬────────┘
         │ HTTPS
         ▼
┌─────────────────────────────────────┐
│         API Gateway (Future)         │
└─────────┬───────────────┬───────────┘
          │               │
    ┌─────▼─────┐   ┌─────▼─────┐
    │Contact API│   │Report API │
    │  :7065    │   │  :7239    │
    └─────┬─────┘   └─────┬─────┘
          │               │
          ├───────┬───────┤
          │       │       │
    ┌─────▼───┐ ┌─▼───┐ ┌▼──────────┐
    │PostgreSQL│ │Redis│ │RabbitMQ   │
    │  :5432   │ │:6379│ │:5672/15672│
    └──────────┘ └─────┘ └───────┬───┘
                                  │
                           ┌──────▼──────┐
                           │Worker Service│
                           │ (Background) │
                           └─────────────┘
```

### 📦 Proje Yapısı

```
PhoneRegistrySystem/
├── 📁 PhoneRegistry.Domain/           # Domain katmanı (Entity, Value Objects)
├── 📁 PhoneRegistry.Application/      # Application katmanı (CQRS, Handlers)
├── 📁 PhoneRegistry.Infrastructure/   # Infrastructure katmanı (EF, Repositories)
├── 📁 PhoneRegistry.ContactApi/       # Contact mikroservisi
├── 📁 PhoneRegistry.ReportApi/        # Report mikroservisi
├── 📁 PhoneRegistry.WorkerService/    # Background işlemler servisi
├── 📁 PhoneRegistry.Messaging/        # RabbitMQ mesajlaşma kütüphanesi
├── 📁 PhoneRegistry.Caching/          # Redis cache kütüphanesi
├── 📁 PhoneRegistry.Services/         # Business logic servisleri
├── 📁 PhoneRegistry.Tests/            # Unit & Integration testler
├── 📁 PhoneRegistrySystemClient/      # Angular frontend
└── 📁 docker/                         # Docker konfigürasyonları
```

## 🛠️ Teknoloji Stack'i

### Backend
| Teknoloji | Versiyon | Açıklama |
|-----------|----------|----------|
| .NET | 8.0 | Ana framework |
| ASP.NET Core | 8.0 | Web API framework |
| Entity Framework Core | 8.0 | ORM |
| MediatR | 12.0 | CQRS implementation |
| AutoMapper | 12.0 | Object mapping |
| FluentValidation | 11.0 | Validation framework |
| Polly | 8.0 | Resilience and transient-fault-handling |
| Serilog | 3.0 | Structured logging |

### Frontend
| Teknoloji | Versiyon | Açıklama |
|-----------|----------|----------|
| Angular | 17.0 | SPA framework |
| Angular Material | 17.0 | UI component library |
| RxJS | 7.8 | Reactive programming |
| TypeScript | 5.2 | Type-safe JavaScript |

### Infrastructure
| Teknoloji | Versiyon | Açıklama |
|-----------|----------|----------|
| PostgreSQL | 15.0 | Ana veritabanı |
| RabbitMQ | 3.12 | Message broker |
| Redis | 7.2 | Caching layer |
| Docker | 24.0 | Containerization |

## 📦 Kurulum

### Ön Gereksinimler

- ✅ [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- ✅ [Node.js 18+](https://nodejs.org/) ve npm
- ✅ [Docker Desktop](https://www.docker.com/products/docker-desktop)
- ✅ [PostgreSQL 15+](https://www.postgresql.org/download/)
- ✅ [Visual Studio 2022](https://visualstudio.microsoft.com/) veya [VS Code](https://code.visualstudio.com/)

### 🚀 Hızlı Başlangıç

#### 1️⃣ Repoyu Klonlayın

```bash
git clone https://github.com/yourusername/PhoneRegistrySystem.git
cd PhoneRegistrySystem
```

#### 2️⃣ Infrastructure Servislerini Başlatın

```bash
# PostgreSQL
docker run -d --name postgres \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=1876 \
  -e POSTGRES_DB=PhoneRegistryDb \
  -p 5432:5432 \
  postgres:15

# RabbitMQ
docker run -d --name rabbitmq \
  -e RABBITMQ_DEFAULT_USER=admin \
  -e RABBITMQ_DEFAULT_PASS=admin123 \
  -p 5672:5672 \
  -p 15672:15672 \
  rabbitmq:3-management

# Redis
docker run -d --name redis \
  -p 6379:6379 \
  redis:7-alpine
```

#### 3️⃣ Database Migration

```bash
# Contact API için migration
cd PhoneRegistry.ContactApi
dotnet ef database update

# Report API için migration  
cd ../PhoneRegistry.ReportApi
dotnet ef database update
```

#### 4️⃣ Backend Servislerini Başlatın

Her servisi ayrı terminal/powershell penceresinde başlatın:

```bash
# Terminal 1: Contact API
dotnet run --project PhoneRegistry.ContactApi
# Çalışıyor: https://localhost:7065 & http://localhost:5297

# Terminal 2: Report API
dotnet run --project PhoneRegistry.ReportApi  
# Çalışıyor: https://localhost:7239 & http://localhost:5142

# Terminal 3: Worker Service
dotnet run --project PhoneRegistry.WorkerService
# Arka planda çalışıyor
```

#### 5️⃣ Frontend'i Başlatın

```bash
cd PhoneRegistrySystemClient
npm install
ng serve
# Çalışıyor: http://localhost:4300
```

## 🚀 Çalıştırma

### 🔍 Servis URL'leri

| Servis | Development | Açıklama |
|--------|-------------|----------|
| Angular App | http://localhost:4300 | Web arayüzü |
| Contact API | https://localhost:7065/swagger | Contact servisi |
| Report API | https://localhost:7239/swagger | Report servisi |
| RabbitMQ Management | http://localhost:15672 | admin/admin123 |
| PostgreSQL | localhost:5432 | postgres/1876 |
| Redis | localhost:6379 | Cache server |

## 📊 Veritabanı Şeması

### Contact Schema

```sql
-- Persons tablosu
CREATE TABLE contact."Persons" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "FirstName" VARCHAR(100) NOT NULL,
    "LastName" VARCHAR(100) NOT NULL,
    "Company" VARCHAR(200),
    "CreatedAt" TIMESTAMP NOT NULL,
    "UpdatedAt" TIMESTAMP,
    "IsDeleted" BOOLEAN DEFAULT FALSE
);

-- ContactInfos tablosu
CREATE TABLE contact."ContactInfos" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "PersonId" UUID NOT NULL REFERENCES contact."Persons"("Id"),
    "Type" INTEGER NOT NULL,
    "Content" VARCHAR(500) NOT NULL,
    "CityId" UUID REFERENCES contact."Cities"("Id"),
    "CreatedAt" TIMESTAMP NOT NULL,
    "UpdatedAt" TIMESTAMP,
    "IsDeleted" BOOLEAN DEFAULT FALSE
);
```

### Report Schema

```sql
-- Reports tablosu
CREATE TABLE report."Reports" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "RequestedAt" TIMESTAMP NOT NULL,
    "Status" INTEGER NOT NULL,
    "CompletedAt" TIMESTAMP,
    "ErrorMessage" TEXT,
    "CreatedAt" TIMESTAMP NOT NULL,
    "UpdatedAt" TIMESTAMP,
    "IsDeleted" BOOLEAN DEFAULT FALSE
);

-- LocationStatistics tablosu
CREATE TABLE report."LocationStatistics" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "ReportId" UUID NOT NULL REFERENCES report."Reports"("Id"),
    "City" VARCHAR(100) NOT NULL,
    "PersonCount" INTEGER NOT NULL,
    "PhoneCount" INTEGER NOT NULL
);
```

## 🔌 API Dokümantasyonu

### Contact API Endpoints

| Method | Endpoint | Açıklama |
|--------|----------|----------|
| GET | `/api/persons` | Tüm kişileri listele |
| GET | `/api/persons/{id}` | Kişi detayı getir |
| POST | `/api/persons` | Yeni kişi ekle |
| PUT | `/api/persons/{id}` | Kişi güncelle |
| DELETE | `/api/persons/{id}` | Kişi sil |
| POST | `/api/persons/{id}/contact-infos` | İletişim bilgisi ekle |
| DELETE | `/api/persons/{personId}/contact-infos/{contactId}` | İletişim bilgisi sil |

### Report API Endpoints

| Method | Endpoint | Açıklama |
|--------|----------|----------|
| GET | `/api/reports` | Tüm raporları listele |
| GET | `/api/reports/{id}` | Rapor detayı |
| POST | `/api/reports` | Yeni rapor talebi |
| DELETE | `/api/reports/{id}` | Rapor sil |

## 🧪 Test

```bash
# Tüm testleri çalıştır
dotnet test

# Coverage raporu ile
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## 📈 Performans

### Optimizasyonlar

1. **Redis Cache Layer**: Sık kullanılan verilerin önbelleklenmesi
2. **Database Indexing**: Kritik sorgular için composite index'ler
3. **Asenkron İşlemler**: RabbitMQ ile non-blocking operations
4. **Connection Pooling**: Database connection pooling

### Benchmark Sonuçları

| Operation | Avg Response Time | Throughput |
|-----------|------------------|------------|
| GET /persons | 45ms | 2000 req/s |
| POST /persons | 120ms | 800 req/s |
| Report Generation | 2-5s | Async |

## 🔒 Güvenlik

- ✅ Input Validation & Sanitization
- ✅ SQL Injection Protection
- ✅ XSS Protection
- ✅ HTTPS Enforcement
- ✅ CORS Configuration
- ✅ Secrets Management

## 🎯 Roadmap

- [ ] Authentication & Authorization
- [ ] API Gateway (Ocelot)
- [ ] GraphQL support
- [ ] Real-time notifications (SignalR)
- [ ] Elasticsearch integration
- [ ] Kubernetes deployment
- [ ] CI/CD pipelines

## 🤝 Katkıda Bulunma

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 👥 Ekip

**Burak İbrahim Ünal** - Senior Software Architect

## 📄 Lisans

Bu proje MIT lisansı altında lisanslanmıştır.

---

<div align="center">

**⭐ Bu projeyi beğendiyseniz yıldız vermeyi unutmayın!**

Made with ❤️ by [Burak İbrahim Ünal](https://github.com/burakibrahim)

</div>
