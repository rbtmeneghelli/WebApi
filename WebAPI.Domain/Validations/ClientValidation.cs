﻿using FluentValidation;
using WebAPI.Domain.Entities.ControlPanel;
using WebAPI.Domain.ExtensionMethods;

namespace WebAPI.Domain.Validations;

public class ClientValidation : AbstractValidator<Client>
{
    public ClientValidation()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("O valor '{PropertyValue}' para '{PropertyName}' é inválido.");

        RuleFor(x => x.ClientName)
            .NotEmpty()
            .WithMessage("O '{PropertyName}' não pode ficar em branco.")
            .MinimumLength(10)
            .WithMessage("O '{PropertyName}' deve ter no mínimo {MinLength} caracteres.")
            .MaximumLength(80)
            .WithMessage("O '{PropertyName}' deve ter no máximo {MaxLength} caracteres.");

        When(client => client.ClientDocument is not null, () =>
        {
            RuleFor(x => x.ClientDocument.BirthDate)
            .NotEmpty()
            .WithMessage("O '{PropertyName}' não pode ficar em branco.")
            .LessThan(DateOnlyExtensionMethods.GetDateTimeNowFromBrazil())
            .WithMessage("A data de nascimento não pode ser futura!")
            .Must(DateOnlyExtensionMethods.IsAdultPerson)
            .WithMessage("A data de nascimento é de uma pessoa menor de 18 anos! para prosseguir insira uma data de nascimento de 18 anos ou mais.");

            RuleFor(x => x.ClientDocument.Age)
            .InclusiveBetween(1, 110)
            .WithMessage("A '{PropertyName}' deve estar entre 1 ou 110");
        });
    }
}


