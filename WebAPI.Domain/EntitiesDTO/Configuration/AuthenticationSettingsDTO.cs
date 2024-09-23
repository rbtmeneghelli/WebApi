﻿using System.ComponentModel.DataAnnotations;

namespace WebAPI.Domain.EntitiesDTO.Configuration;

public record AuthenticationSettingsResponseDTO
{
    [Required]
    [Display(Name = "NumberOfTryToBlockUser", Description = "Número de tentativas antes do bloqueio do usuário")]
    public string EnvironmentDescription { get; set; }

    [Required]
    [Display(Name = "NumberOfTryToBlockUser", Description = "Número de tentativas antes do bloqueio do usuário")]
    public int NumberOfTryToBlockUser { get; set; }

    [Required]
    [Display(Name = "BlockUserTime", Description = "Duração do bloqueio em minutos")]
    public int BlockUserTime { get; set; }

    [Required]
    [Display(Name = "ApplyTwoFactoryValidation", Description = "Aplicar validação de dois fatores")]
    public bool ApplyTwoFactoryValidation { get; set; }

    [Required]
    [Display(Name = "Status", Description = "Status do ambiente")]
    public string StatusDescription { get; set; }
}

public record AuthenticationSettingsRequestDTO
{
    [Required]
    [Display(Name = "NumberOfTryToBlockUser", Description = "Número de tentativas antes do bloqueio do usuário")]
    [Range(1, 10, ErrorMessage = "O campo {0} deve ser preenchido com valor de {1} até {2}")]
    public int NumberOfTryToBlockUser { get; set; }

    [Required]
    [Display(Name = "BlockUserTime", Description = "Duração do bloqueio em minutos")]
    [Range(1, 120, ErrorMessage = "O campo {0} deve ser preenchido com valor de {1} até {2}")]
    public int BlockUserTime { get; set; }

    [Required]
    [Display(Name = "ApplyTwoFactoryValidation", Description = "Aplicar validação de dois fatores")]
    public bool ApplyTwoFactoryValidation { get; set; }
}
