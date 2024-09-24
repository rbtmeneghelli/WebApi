﻿using WebAPI.Domain.Constants;
using WebAPI.Domain.Entities.Configuration;
using WebAPI.Domain.EntitiesDTO.Configuration;
using WebAPI.Domain.Enums;
using WebAPI.Domain.ExtensionMethods;
using WebAPI.Domain.Interfaces.Repository;
using WebAPI.Domain.Interfaces.Services.Tools;

namespace WebAPI.Controllers.V1.Configuration;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize("Bearer")]
public sealed class AuthenticationSettingsController : GenericController
{
    private readonly IGenericConfigurationService _iGenericConfigurationService;
    private readonly GeneralMethod _generalMethod;
    private readonly IFileService<AuthenticationSettingsExcelDTO> _iFileService;

    public AuthenticationSettingsController(
        IGenericConfigurationService iGenericConfigurationService,
        IFileService<AuthenticationSettingsExcelDTO> iFileService,
        IMapper iMapperService,
        IHttpContextAccessor iHttpContextAccessor,
        IGenericNotifyLogsService iGenericNotifyLogsService)
    : base(iMapperService, iHttpContextAccessor, iGenericNotifyLogsService)
    {
        _iGenericConfigurationService = iGenericConfigurationService;
        _generalMethod = GeneralMethod.GetLoadExtensionMethods();
        _iFileService = iFileService;
    }

    //private readonly IMemoryCacheService _memoryCacheService;

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var model = await _iGenericConfigurationService.AuthenticationSettingsService.GetAllAuthenticationSettingsAsync();
        return CustomResponse(model, FixConstants.SUCCESS_IN_GETALL);
    }

    [HttpGet("GetByEnvironment")]
    public async Task<IActionResult> GetByEnvironment()
    {
        var existAuthenticationSettings = await _iGenericConfigurationService.AuthenticationSettingsService.ExistAuthenticationSettingsByEnvironmentAsync();
        if (existAuthenticationSettings)
        {
            var model = await _iGenericConfigurationService.AuthenticationSettingsService.GetAuthenticationSettingsByEnvironmentAsync();
            return CustomResponse(model, FixConstants.SUCCESS_IN_GETID);
        }

        return CustomNotFound();
    }

    [HttpGet("GetById/{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var existAuthenticationSettings = await _iGenericConfigurationService.AuthenticationSettingsService.ExistAuthenticationSettingsByIdAsync(id);
        if (existAuthenticationSettings)
        {
            var model = await _iGenericConfigurationService.AuthenticationSettingsService.GetAuthenticationSettingsByIdAsync(id);
            return CustomResponse(model, FixConstants.SUCCESS_IN_GETID);
        }

        return CustomNotFound();
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] AuthenticationSettingsCreateRequestDTO authenticationSettingsCreateRequestDTO)
    {
        if (ModelStateIsInvalid()) return CustomResponse(ModelState);

        var authenticationSettingsRequest = ApplyMapToEntity<AuthenticationSettingsCreateRequestDTO, AuthenticationSettings>(authenticationSettingsCreateRequestDTO);
        var result = await _iGenericConfigurationService.AuthenticationSettingsService.CreateAuthenticationSettingsAsync(authenticationSettingsRequest);

        if (result)
            return CreatedAtAction(nameof(Create), authenticationSettingsRequest);

        return CustomResponse();
    }

    [HttpPut("Update")]
    public async Task<IActionResult> Update(int id, [FromBody] AuthenticationSettingsUpdateRequestDTO authenticationSettingsUpdateRequestDTO)
    {
        if (ModelStateIsInvalid()) return CustomResponse(ModelState);

        var authenticationSettingsRequest = ApplyMapToEntity<AuthenticationSettingsUpdateRequestDTO, AuthenticationSettings>(authenticationSettingsUpdateRequestDTO);

        if (id != authenticationSettingsRequest.Id)
        {
            NotificationError(FixConstants.ERROR_IN_GETID);
            return CustomResponse();
        }

        if (await _iGenericConfigurationService.AuthenticationSettingsService.ExistAuthenticationSettingsByIdAsync(id))
        {
            var result = await _iGenericConfigurationService.AuthenticationSettingsService.UpdateAuthenticationSettingsAsync(id, authenticationSettingsRequest);
            if (result)
                return NoContent();
            else
                return CustomResponse();
        }

        return CustomNotFound();
    }

    [HttpDelete("LogicDelete/{id:long}")]
    public async Task<IActionResult> LogicDelete(int id)
    {
        if (await _iGenericConfigurationService.AuthenticationSettingsService.ExistAuthenticationSettingsByIdAsync(id))
        {
            bool result = await _iGenericConfigurationService.AuthenticationSettingsService.LogicDeleteAuthenticationSettingsByIdAsync(id);
            if (result)
                return CustomResponse(default, FixConstants.SUCCESS_IN_DELETELOGIC);
            else
                return CustomResponse();
        }

        return CustomNotFound();
    }

    [HttpPost("Reactive")]
    public async Task<IActionResult> Reactive(AuthenticationSettingsReactiveRequestDTO authenticationSettingsReactiveRequestDTO)
    {
        if (await _iGenericConfigurationService.AuthenticationSettingsService.ExistAuthenticationSettingsByIdAsync(authenticationSettingsReactiveRequestDTO.Id.GetValueOrDefault()))
        {
            bool result = await _iGenericConfigurationService.AuthenticationSettingsService.ReactiveAuthenticationSettingsByIdAsync(authenticationSettingsReactiveRequestDTO.Id.Value);
            if (result)
                return CustomResponse(default, FixConstants.SUCCESS_IN_ACTIVERECORD);
            else
                return CustomResponse();
        }

        return CustomNotFound();
    }

    [HttpPost("Export2Excel")]
    public async Task<IActionResult> Export2Excel()
    {
        if (ModelStateIsInvalid()) return CustomResponse(ModelState);

        var list = await _iGenericConfigurationService.AuthenticationSettingsService.GetAllAuthenticationSettingsAsync();
        if (list?.Count() > 0)
        {
            var memoryStreamResult = _generalMethod.GetMemoryStreamType(EnumMemoryStreamFile.XLSX);
            var excelData = ApplyMapToEntity<IEnumerable<AuthenticationSettingsResponseDTO>, IEnumerable<AuthenticationSettingsExcelDTO>>(list);
            var excelName = $"AuthenticationSettings_{GuidExtensionMethod.GetGuidDigits("N")}.{memoryStreamResult.Extension}";
            var memoryStreamExcel = await _iFileService.CreateExcelFileEPPLUS(excelData, excelName);
            return File(memoryStreamExcel.ToArray(), memoryStreamResult.Type, excelName);
        }

        return CustomNotFound();
    }

    /// <summary>
    /// Esse endpoint ira armazenar arquivos por X tempo, e depois será atualizado após 5 minutos.
    /// </summary>
    /// <returns></returns>
    //[HttpGet("loadBanners")]
    //public async Task<IActionResult> LoadBanners()
    //{
    //    if (!_memoryCacheService.TryGet<IEnumerable<Region>>("FilesCache", out var cached))
    //    {
    //        var files = await _regionService.GetAllRegionAsync();

    //        _memoryCacheService.Set("FilesCache", files);

    //        return CustomResponse(files);
    //    }
    //    else
    //    {
    //        var files = _memoryCacheService.Get<IEnumerable<Region>>("FilesCache");
    //        return CustomResponse(files);
    //    }
    //}

    //
    //[HttpPost("uploadMultFiles")]
    //public IActionResult UploadFiles([FromForm] IEnumerable<MultFiles> multFiles)
    //{
    //    if (multFiles is null)
    //    {
    //        NotificationError("Nenhum arquivo foi enviado.");
    //        return CustomResponse();
    //    }

    //    return CustomResponse();
    //}
}