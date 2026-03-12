using DocMan.Application.Common;
using DocMan.Application.Documents.DTOs;
using MediatR;

namespace DocMan.Application.Documents.Queries;

public class GetDocumentByIdQueryHandler
    : IRequestHandler<GetDocumentByIdQuery, Result<DocumentDto>>
{
    private readonly IDocumentRepository _repository;

    public GetDocumentByIdQueryHandler(IDocumentRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<DocumentDto>> Handle(
        GetDocumentByIdQuery request, CancellationToken cancellationToken)
    {
        var document = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (document is null)
            return Result<DocumentDto>.Failure("Doküman bulunamadı.");

        return Result<DocumentDto>.Success(document.ToDto());
    }
}
