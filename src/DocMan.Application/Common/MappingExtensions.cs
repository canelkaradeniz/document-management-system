using DocMan.Application.Documents.DTOs;
using DocMan.Domain.Entities;
using DocMan.Domain.Enums;

namespace DocMan.Application.Common;

public static class MappingExtensions
{
    public static DocumentDto ToDto(this Document doc) => new(
        doc.Id,
        doc.Name,
        doc.Description,
        doc.Type,
        doc.Type.ToDisplayName(),
        doc.FileExtension,
        doc.FileSizeBytes,
        FormatFileSize(doc.FileSizeBytes),
        doc.UploadedBy,
        doc.CreatedAt,
        doc.UpdatedAt,
        doc.Tags
    );

    public static DocumentListItemDto ToListItemDto(this Document doc) => new(
        doc.Id,
        doc.Name,
        doc.Type,
        doc.Type.ToDisplayName(),
        doc.FileExtension,
        FormatFileSize(doc.FileSizeBytes),
        doc.UploadedBy,
        doc.CreatedAt,
        doc.Tags
    );

    public static string ToDisplayName(this DocumentType type) => type switch
    {
        DocumentType.Contract => "Sözleşme",
        DocumentType.Proposal => "Teklif",
        DocumentType.Invoice => "Fatura",
        _ => type.ToString()
    };

    private static string FormatFileSize(long bytes)
    {
        string[] sizes = ["B", "KB", "MB", "GB"];
        int order = 0;
        double size = bytes;
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        return $"{size:0.##} {sizes[order]}";
    }
}
