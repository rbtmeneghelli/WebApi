﻿using WebAPI.Domain.CQRS.Command;
using WebAPI.Domain.CQRS.Queries;
using WebAPI.Domain.Entities.Configuration;
using WebAPI.Domain.Entities.ControlPanel;
using WebAPI.Domain.Entities.Others;
using WebAPI.Domain.EntitiesDTO.Configuration;
using WebAPI.Domain.EntitiesDTO.ControlPanel;
using WebAPI.Domain.EntitiesDTO.Others;
using WebAPI.Domain.ExtensionMethods;

namespace WebAPI.Application.Mapping;

public class MappingProfile : AutoMapper.Profile
{
    public MappingProfile()
    {
        CreateMap<UserRequestDTO, User>()
        .ForMember(dest => dest.Id, act => act.MapFrom(src => src.GetId()))
        .ForMember(dest => dest.Status, act => act.MapFrom(src => src.IsActive))
        .ForMember(dest => dest.IsAuthenticated, act => act.MapFrom(src => src.IsAuthenticated))
        .ForMember(dest => dest.LastPassword, act => act.MapFrom(src => src.LastPassword.ApplyTrim()))
        .ForMember(dest => dest.Login, act => act.MapFrom(src => src.Login.ApplyTrim()))
        .ForMember(dest => dest.Password, act => act.MapFrom(src => src.Password.ApplyTrim()));

        CreateMap<User, UserResponseDTO>()
        .ForMember(dest => dest.IsActive, act => act.MapFrom(src => src.Status))
        .ForMember(dest => dest.IsAuthenticated, act => act.MapFrom(src => src.IsAuthenticated))
        .ForMember(dest => dest.LastPassword, act => act.MapFrom(src => src.LastPassword.ApplyTrim()))
        .ForMember(dest => dest.Login, act => act.MapFrom(src => src.Login.ApplyTrim()))
        .ForMember(dest => dest.Password, act => act.MapFrom(src => src.Password.ApplyTrim())).ReverseMap();

        CreateMap<Log, LogResponseDTO>()
        .ForMember(dest => dest.Class, act => act.MapFrom(src => src.Class.ApplyTrim()))
        .ForMember(dest => dest.Method, act => act.MapFrom(src => src.Method.ApplyTrim()))
        .ForMember(dest => dest.MessageError, act => act.MapFrom(src => src.MessageError))
        .ForMember(dest => dest.Object, act => act.MapFrom(src => src.Object))
        .ForMember(dest => dest.UpdateTime, act => act.MapFrom(src => src.UpdateTime))
        .ForMember(dest => dest.Id, act => act.MapFrom(src => src.Id)).ReverseMap();

        CreateMap<Audit, AuditResponseDTO>()
        .ForMember(dest => dest.TableName, act => act.MapFrom(src => src.TableName.ApplyTrim()))
        .ForMember(dest => dest.ActionName, act => act.MapFrom(src => src.ActionName.ApplyTrim()))
        .ForMember(dest => dest.Id, act => act.MapFrom(src => src.Id))
        .ForMember(dest => dest.KeyValues, act => act.MapFrom(src => src.KeyValues))
        .ForMember(dest => dest.NewValues, act => act.MapFrom(src => src.NewValues))
        .ForMember(dest => dest.OldValues, act => act.MapFrom(src => src.OldValues)).ReverseMap();

        //TODO: Ajustar 
        //CreateMap<Audit, AuditResponseDTO>()
        //.ForMember(dest => dest.TableName, act => act.MapFrom(src => src.TableName.ApplyTrim()))
        //.ForMember(dest => dest.ActionName, act => act.MapFrom(src => src.ActionName.ApplyTrim()))
        //.ForMember(dest => dest.Id, act => act.MapFrom(src => src.Id))
        //.ForMember(dest => dest.KeyValues, act => act.MapFrom(src => src.KeyValues))
        //.ForMember(dest => dest.NewValues, act => act.MapFrom(src => src.NewValues))
        //.ForMember(dest => dest.OldValues, act => act.MapFrom(src => src.OldValues)).ReverseMap();

        CreateMap<UserResponseDTO, UserExcelDTO>().ReverseMap();

        CreateMap<Region, RegionQueryFilterResponse>().ReverseMap();

        CreateMap<AuthenticationSettingsCreateRequestDTO, AuthenticationSettings>()
        .ForMember(dest => dest.Id, act => act.MapFrom(src => src.Id))
        .ForMember(dest => dest.NumberOfTryToBlockUser, act => act.MapFrom(src => src.NumberOfTryToBlockUser))
        .ForMember(dest => dest.BlockUserTime, act => act.MapFrom(src => src.BlockUserTime))
        .ForMember(dest => dest.ApplyTwoFactoryValidation, act => act.MapFrom(src => src.ApplyTwoFactoryValidation))
        .ForMember(dest => dest.IdEnvironmentType, act => act.MapFrom(src => src.IdEnvironment))
        .ForMember(dest => dest.CreateDate, act => act.MapFrom(src => DateOnlyExtensionMethods.GetDateTimeNowFromBrazil()))
        .ForMember(dest => dest.Status, act => act.MapFrom(src => true));

        CreateMap<AuthenticationSettingsUpdateRequestDTO, AuthenticationSettings>()
        .ForMember(dest => dest.Id, act => act.MapFrom(src => src.Id))
        .ForMember(dest => dest.NumberOfTryToBlockUser, act => act.MapFrom(src => src.NumberOfTryToBlockUser))
        .ForMember(dest => dest.BlockUserTime, act => act.MapFrom(src => src.BlockUserTime))
        .ForMember(dest => dest.ApplyTwoFactoryValidation, act => act.MapFrom(src => src.ApplyTwoFactoryValidation))
        .ForMember(dest => dest.IdEnvironmentType, act => act.MapFrom(src => src.IdEnvironment))
        .ForMember(dest => dest.UpdateDate, act => act.MapFrom(src => DateOnlyExtensionMethods.GetDateTimeNowFromBrazil()));

        CreateMap<ExpirationPasswordSettingsCreateRequestDTO, ExpirationPasswordSettings>()
        .ForMember(dest => dest.Id, act => act.MapFrom(src => src.Id))
        .ForMember(dest => dest.QtyDaysPasswordExpire, act => act.MapFrom(src => src.QtyDaysPasswordExpire))
        .ForMember(dest => dest.NotifyExpirationDays, act => act.MapFrom(src => src.NotifyExpirationDays))
        .ForMember(dest => dest.IdEnvironmentType, act => act.MapFrom(src => src.IdEnvironment))
        .ForMember(dest => dest.CreateDate, act => act.MapFrom(src => DateOnlyExtensionMethods.GetDateTimeNowFromBrazil()))
        .ForMember(dest => dest.Status, act => act.MapFrom(src => true));

        CreateMap<ExpirationPasswordSettingsUpdateRequestDTO, ExpirationPasswordSettings>()
        .ForMember(dest => dest.Id, act => act.MapFrom(src => src.Id))
        .ForMember(dest => dest.QtyDaysPasswordExpire, act => act.MapFrom(src => src.QtyDaysPasswordExpire))
        .ForMember(dest => dest.NotifyExpirationDays, act => act.MapFrom(src => src.NotifyExpirationDays))
        .ForMember(dest => dest.IdEnvironmentType, act => act.MapFrom(src => src.IdEnvironment))
        .ForMember(dest => dest.UpdateDate, act => act.MapFrom(src => DateOnlyExtensionMethods.GetDateTimeNowFromBrazil()));

        CreateMap<RequiredPasswordSettingsCreateRequestDTO, RequiredPasswordSettings>()
        .ForMember(dest => dest.Id, act => act.MapFrom(src => src.Id))
        .ForMember(dest => dest.MinimalOfChars, act => act.MapFrom(src => src.MinimalOfChars))
        .ForMember(dest => dest.MustHaveNumbers, act => act.MapFrom(src => src.MustHaveNumbers))
        .ForMember(dest => dest.MustHaveSpecialChars, act => act.MapFrom(src => src.MustHaveSpecialChars))
        .ForMember(dest => dest.MustHaveUpperCaseLetter, act => act.MapFrom(src => src.MustHaveUpperCaseLetter))
        .ForMember(dest => dest.IdEnvironmentType, act => act.MapFrom(src => src.IdEnvironment))
        .ForMember(dest => dest.CreateDate, act => act.MapFrom(src => DateOnlyExtensionMethods.GetDateTimeNowFromBrazil()))
        .ForMember(dest => dest.Status, act => act.MapFrom(src => true));

        CreateMap<RequiredPasswordSettingsUpdateRequestDTO, RequiredPasswordSettings>()
        .ForMember(dest => dest.Id, act => act.MapFrom(src => src.Id))
        .ForMember(dest => dest.MinimalOfChars, act => act.MapFrom(src => src.MinimalOfChars))
        .ForMember(dest => dest.MustHaveNumbers, act => act.MapFrom(src => src.MustHaveNumbers))
        .ForMember(dest => dest.MustHaveSpecialChars, act => act.MapFrom(src => src.MustHaveSpecialChars))
        .ForMember(dest => dest.MustHaveUpperCaseLetter, act => act.MapFrom(src => src.MustHaveUpperCaseLetter))
        .ForMember(dest => dest.IdEnvironmentType, act => act.MapFrom(src => src.IdEnvironment))
        .ForMember(dest => dest.UpdateDate, act => act.MapFrom(src => DateOnlyExtensionMethods.GetDateTimeNowFromBrazil()));

        CreateMap<LogSettingsCreateRequestDTO, LogSettings>()
        .ForMember(dest => dest.Id, act => act.MapFrom(src => src.Id))
        .ForMember(dest => dest.SaveLogUpdateData, act => act.MapFrom(src => src.SaveLogUpdateData))
        .ForMember(dest => dest.SaveLogResearchData, act => act.MapFrom(src => src.SaveLogResearchData))
        .ForMember(dest => dest.SaveLogCreateData, act => act.MapFrom(src => src.SaveLogCreateData))
        .ForMember(dest => dest.SaveLogDeleteData, act => act.MapFrom(src => src.SaveLogDeleteData))
        .ForMember(dest => dest.SaveLogTurnOffSystem, act => act.MapFrom(src => src.SaveLogTurnOffSystem))
        .ForMember(dest => dest.SaveLogTurnOnSystem, act => act.MapFrom(src => src.SaveLogTurnOnSystem))
        .ForMember(dest => dest.IdEnvironmentType, act => act.MapFrom(src => src.IdEnvironment))
        .ForMember(dest => dest.CreateDate, act => act.MapFrom(src => DateOnlyExtensionMethods.GetDateTimeNowFromBrazil()))
        .ForMember(dest => dest.Status, act => act.MapFrom(src => true));

        CreateMap<LogSettingsUpdateRequestDTO, LogSettings>()
        .ForMember(dest => dest.Id, act => act.MapFrom(src => src.Id))
        .ForMember(dest => dest.SaveLogUpdateData, act => act.MapFrom(src => src.SaveLogUpdateData))
        .ForMember(dest => dest.SaveLogResearchData, act => act.MapFrom(src => src.SaveLogResearchData))
        .ForMember(dest => dest.SaveLogCreateData, act => act.MapFrom(src => src.SaveLogCreateData))
        .ForMember(dest => dest.SaveLogDeleteData, act => act.MapFrom(src => src.SaveLogDeleteData))
        .ForMember(dest => dest.SaveLogTurnOffSystem, act => act.MapFrom(src => src.SaveLogTurnOffSystem))
        .ForMember(dest => dest.SaveLogTurnOnSystem, act => act.MapFrom(src => src.SaveLogTurnOnSystem))
        .ForMember(dest => dest.IdEnvironmentType, act => act.MapFrom(src => src.IdEnvironment))
        .ForMember(dest => dest.UpdateDate, act => act.MapFrom(src => DateOnlyExtensionMethods.GetDateTimeNowFromBrazil()));

        CreateMap<EnvironmentTypeSettingsCreateRequestDTO, EnvironmentTypeSettings>()
        .ForMember(dest => dest.Id, act => act.MapFrom(src => src.Id))
        .ForMember(dest => dest.Description, act => act.MapFrom(src => src.EnvironmentDescription))
        .ForMember(dest => dest.Initials, act => act.MapFrom(src => src.EnvironmentInitial))
        .ForMember(dest => dest.CreateDate, act => act.MapFrom(src => DateOnlyExtensionMethods.GetDateTimeNowFromBrazil()))
        .ForMember(dest => dest.Status, act => act.MapFrom(src => true));

        CreateMap<EnvironmentTypeSettingsUpdateRequestDTO, EnvironmentTypeSettings>()
        .ForMember(dest => dest.Id, act => act.MapFrom(src => src.Id))
        .ForMember(dest => dest.Description, act => act.MapFrom(src => src.EnvironmentDescription))
        .ForMember(dest => dest.Initials, act => act.MapFrom(src => src.EnvironmentInitial))
        .ForMember(dest => dest.UpdateDate, act => act.MapFrom(src => DateOnlyExtensionMethods.GetDateTimeNowFromBrazil()));
    }
}