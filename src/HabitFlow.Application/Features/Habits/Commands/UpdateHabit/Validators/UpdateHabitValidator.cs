using System;
using FluentValidation;
using HabitFlow.Application.Features.Habits.Commands.UpdateHabit.Dtos;

namespace HabitFlow.Application.Features.Habits.Commands.UpdateHabit.Validators;

public class UpdateHabitValidator : AbstractValidator<UpdateHabitDto>
{
    public UpdateHabitValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres.");

        RuleFor(x => x.Frequency)
            .NotEmpty().WithMessage("Frequência é obrigatória.")
            .MaximumLength(50).WithMessage("Frequência deve ter no máximo 50 caracteres.");

        RuleFor(x => x.Target)
            .NotEmpty().WithMessage("Meta é obrigatória.")
            .MaximumLength(100).WithMessage("Meta deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Color)
            .MaximumLength(20).WithMessage("Cor deve ter no máximo 20 caracteres.")
            .Matches("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$").WithMessage("Cor deve ser um valor hexadecimal válido.")
            .When(x => !string.IsNullOrEmpty(x.Color));
    }
}