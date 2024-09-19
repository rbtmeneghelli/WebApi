﻿using WebAPI.Domain.CQRS.Command;
using MediatR;
using WebAPI.Domain.ExtensionMethods;
using WebAPI.Application.InterfacesRepository;
using WebAPI.Application.Generic;

namespace WebAPI.Application.Handlers.Command;

public class CreateRegionCommandHandler : GenericService, IRequestHandler<CreateRegionCommandRequest, bool>
{
    private readonly IRegionRepository _regionRepository;

    public CreateRegionCommandHandler(IRegionRepository regionRepository, INotificationMessageService notificationMessageService) : base(notificationMessageService)
    {
        _regionRepository = regionRepository;
    }

    public Task<bool> Handle(CreateRegionCommandRequest request, CancellationToken cancellationToken)
    {
        _regionRepository.Add(new Region()
        {
            Initials = "XPTO",
            IsActive = true,
            Name = "XPTO"
        });

        return Task.FromResult(true);
    }
}
