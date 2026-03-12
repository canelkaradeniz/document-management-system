using DocMan.Domain.Entities;
using DocMan.Domain.Enums;

namespace DocMan.Application.Common;

public interface IDocumentRepository
{
    Task<Document?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Document?> GetByContentHashAsync(string contentHash, CancellationToken ct = default);
    Task<(List<Document> Items, int TotalCount)> SearchAsync(
        string? searchTerm,
        DocumentType? type,
        string? uploadedBy,
        DateTime? dateFrom,
        DateTime? dateTo,
        string? tag,
        string sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken ct = default);
    Task<Document> AddAsync(Document document, CancellationToken ct = default);
    Task<List<Document>> GetSimilarByNameAsync(string name, DocumentType type, CancellationToken ct = default);
}
