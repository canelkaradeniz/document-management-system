using DocMan.Application.Common;
using DocMan.Application.Documents.DTOs;
using DocMan.Domain.Enums;
using MediatR;

namespace DocMan.Application.Documents.Queries;

public record SearchDocumentsQuery : IRequest<Result<PagedResult<DocumentListItemDto>>>
{
    public string? SearchTerm { get; init; }
    public DocumentType? Type { get; init; }
    public string? UploadedBy { get; init; }
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
    public string? Tag { get; init; }
    public string SortBy { get; init; } = "CreatedAt";
    public bool SortDescending { get; init; } = true;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
