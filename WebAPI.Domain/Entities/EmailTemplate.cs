﻿using WebAPI.Domain.Generic;

namespace WebAPI.Domain.Entities;

public class EmailTemplate : GenericEntity
{
    public string Description { get; set; }
    public virtual List<EmailDisplay> EmailDisplays { get; set; }
}
