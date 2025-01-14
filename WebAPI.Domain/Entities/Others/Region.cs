﻿using System.Diagnostics;
using System.Text.Json.Serialization;
using WebAPI.Domain.Entities.Generic;
using WebAPI.Domain.DTO.Others;

namespace WebAPI.Domain.Entities.Others;

[DebuggerDisplay("Id: {Id}, Nome: {Name}, Sigla: {Initials}")]
public class Region : GenericEntity
{
    [JsonPropertyName("Nome")]
    public string Name { get; set; }
    [JsonPropertyName("Sigla")]
    public string Initials { get; set; }
    public IEnumerable<States> States { get; set; }

    /// <summary>
    /// Faz a conversão implicita do objeto Region para RegionDto, ao passar a entidade
    /// O explicit você precisa efetuar um cast antes de mandar o parametro
    /// </summary>
    /// <param name="region"></param>
    public static implicit operator RegionResponseDTO(Region region)
    {
        return new()
        {
            Name = region.Name,
            Initials = region.Initials
        };
    }

    public static implicit operator string(Region region)
    {
        return $"Nome da região {region.Name}, Iniciais da região {region.Initials}";
    }
}
