﻿using WebAPI.Domain.Entities.Configuration;
using WebAPI.Domain.EntitiesDTO.Configuration;

namespace WebAPI.Domain.Interfaces.Services.Configuration;

public interface ILogSettingsService
{
    Task<IEnumerable<LogSettingsResponseDTO>> GetAllLogSettingsAsync();
    Task<LogSettingsResponseDTO> GetLogSettingsByEnvironmentAsync();
    Task<LogSettingsResponseDTO> GetLogSettingsByIdAsync(long id);
    Task<bool> ExistLogSettingsByEnvironmentAsync();
    Task<bool> ExistLogSettingsByIdAsync(long id);
    Task<bool> CreateLogSettingsAsync(LogSettings logSettings);
    Task<bool> UpdateLogSettingsAsync(long id, LogSettings logSettings);
    Task<bool> LogicDeleteLogSettingsByIdAsync(long id);
    Task<bool> ReactiveLogSettingsByIdAsync(long id);
    Task<IEnumerable<LogSettingsExcelDTO>> GetAllLogSettingsExcelAsync();
}
