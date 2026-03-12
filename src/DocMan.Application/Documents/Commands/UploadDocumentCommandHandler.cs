using DocMan.Application.Common;
using DocMan.Application.Documents.DTOs;
using DocMan.Domain.Entities;
using MediatR;

namespace DocMan.Application.Documents.Commands;

public class UploadDocumentCommandHandler
    : IRequestHandler<UploadDocumentCommand, Result<DocumentDto>>
{
    private readonly IDocumentRepository _repository;

    public UploadDocumentCommandHandler(IDocumentRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<DocumentDto>> Handle(
        UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        // Duplicate check: exact content hash match
        var existingByHash = await _repository.GetByContentHashAsync(
            request.ContentHash, cancellationToken);

        if (existingByHash is not null && !request.ForceUpload)
        {
            var warnings = new List<string>
            {
                $"Bu dosyanın aynısı zaten sistemde mevcut: '{existingByHash.Name}' (ID: {existingByHash.Id}). " +
                "Yine de yüklemek istiyorsanız 'forceUpload: true' gönderin."
            };
            return Result<DocumentDto>.Success(existingByHash.ToDto(), warnings);
        }

        // Similar name check
        var warnings2 = new List<string>();
        var similarDocs = await _repository.GetSimilarByNameAsync(
            request.Name, request.Type, cancellationToken);

        if (similarDocs.Count > 0)
        {
            var names = string.Join(", ", similarDocs.Select(d => $"'{d.Name}'"));
            warnings2.Add($"Benzer isimde dokümanlar bulundu: {names}. Yinelenen yükleme olmadığından emin olun.");
        }

        var document = new Document
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Type = request.Type,
            FileExtension = request.FileExtension,
            FileSizeBytes = request.FileSizeBytes,
            ContentHash = request.ContentHash,
            UploadedBy = request.UploadedBy,
            CreatedAt = DateTime.UtcNow,
            Tags = request.Tags
        };

        await _repository.AddAsync(document, cancellationToken);

        return Result<DocumentDto>.Success(document.ToDto(), warnings2);
    }
}
