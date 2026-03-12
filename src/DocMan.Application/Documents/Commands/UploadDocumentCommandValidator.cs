using FluentValidation;

namespace DocMan.Application.Documents.Commands;

public class UploadDocumentCommandValidator : AbstractValidator<UploadDocumentCommand>
{
    public UploadDocumentCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Doküman adı zorunludur.")
            .MaximumLength(250).WithMessage("Doküman adı en fazla 250 karakter olabilir.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Geçersiz doküman tipi.");

        RuleFor(x => x.FileExtension)
            .NotEmpty().WithMessage("Dosya uzantısı zorunludur.");

        RuleFor(x => x.FileSizeBytes)
            .GreaterThan(0).WithMessage("Dosya boyutu geçerli olmalıdır.");

        RuleFor(x => x.ContentHash)
            .NotEmpty().WithMessage("İçerik hash değeri zorunludur.");

        RuleFor(x => x.UploadedBy)
            .NotEmpty().WithMessage("Yükleyen kullanıcı bilgisi zorunludur.");
    }
}
