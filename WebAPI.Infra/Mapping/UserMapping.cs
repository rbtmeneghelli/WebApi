﻿using WebAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebAPI.Infra.Generic;

namespace WebAPI.Infra.Data.Mapping;

public class UserMapping : GenericMapping<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        _builder = builder;
        base.ConfigureDefaultColumns();
        //Exemplo de criação de tabela Temporal (Historico) que trabalha como uma trilha de auditoria
        _builder.ToTable("Users", e => e.IsTemporal(t =>
        {
            t.HasPeriodStart("InicioValidade");
            t.HasPeriodEnd("TerminoValidade");
            t.UseHistoryTable("UsersHistory");
        }));
        ConfigureColumns();
        ConfigureForeignKeys();
        ConfigureIndexes();
    }

    private void ConfigureForeignKeys()
    {
        _builder.HasOne(x => x.Profile).WithMany(x => x.Users).HasForeignKey(x => x.IdProfile);
    }

    private void ConfigureIndexes()
    {
        // Indice exclusivo >> Permite que a coluna da tabela impede que valores duplicados existam
        _builder.HasIndex(a => a.IdProfile).IsUnique(false);
        // Indice não exclusivo >> Permite que a coluna da tabela tenha valores duplicados, porém é apresentado uma melhora nas consultas
        //_builder.HasIndex(a => a.IdProfile);
    }

    private void ConfigureColumns()
    {
        _builder.Property(x => x.Login).IsRequired(true).HasMaxLength(120).HasColumnName("Login");
        _builder.Property(x => x.Password).IsRequired(true).HasMaxLength(255).HasColumnName("Password");
        _builder.Property(x => x.LastPassword).IsRequired(false).HasMaxLength(255).HasColumnName("Last_Password");
        _builder.Property(x => x.IsAuthenticated).IsRequired(true).HasDefaultValue(false).HasColumnName("Is_Authenticated");
    }
}
