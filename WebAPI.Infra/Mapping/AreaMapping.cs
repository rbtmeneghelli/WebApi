﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebAPI.Infra.Generic;
using WebAPI.Domain.Entities.ControlPanel;

namespace WebAPI.Infra.Data.Mapping;

public class AreaMapping : GenericMapping<Area>
{
    public override void Configure(EntityTypeBuilder<Area> builder)
    {
        _builder = builder;
        base.ConfigureDefaultColumns();
        _builder.ToTable("Areas");
        ConfigureColumns();
    }

    private void ConfigureColumns()
    {
        _builder.Property(x => x.Description).IsRequired().HasMaxLength(255).HasColumnName("Description");
        _builder.Property(x => x.HierarchyLevel).IsRequired().HasConversion<string>().HasColumnName("HierarchyLevel");
        _builder.Property(x => x.Order).IsRequired().HasColumnName("Order");
        _builder.HasMany(x => x.Profiles).WithOne(x => x.Area).HasForeignKey(x => x.IdArea).OnDelete(DeleteBehavior.NoAction);
    }
}