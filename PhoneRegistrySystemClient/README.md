# PhoneRegistrySystemClient

Bu proje [Angular CLI](https://github.com/angular/angular-cli) version 17.0.0 ile oluşturulmuştur.

## 🚀 Geliştirme Sunucusu

Geliştirme sunucusunu başlatmak için `ng serve` komutunu çalıştırın. `http://localhost:4200/` adresine gidin. Kaynak dosyalarından herhangi birini değiştirirseniz uygulama otomatik olarak yeniden yüklenir.

## 📦 Kurulum

```bash
# Bağımlılıkları yükleyin
npm install

# Geliştirme sunucusunu başlatın
npm start
# veya
ng serve
```

## 🏗️ Proje Yapısı

```
src/
├── app/
│   ├── core/                 # Temel servisler ve modeller
│   │   ├── models/          # TypeScript modelleri
│   │   └── services/        # API servisleri
│   ├── features/            # Feature modülleri
│   │   └── persons/         # Kişi yönetimi
│   ├── shared/              # Paylaşılan componentler
│   └── environments/        # Ortam yapılandırmaları
└── assets/                  # Statik dosyalar
```

## 🎨 UI Framework

- **Angular Material** - Modern component library
- **Bootstrap 5** - Responsive grid system
- **FontAwesome** - Icon library
- **Custom SCSS** - Modern styling

## 🔧 Yapı

Projeyi derlemek için `ng build` komutunu çalıştırın. Derleme artefaktları `dist/` dizininde saklanacaktır.

## 🧪 Unit Testleri

Unit testleri çalıştırmak için `ng test` komutunu çalıştırın. Bu komut [Karma](https://karma-runner.github.io) aracılığıyla çalışır.

## 🔍 End-to-End Testleri

End-to-end testleri çalıştırmak için `ng e2e` komutunu çalıştırın. Bu komutu kullanmadan önce e2e test platformu ekleyen bir paket eklemeniz gerekir.

## 📚 Daha Fazla Yardım

Angular CLI hakkında daha fazla yardım almak için `ng help` komutunu kullanın veya [Angular CLI Overview and Command Reference](https://angular.io/cli) sayfasına göz atın.

## 🌐 API Endpoints

### Persons API (Port: 5000)
- `GET /api/persons` - Kişileri listele
- `GET /api/persons/{id}` - Kişi detayını getir
- `POST /api/persons` - Yeni kişi oluştur
- `DELETE /api/persons/{id}` - Kişi sil
- `POST /api/persons/{id}/contact-infos` - İletişim bilgisi ekle
- `DELETE /api/persons/{id}/contact-infos/{contactId}` - İletişim bilgisi sil

### Reports API (Port: 5001)
- `GET /api/reports` - Raporları listele
- `GET /api/reports/{id}` - Rapor detayını getir
- `POST /api/reports` - Yeni rapor talep et

## 🔧 Yapılandırma

`src/environments/environment.ts` dosyasında API URL'lerini yapılandırabilirsiniz:

```typescript
export const environment = {
  production: false,
  contactApiUrl: 'http://localhost:5000',
  reportApiUrl: 'http://localhost:5001'
};
```
