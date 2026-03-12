using DocMan.Application.Common;
using DocMan.Application.Documents.DTOs;
using MediatR;

namespace DocMan.Application.Documents.Queries;

public record GetDocumentByIdQuery(Guid Id) : IRequest<Result<DocumentDto>>;
