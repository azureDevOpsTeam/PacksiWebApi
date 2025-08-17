using ApplicationLayer.DTOs.Identity;
using FluentValidation;

public class SignUpDtoValidator : AbstractValidator<SignUpDto>
{
    public SignUpDtoValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("وارد کردن نام نمایشی الزامی است.");

        RuleFor(x => x.PhonePrefix)
            .NotEmpty().WithMessage("وارد کردن پیش شماره الزامی است.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("وارد کردن شماره تلفن الزامی است.")
            .Matches(@"^\d{8,15}$").WithMessage("شماره تلفن باید فقط شامل اعداد و بین 8 تا 15 رقم باشد.")
            .Must(phone => !phone.StartsWith("0"))
            .WithMessage("شماره تلفن نباید با صفر شروع شود.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("رمز عبور الزامی است.")
            .MinimumLength(8).WithMessage("رمز عبور باید حداقل 8 کاراکتر باشد.")
            .Matches("[A-Z]").WithMessage("رمز عبور باید حداقل یک حرف بزرگ داشته باشد.")
            .Matches("[a-z]").WithMessage("رمز عبور باید حداقل یک حرف کوچک داشته باشد.")
            .Matches("[0-9]").WithMessage("رمز عبور باید حداقل یک عدد داشته باشد.")
            .Matches("[^a-zA-Z0-9]").WithMessage("رمز عبور باید حداقل یک کاراکتر خاص داشته باشد.");
    }
}