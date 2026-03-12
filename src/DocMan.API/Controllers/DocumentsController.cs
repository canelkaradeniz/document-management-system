using DocMan.Application.Common;
using DocMan.Application.Documents.Commands;
using DocMan.Application.Documents.DTOs;
using DocMan.Application.Documents.Queries;
using DocMan.Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DocMan.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IValidator<UploadDocumentCommand> _uploadValidator;

    public DocumentsController(IMediator mediator, IValidator<UploadDocumentCommand> uploadValidator)
    {
        _mediator = mediator;
        _uploadValidator = uploadValidator;
    }

    /// <summary>
    /// Dokümanları arar ve listeler. Filtreleme, sıralama ve sayfalama destekler.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<Result<PagedResult<DocumentListItemDto>>>> Search(
        [FromQuery] string? search,
        [FromQuery] DocumentType? type,
        [FromQuery] string? uploadedBy,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] string? tag,
        [FromQuery] string sortBy = "CreatedAt",
        [FromQuery] bool sortDesc = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var query = new SearchDocumentsQuery
        {
            SearchTerm = search,
            Type = type,
            UploadedBy = uploadedBy,
            DateFrom = dateFrom,
            DateTo = dateTo,
            Tag = tag,
            SortBy = sortBy,
            SortDescending = sortDesc,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Belirli bir dokümanın detaylarını getirir.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Result<DocumentDto>>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetDocumentByIdQuery(id), ct);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Yeni doküman yükler. Duplicate kontrolü yapar ve kullanıcıyı bilgilendirir.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Result<DocumentDto>>> Upload(
        [FromBody] UploadDocumentCommand command, CancellationToken ct)
    {
        var validation = await _uploadValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
        {
            var errors = string.Join(" ", validation.Errors.Select(e => e.ErrorMessage));
            return BadRequest(Result<DocumentDto>.Failure(errors));
        }

        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    /// <summary>
    /// Doküman tipleri listesi (frontend dropdown için).
    /// </summary>
    [HttpGet("types")]
    public ActionResult GetDocumentTypes()
    {
        var types = Enum.GetValues<DocumentType>()
            .Select(t => new { Value = (int)t, Name = t.ToString(), DisplayName = t.ToDisplayName() });

        return Ok(types);
    }

    /// <summary>
    /// Duplicate check endpoint - yükleme öncesi kontrol için.
    /// </summary>
    [HttpGet("check-duplicate")]
    public async Task<ActionResult<DuplicateCheckResult>> CheckDuplicate(
        [FromQuery] string contentHash, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(contentHash))
            return BadRequest(new DuplicateCheckResult(false, null, null, "Content hash gereklidir."));

        var repo = HttpContext.RequestServices.GetRequiredService<IDocumentRepository>();
        var existing = await repo.GetByContentHashAsync(contentHash, ct);

        if (existing is not null)
        {
            return Ok(new DuplicateCheckResult(
                true, existing.Id, existing.Name,
                $"Bu dosya zaten sistemde mevcut: '{existing.Name}'"));
        }

        return Ok(new DuplicateCheckResult(false, null, null, "Duplicate bulunamadı."));
    }
}
