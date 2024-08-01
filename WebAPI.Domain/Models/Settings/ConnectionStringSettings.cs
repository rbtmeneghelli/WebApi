﻿namespace WebAPI.Domain.Models;

public sealed record ConectionStringSettings
{
    public string DefaultConnection { get; set; }
    public string DefaultConnectionLogs { get; set; }
    public string DefaultConnectionToDocker { get; set; }
    public string DefaultConnectionToMongoDb { get; set; }
}

