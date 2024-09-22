﻿using WebAPI.Domain.ExtensionMethods;
using Microsoft.AspNetCore.Cors;
using WebAPI.Domain.Enums;
using WebAPI.IoC.Middleware.Security;
using WebAPI.Domain.Interfaces.Services.Tools;
using WebAPI.Domain.Interfaces.Services.Configuration;
using WebAPI.IoC.ActionFilter;
using WebAPI.Domain.Interfaces.Repository;
using WebAPI.Domain.Interfaces.Services;

namespace WebAPI.V1.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[AllowAnonymous]
public sealed class GeneralController : GenericController
{
    private readonly IGeneralService _iGeneralService;
    private readonly IQRCodeService _iQRCodeService;
    private readonly IFirebaseService _iFirebaseService;
    private readonly IEmailService _iEmailService;
    private readonly GeneralMethod _generalMethod;

    private EnvironmentVariables _environmentVariables { get; }

    public GeneralController(
        IGeneralService iGeneralService,                            
        IQRCodeService iQRCodeService, 
        IFirebaseService iFirebaseService, 
        IEmailService iEmailService,
        EnvironmentVariables environmentVariables,
        IMapper iMapperService,
        IHttpContextAccessor iHttpContextAccessor,
        IGenericNotifyLogsService iGenericNotifyLogsService) 
        : base(iMapperService, iHttpContextAccessor, iGenericNotifyLogsService)
    {
        _iGeneralService = iGeneralService;
        _iQRCodeService = iQRCodeService;
        _iFirebaseService = iFirebaseService;
        _iEmailService = iEmailService;
        _environmentVariables = environmentVariables;
        _generalMethod = GeneralMethod.GetLoadExtensionMethods();
    }

    [HttpGet("export2Zip/{directory}/{typeFile:int?}")]
    public async Task<IActionResult> Export2Zip(string directory, int? typeFile)
    {
        EnumMemoryStreamFile enumtypeFile = typeFile.HasValue ? (EnumMemoryStreamFile)typeFile : EnumMemoryStreamFile.PDF;
        MemoryStream memoryStream = await _iGeneralService.Export2ZipAsync(directory, enumtypeFile);
        var memoryStreamResult = _generalMethod.GetMemoryStreamType(enumtypeFile);
        return File(await Task.FromResult(memoryStream.ToArray()), memoryStreamResult.Type, $"Archive.{memoryStreamResult.Extension}");
    }

    [HttpGet("backup/{directory}")]
    public async Task<IActionResult> Backup(string directory)
    {
        var result = await _iGeneralService.RunSqlBackupAsync(directory);

        if (result)
            return CustomResponse(null, "Backup executado com sucesso");

        return CustomResponse();
    }

    [HttpPost("createQRCode")]
    public IActionResult CreateQRCode([FromBody] QRCodeFile qRCodeFile)
    {
        var memoryStreamResult = _generalMethod.GetMemoryStreamType(EnumMemoryStreamFile.PNG);
        var image = _iQRCodeService.CreateQRCode(qRCodeFile);
        return File(image, memoryStreamResult.Type);
    }

    [HttpPost("readQRCode")]
    public IActionResult ReadQRCode(IFormFile qrCodeFile)
    {
        if (GuardClauses.ObjectIsNull(qrCodeFile) || qrCodeFile.Length == 0)
        {
            NotificationError("Nenhum arquivo foi enviado.");
            return CustomResponse();
        }

        return CustomResponse(_iQRCodeService.ReadQrCode(qrCodeFile));
    }

    /// <summary>
    /// Esse endpoint irá funcionar apenas se o IP listado não estiver bloqueado para seu uso
    /// </summary>
    /// <returns></returns>
    [ServiceFilter(typeof(IPBlockActionFilter))]
    [HttpGet("bloqueado")]
    public string Bloqueado()
    {
        var teste = _environmentVariables;
        return $"Acesso BLOQUEADO para este endpoint : {DateOnlyExtensionMethods.GetDateTimeNowFromBrazil()}";
    }

    [HttpGet("loadEnvironmentVariables")]
    public IActionResult LoadEnvironmentVariables()
    {
        try
        {
            return CustomResponse(_environmentVariables.ConnectionStringSettings.SerializeObject(), "Variaveis de ambiente");
        }
        catch (Exception ex)
        {
            NotificationError("Ocorreu um erro durante a leitura das var de ambiente");
            return CustomResponse();
        }
    }

    [HttpGet("SendPushNotification")]
    public async Task<IActionResult> SendPushNotification()
    {
        await _iFirebaseService.SendPushNotification_V2("Xpto", "isso e um teste");
        return CustomResponse();
    }

    [EnableCors("EnableCORS")]
    [HttpPost("refreshEnvironmentVariables")]
    public IActionResult RefreshEnvironmentVariables([FromBody] EnvironmentVarSettings environmentVarSettings)
    {
        try
        {
            if (_environmentVariables is null)
            {
                NotificationError("Ocorreu um erro durante o processo de atualização das var de ambiente");
                return CustomResponse();
            }

#if DEBUG
            _iGeneralService.RefreshEnvironmentVarLocal(environmentVarSettings);
#else
                _iGeneralService.RefreshEnvironmentVarAzure(environmentVarSettings);
#endif
        }
        catch (Exception ex)
        {
            NotificationError("Ocorreu um erro durante o processo de atualização das var de ambiente");
        }

        return CustomResponse();
    }

    [HttpGet("testSendEmail")]
    [SecurityFilter(1,1)]
    public async Task<IActionResult> TestSendEmail()
    {
        await _iEmailService.CustomSendEmailAsync(EnumEmail.Welcome, "teste@gmail.com", "XPTO");
        return CustomResponse();
    }
}
