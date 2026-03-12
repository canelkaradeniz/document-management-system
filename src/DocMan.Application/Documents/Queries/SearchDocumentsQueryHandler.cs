using DocMan.Application.Common;
using DocMan.Application.Documents.DTOs;
using MediatR;

namespace DocMan.Application.Documents.Queries;

public class SearchDocumentsQueryHandler
    : IRequestHandler<SearchDocumentsQuery, Result<PagedResult<DocumentListItemDto>>>
{
    private readonly IDocumentRepository _repository;

    public SearchDocumentsQueryHandler(IDocumentRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PagedResult<DocumentListItemDto>>> Handle(
        SearchDocumentsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repository.SearchAsync(
            request.SearchTerm,
            request.Type,
            request.UploadedBy,
            request.DateFrom,
            request.DateTo,
            request.Tag,
            request.SortBy,
            request.SortDescending,
            request.Page,
            request.PageSize,
            cancellationToken);

        var dtos = items.Select(d => d.ToListItemDto()).ToList();

        var pagedResult = new PagedResult<DocumentListItemDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return Result<PagedResult<DocumentListItemDto>>.Success(pagedResult);
    }
}
