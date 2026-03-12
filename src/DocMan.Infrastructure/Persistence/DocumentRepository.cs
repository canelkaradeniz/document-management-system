using DocMan.Application.Common;
using DocMan.Domain.Entities;
using DocMan.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DocMan.Infrastructure.Persistence;

public class DocumentRepository : IDocumentRepository
{
    private readonly AppDbContext _context;

    public DocumentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Document?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Documents.FindAsync([id], ct);
    }

    public async Task<Document?> GetByContentHashAsync(string contentHash, CancellationToken ct = default)
    {
        return await _context.Documents
            .FirstOrDefaultAsync(d => d.ContentHash == contentHash, ct);
    }

    public async Task<(List<Document> Items, int TotalCount)> SearchAsync(
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
        CancellationToken ct = default)
    {
        var query = _context.Documents.AsQueryable();

        // Full-text-like search on name and description
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(d =>
                d.Name.ToLower().Contains(term) ||
                (d.Description != null && d.Description.ToLower().Contains(term)));
        }

        if (type.HasValue)
            query = query.Where(d => d.Type == type.Value);

        if (!string.IsNullOrWhiteSpace(uploadedBy))
            query = query.Where(d => d.UploadedBy == uploadedBy);

        if (dateFrom.HasValue)
            query = query.Where(d => d.CreatedAt >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(d => d.CreatedAt <= dateTo.Value);

        if (!string.IsNullOrWhiteSpace(tag))
        {
            // SQLite JSON search - tags stored as JSON array
            query = query.Where(d => d.Tags.Contains(tag));
        }

        // Sorting
        query = sortBy.ToLower() switch
        {
            "name" => sortDescending ? query.OrderByDescending(d => d.Name) : query.OrderBy(d => d.Name),
            "type" => sortDescending ? query.OrderByDescending(d => d.Type) : query.OrderBy(d => d.Type),
            "uploadedby" => sortDescending ? query.OrderByDescending(d => d.UploadedBy) : query.OrderBy(d => d.UploadedBy),
            "filesize" => sortDescending ? query.OrderByDescending(d => d.FileSizeBytes) : query.OrderBy(d => d.FileSizeBytes),
            _ => sortDescending ? query.OrderByDescending(d => d.CreatedAt) : query.OrderBy(d => d.CreatedAt)
        };

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<Document> AddAsync(Document document, CancellationToken ct = default)
    {
        _context.Documents.Add(document);
        await _context.SaveChangesAsync(ct);
        return document;
    }

    public async Task<List<Document>> GetSimilarByNameAsync(
        string name, DocumentType type, CancellationToken ct = default)
    {
        var nameLower = name.ToLower();
        // Find documents with similar names of the same type
        return await _context.Documents
            .Where(d => d.Type == type && d.Name.ToLower().Contains(nameLower))
            .Take(5)
            .ToListAsync(ct);
    }
}
