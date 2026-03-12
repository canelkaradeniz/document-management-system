using DocMan.Application.Common;
using DocMan.Application.Documents.DTOs;
using DocMan.Domain.Enums;
using MediatR;

namespace DocMan.Application.Documents.Commands;

public record UploadDocumentCommand : IRequest<Result<DocumentDto>>
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DocumentType Type { get; init; }
    public string FileExtension { get; init; } = string.Empty;
    public long FileSizeBytes { get; init; }
    public string ContentHash { get; init; } = string.Empty;
    public string UploadedBy { get; init; } = string.Empty;
    public List<string> Tags { get; init; } = new();
    public bool ForceUpload { get; init; } = false;
}
