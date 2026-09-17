using FluentValidation;
using Portfolio_game_dev.Areas.Admin.ViewModels;

namespace Portfolio_game_dev.Validators;

public class SkillEditViewModelValidator : AbstractValidator<SkillEditViewModel> {
    public SkillEditViewModelValidator() {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название обязательно")
            .MaximumLength(100);

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Укажите категорию")
            .MaximumLength(50);

        RuleFor(x => x.Level)
            .InclusiveBetween(1, 5);

        RuleFor(x => x.YearsOfExperience)
            .InclusiveBetween(0, 50);

        RuleFor(x => x.IconUrl)
            .Must(BeAValidUrlOrEmpty).WithMessage("Некорректный URL иконки");
    }

    private static bool BeAValidUrlOrEmpty(string? url)
        => string.IsNullOrWhiteSpace(url)
           || Uri.TryCreate(url, UriKind.Absolute, out _);
}