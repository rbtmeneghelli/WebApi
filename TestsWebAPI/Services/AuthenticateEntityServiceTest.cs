﻿namespace DefaultWebApiTest.Services;

public class AuthenticateEntityServiceTest : IAuthenticateEntityServiceTest
{
    private readonly IAuthenticateEntityRepositoryTest _authenticateEntityRepository;

    public AuthenticateEntityServiceTest(IAuthenticateEntityRepositoryTest authenticateEntityRepository)
    {
        _authenticateEntityRepository = authenticateEntityRepository;
    }

    private AuthenticateEntity GetDefaultAuthenticate()
    {
        return new AuthenticateEntity()
        {
            Token = string.Empty,
            Data = Constants.GetDateTimeNowFromBrazil(),
            HoraInicial = Constants.GetDateTimeNowFromBrazil().TimeOfDay,
            HoraFinal = Constants.GetDateTimeNowFromBrazil().TimeOfDay
        };
    }

    public bool ExistAuthentication()
    {
        return _authenticateEntityRepository.ExistAnyRecord();
    }

    public void InsertDefaultAuthenticate()
    {
        _authenticateEntityRepository.Insert(GetDefaultAuthenticate());
    }

    public bool IsValidAuthentication(AuthenticateEntity authenticateEntity)
    {
        DateTime currentDateTime = DateTime.Now;

        if (authenticateEntity.Data.HasValue && authenticateEntity.HoraInicial.HasValue && authenticateEntity.HoraFinal.HasValue)
            return currentDateTime.Date == authenticateEntity.Data.Value.Date &&
                   currentDateTime.TimeOfDay >= authenticateEntity.HoraInicial &&
                   currentDateTime.TimeOfDay <= authenticateEntity.HoraFinal;

        return false;
    }

    public AuthenticateEntity GetAuthenticate(long authId = 1L)
    {
        return _authenticateEntityRepository.GetById(authId);
    }

    public void InsertAuthenticate(AuthenticateEntity authenticateEntity)
    {
        _authenticateEntityRepository.Insert(authenticateEntity);
    }

    public void UpdateAuthenticate(AuthenticateEntity authenticateEntity)
    {
        _authenticateEntityRepository.Update(authenticateEntity);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
