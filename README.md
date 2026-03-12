# DocMan - Doküman Yönetim Sistemi

Şirket içi doküman yönetim sistemindeki arama ve tekrar yükleme sorunlarını çözmek için geliştirilmiş bir prototip.

## Hızlı Başlangıç

### Gereksinimler
- .NET 8 SDK
- Node.js 20+

### Backend
```bash
cd src/DocMan.API
dotnet run
```
API: http://localhost:5218 | Swagger: http://localhost:5218/swagger

### Frontend
```bash
cd client
npm install
npm run dev
```
UI: http://localhost:5173

---

## 1. Problemi Yorumlama

### Problemi nasıl tanımlıyorum?

Kullanıcılar doküman bulamıyor ve bu yüzden aynı dosyaları tekrar yüklüyor. "Arama sonuçları karışık" geri bildirimi, sistemin dokümanları organize etme ve sunma biçiminde bir sorun olduğuna işaret ediyor.

Ama buradaki asıl mesele şu: **kullanıcılar sisteme güvenmiyor.** Bir şeyi aradıklarında bulamayınca, "bu sistem işe yaramaz" deyip doğrudan yeni yükleme yapıyorlar. Bu bir arama algoritması sorunundan çok, **kullanıcı deneyimi ve güven sorunu.**

### Gerçek kök neden ne olabilir?

Birkaç olasılık var:
1. **Zayıf metadata**: Dokümanlar yüklenirken yeterli etiket, açıklama veya kategori bilgisi girilmiyor olabilir
2. **İsimlendirme standardı yok**: "sözleşme_v2_final_GERCEK_final.pdf" gibi isimler arama sonuçlarını bozmakla birlikte kullanıcıyı da yanlış yönlendiriyor
3. **Arama mekanizması yetersiz**: Tam eşleşme arıyor olabilir, fuzzy search veya partial match desteklemiyor olabilir
4. **Sonuç sunumu kötü**: Arama sonuçları tarih, tip gibi anlamlı bilgiler olmadan düz liste halinde gösteriliyor olabilir

### Bu talep yanlış bir varsayıma dayanıyor olabilir mi?

Evet, birkaç ihtimal:
- "Arama bozuk" deniyor ama belki arama gayet iyi çalışıyor, sorun **dokümanların doğru şekilde kategorize edilmemiş** olmasında
- Belki sorun aramada değil, kullanıcıların **hangi dokümanın güncel olduğunu anlayamamasında** (versiyon sorunu)
- "Aynı dokümanı tekrar yüklüyorum" diyen kullanıcı belki gerçekten farklı bir versiyon yüklüyor ama bunu duplicate olarak algılıyor

### Eksik bilgiler
- Mevcut arama nasıl çalışıyor? (full-text mi, sadece isim bazlı mı?)
- Doküman sayısı ne kadar? (1000 ile 1 milyon arası çok farklı yaklaşımlar gerektirir)
- Kullanıcılar departmanlara göre ayrışıyor mu? Herkes her şeyi mi görüyor?
- "Arama sonuçları karışık" tam olarak ne demek? Çok fazla mı sonuç dönüyor, yoksa alakasız mı?

---

## 2. Karar ve Tasarım Yaklaşımı

### Seçtiğim yaklaşım ve neden

**CQRS (Command Query Responsibility Segregation)** pattern'i ile temiz bir katmanlı mimari tercih ettim.

Neden:
- Read ve Write operasyonları farklı optimizasyonlar gerektiriyor. Arama yoğun bir sistem, CQRS buna çok uygun.
- Her query/command bağımsız test edilebilir
- İleride read tarafına cache eklemek veya farklı bir arama motoru (Elasticsearch gibi) koymak kolay

### Bilerek kabul ettiğim riskler
- **SQLite kullanımı**: Production'da kullanılmaz ama prototip için yeterli. Gerçek senaryoda PostgreSQL veya mevcut veritabanı kullanılır.
- **Dosya içeriği yok**: Sadece metadata yönetimi var, gerçek dosya upload/download yok. Case study'nin odağı arama ve duplicate tespiti olduğu için scope dışı bıraktım.
- **Authentication yok**: Kullanıcı bilgisi hardcoded. Gerçek sistemde SSO/LDAP entegrasyonu olur.

### Yapmamayı tercih ettiğim şeyler
- **Elasticsearch entegrasyonu**: Kısıt olarak "ek altyapı yatırımı yapılmayacak" denmiş. Ayrıca 8000 kullanıcı için overkill.
- **Dosya versiyonlama**: Önemli bir özellik ama MVP kapsamını şişirirdi.
- **Full-text search motoru**: SQLite LIKE sorgusu 8000 kullanıcı ve makul doküman sayısı için yeterli. 400ms kısıtı içinde kalıyor.
- **Microservice mimarisi**: Monolith bu ölçek için doğru karar.

### MVP Kapsamı
1. **Doküman listeleme** (filtreleme + sayfalama)
2. **Arama** (isim ve açıklama üzerinde, tip ve tarih filtresi ile)
3. **Duplicate tespiti** (SHA-256 hash ile birebir aynı dosya kontrolü + benzer isim uyarısı)
4. **Anlamlı geri bildirim** (duplicate uyarıları, boş sonuç mesajları, etiket sistemi)

### Mimari Diyagram

```
┌─────────────────────────────────────────────────┐
│                   React SPA                      │
│         (Arama, Listeleme, Yükleme)             │
└─────────────────┬───────────────────────────────┘
                  │ HTTP/REST
┌─────────────────▼───────────────────────────────┐
│              ASP.NET Core API                    │
│            (Controllers + CORS)                  │
├─────────────────────────────────────────────────┤
│           Application Layer (CQRS)               │
│  ┌──────────────┐  ┌─────────────────────────┐  │
│  │   Commands    │  │        Queries          │  │
│  │ UploadDoc     │  │ SearchDocuments         │  │
│  │ (+ Validation)│  │ GetDocumentById         │  │
│  └──────────────┘  └─────────────────────────┘  │
│              MediatR Pipeline                    │
├─────────────────────────────────────────────────┤
│         Infrastructure Layer                     │
│  ┌──────────────────────────────────────┐       │
│  │      EF Core + Repository Pattern     │       │
│  │   (Index'ler: Hash, Name, Type, Date) │       │
│  └──────────────────┬───────────────────┘       │
└─────────────────────┼───────────────────────────┘
                      │
               ┌──────▼──────┐
               │   SQLite    │
               │  (Prototip) │
               └─────────────┘
```

---

## 3. Çalışan Prototip

### Özellikler
- **Doküman Listeleme**: Tüm dokümanlar tablo görünümünde, sayfalama ile
- **Arama ve Filtreleme**: İsim/açıklama üzerinde arama, tip filtresi, kullanıcı filtresi
- **Duplicate Tespiti**:
  - Dosya seçildiğinde SHA-256 hash hesaplanıp backend'e sorulur
  - Aynı hash varsa kullanıcıya "bu dosya zaten var" uyarısı gösterilir
  - Kullanıcı isterse yine de yükleyebilir (bilinçli karar)
  - Benzer isimli dokümanlar da uyarı olarak gösterilir
- **Etiket Sistemi**: Her doküman etiketlenebilir, aramayı iyileştirir
- **Anlamlı Geri Bildirim**: Boş sonuçlarda yönlendirici mesajlar, yükleme sonuçları

---

## 4. Teknik Değerlendirme

### 1. Bu çözüm 6 ay sonra neden problem çıkarabilir?

SQLite single-writer limitation'ı yüzünden eşzamanlı yazma işlemlerinde lock'lar oluşabilir. Doküman sayısı arttıkça LIKE bazlı arama yavaşlar. Ayrıca şu an dosya içeriği indexlenmiyor; kullanıcılar zamanla "dosyanın içinde şunu arıyorum" demeye başlayacak.

### 2. 10.000 kullanıcıya ölçeklendiğinde ilk kırılacak nokta neresi olur?

Veritabanı katmanı. SQLite concurrent read'lerde iyi ama write lock'ları darboğaz yaratır. Arama sorguları LIKE ile yapıldığı için index'ler yetmez hale gelir. İlk adım PostgreSQL'e geçiş ve connection pooling olur.

### 3. En zayıf gördüğüm teknik kararım nedir?

Benzer isim kontrolü (GetSimilarByNameAsync) çok basit bir LIKE sorgusu ile yapılıyor. Gerçek hayatta Levenshtein distance veya TF-IDF gibi bir benzerlik algoritması kullanılmalı. Şu anki yaklaşım "sözleşme" aramasında "sözleşme_v2"yi bulur ama "kontrat"ı bulamaz.

### 4. Bu çözümde beni en rahatsız eden teknik nokta nedir?

Authentication ve authorization'ın olmaması. Gerçek bir doküman yönetim sisteminde "kim neyi görebilir" çok kritik. Şu an tüm dokümanlar herkese açık. Ayrıca dosya içeriğinin fiziksel olarak yönetilmemesi (sadece metadata var) prototipin en büyük eksikliği.

---

## 5. İletişim Bölümü

### İş Birimine Açıklama

> Doküman arama sistemini iyileştirdik. Artık kullanıcılar dokümanlarını isim, tip ve tarih gibi filtrelerle çok daha kolay bulabilecek.
>
> Ayrıca aynı dosyayı tekrar yüklemek isteyen kullanıcılara "bu dosya zaten sistemde var" uyarısı gösteriyoruz. Böylece hem gereksiz tekrar yüklemeler azalacak, hem de kullanıcılar aradıkları dokümanın zaten sistemde olduğunu görebilecek.
>
> Etiket sistemi sayesinde dokümanlar daha kolay organize edilebilecek. Örneğin "fatura", "2024", "abc-ltd" gibi etiketler eklenebilecek ve bu etiketlerle arama yapılabilecek.

### CTO'ya Teknik Özet

> **Ne yaptık**: CQRS pattern ile arama ve yükleme operasyonlarını ayırdık. SHA-256 hash ile birebir duplicate tespiti, LIKE bazlı arama ve etiket sistemi ekledik.
>
> **Teknik borç**:
> - SQLite prototip için kullanıldı, production'da PostgreSQL'e geçilmeli
> - Full-text search yok, doküman sayısı 50K'yı geçerse performans sorunu olabilir
> - Fuzzy search yok, sadece partial match var
> - Auth/authz eklenmeli
> - Dosya storage yönetimi (S3/Azure Blob) entegre edilmeli
>
> **Riskler**:
> - Mevcut DB constraint'i yüzünden arama optimizasyonu sınırlı kalıyor
> - 400ms response time hedefi şu an karşılanıyor ama doküman sayısı arttıkça index optimizasyonu gerekecek
>
> **Önerilen sonraki adımlar**: PostgreSQL full-text search (pg_trgm) entegrasyonu, rate limiting, ve kullanıcı bazlı erişim kontrolü.

---

## Teknoloji Stack

| Katman | Teknoloji |
|--------|-----------|
| Backend | .NET 8, ASP.NET Core Web API |
| Pattern | CQRS (MediatR), Repository Pattern |
| Validation | FluentValidation |
| ORM | Entity Framework Core 8 |
| Database | SQLite (prototip) |
| Frontend | React 19 + TypeScript + Vite |
| API İletişimi | Axios |

## Proje Yapısı

```
├── src/
│   ├── DocMan.Domain/          # Entity'ler ve Enum'lar
│   ├── DocMan.Application/     # CQRS Commands, Queries, DTOs
│   ├── DocMan.Infrastructure/  # EF Core, Repository implementasyonu
│   └── DocMan.API/             # Controller'lar, DI konfigürasyonu
├── client/                     # React SPA
└── README.md
```
