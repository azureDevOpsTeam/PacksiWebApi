using ApplicationLayer.DTOs.Comments;
using FluentValidation;

namespace ApplicationLayer.Validators.Comments;

public class CreateCommentValidator : AbstractValidator<CreateCommentDto>
{
    public CreateCommentValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("متن نظر الزامی است")
            .MaximumLength(1000);
    }
}