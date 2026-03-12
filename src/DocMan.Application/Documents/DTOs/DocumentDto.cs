using DocMan.Domain.Enums;

namespace DocMan.Application.Documents.DTOs;

public record DocumentDto(
    Guid Id,
    string Name,
    string? Description,
    DocumentType Type,
    string TypeDisplayName,
    string FileExtension,
    long FileSizeBytes,
    string FileSizeDisplay,
    string UploadedBy,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    List<string> Tags
);

public record DocumentListItemDto(
    Guid Id,
    string Name,
    DocumentType Type,
    string TypeDisplayName,
    string FileExtension,
    string FileSizeDisplay,
    string UploadedBy,
    DateTime CreatedAt,
    List<string> Tags
);

public record DuplicateCheckResult(
    bool IsDuplicate,
    Guid? ExistingDocumentId,
    string? ExistingDocumentName,
    string Message
);
