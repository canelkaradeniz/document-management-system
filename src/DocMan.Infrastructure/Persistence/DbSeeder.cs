using DocMan.Domain.Entities;
using DocMan.Domain.Enums;

namespace DocMan.Infrastructure.Persistence;

public static class DbSeeder
{
    public static void Seed(AppDbContext context)
    {
        if (context.Documents.Any()) return;

        var documents = new List<Document>
        {
            new()
            {
                Id = Guid.NewGuid(), Name = "2024 Yıllık Tedarik Sözleşmesi",
                Description = "ABC Ltd. ile yapılan yıllık tedarik sözleşmesi", Type = DocumentType.Contract,
                FileExtension = ".pdf", FileSizeBytes = 2_500_000,
                ContentHash = "a1b2c3d4e5f6", UploadedBy = "ahmet.yilmaz",
                CreatedAt = DateTime.UtcNow.AddDays(-90), Tags = new List<string> { "tedarik", "2024", "abc-ltd" }
            },
            new()
            {
                Id = Guid.NewGuid(), Name = "Q1 2024 Satış Teklifi - XYZ Corp",
                Description = "XYZ Corp için hazırlanan Q1 satış teklifi", Type = DocumentType.Proposal,
                FileExtension = ".docx", FileSizeBytes = 1_200_000,
                ContentHash = "b2c3d4e5f6g7", UploadedBy = "mehmet.demir",
                CreatedAt = DateTime.UtcNow.AddDays(-75), Tags = new List<string> { "satış", "q1", "xyz-corp" }
            },
            new()
            {
                Id = Guid.NewGuid(), Name = "Ocak 2024 Faturası #1001",
                Description = "Ocak ayı hizmet faturası", Type = DocumentType.Invoice,
                FileExtension = ".pdf", FileSizeBytes = 500_000,
                ContentHash = "c3d4e5f6g7h8", UploadedBy = "ayse.kaya",
                CreatedAt = DateTime.UtcNow.AddDays(-60), Tags = new List<string> { "fatura", "ocak", "hizmet" }
            },
            new()
            {
                Id = Guid.NewGuid(), Name = "Bakım Sözleşmesi - DEF Teknoloji",
                Description = "Yıllık IT bakım ve destek sözleşmesi", Type = DocumentType.Contract,
                FileExtension = ".pdf", FileSizeBytes = 3_100_000,
                ContentHash = "d4e5f6g7h8i9", UploadedBy = "ahmet.yilmaz",
                CreatedAt = DateTime.UtcNow.AddDays(-45), Tags = new List<string> { "bakım", "it", "def-teknoloji" }
            },
            new()
            {
                Id = Guid.NewGuid(), Name = "Şubat 2024 Faturası #1002",
                Description = "Şubat ayı yazılım lisans faturası", Type = DocumentType.Invoice,
                FileExtension = ".pdf", FileSizeBytes = 450_000,
                ContentHash = "e5f6g7h8i9j0", UploadedBy = "ayse.kaya",
                CreatedAt = DateTime.UtcNow.AddDays(-30), Tags = new List<string> { "fatura", "şubat", "lisans" }
            },
            new()
            {
                Id = Guid.NewGuid(), Name = "Danışmanlık Teklifi - GHI Consulting",
                Description = "Dijital dönüşüm danışmanlık teklifi", Type = DocumentType.Proposal,
                FileExtension = ".pptx", FileSizeBytes = 5_800_000,
                ContentHash = "f6g7h8i9j0k1", UploadedBy = "mehmet.demir",
                CreatedAt = DateTime.UtcNow.AddDays(-20), Tags = new List<string> { "danışmanlık", "dijital-dönüşüm" }
            },
            new()
            {
                Id = Guid.NewGuid(), Name = "Mart 2024 Faturası #1003",
                Description = "Mart ayı hosting faturası", Type = DocumentType.Invoice,
                FileExtension = ".pdf", FileSizeBytes = 320_000,
                ContentHash = "g7h8i9j0k1l2", UploadedBy = "ayse.kaya",
                CreatedAt = DateTime.UtcNow.AddDays(-10), Tags = new List<string> { "fatura", "mart", "hosting" }
            },
            new()
            {
                Id = Guid.NewGuid(), Name = "Gizlilik Sözleşmesi - NDA Template",
                Description = "Standart NDA şablonu - tüm tedarikçiler için", Type = DocumentType.Contract,
                FileExtension = ".docx", FileSizeBytes = 180_000,
                ContentHash = "h8i9j0k1l2m3", UploadedBy = "zeynep.ozturk",
                CreatedAt = DateTime.UtcNow.AddDays(-5), Tags = new List<string> { "nda", "gizlilik", "şablon" }
            },
            new()
            {
                Id = Guid.NewGuid(), Name = "Q2 2024 Satış Teklifi - MNO Group",
                Description = "MNO Group için Q2 dönemi özel fiyat teklifi", Type = DocumentType.Proposal,
                FileExtension = ".xlsx", FileSizeBytes = 980_000,
                ContentHash = "i9j0k1l2m3n4", UploadedBy = "mehmet.demir",
                CreatedAt = DateTime.UtcNow.AddDays(-3), Tags = new List<string> { "satış", "q2", "mno-group" }
            },
            new()
            {
                Id = Guid.NewGuid(), Name = "Yıllık Tedarik Sözleşmesi - 2024 Revize",
                Description = "ABC Ltd. sözleşmesinin revize edilmiş versiyonu", Type = DocumentType.Contract,
                FileExtension = ".pdf", FileSizeBytes = 2_700_000,
                ContentHash = "j0k1l2m3n4o5", UploadedBy = "ahmet.yilmaz",
                CreatedAt = DateTime.UtcNow.AddDays(-1), Tags = new List<string> { "tedarik", "2024", "abc-ltd", "revize" }
            }
        };

        context.Documents.AddRange(documents);
        context.SaveChanges();
    }
}
