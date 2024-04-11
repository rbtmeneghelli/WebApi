﻿using WebAPI.Domain.CQRS.Command;
using KissLog;
using MediatR;
using Constants = WebAPI.Domain.Constants;

namespace WebAPI.V1.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize("Bearer")]
[AllowAnonymous]

public sealed class MediatorController : GenericController
{
    private readonly IMediator _mediator;

    public MediatorController(IMapper mapper, IHttpContextAccessor accessor, INotificationMessageService noticationMessageService, IKLogger iKLogger, IMediator mediator) : base(mapper, accessor, noticationMessageService, iKLogger)
    {
        _mediator = mediator;
    }

    [HttpPost("GetAllPaginate")]
    public async Task<IActionResult> GetAllPaginate([FromBody] RegionQueryFilterRequest findRegionQueryFilterHandler)
    {
        if (ModelStateIsInvalid()) return CustomResponse(ModelState);

        var model = await _mediator.Send(findRegionQueryFilterHandler);

        var result = ApplyMapToEntity<IEnumerable<Region>, IEnumerable<RegionQueryFilterResponse>>(model);

        return CustomResponse(result, Constants.SUCCESS_IN_GETALLPAGINATE);
    }

    [HttpPost("GetById")]
    public async Task<IActionResult> GetById([FromBody] RegionQueryByIdRequest regionQueryByIdRequest)
    {
        var model = await _mediator.Send(regionQueryByIdRequest);

        var result = ApplyMapToEntity<Region, RegionQueryFilterResponse>(model);

        return CustomResponse(result, Constants.SUCCESS_IN_GETID);
    }

    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("Add")]
    public async Task<IActionResult> Add([FromBody] CreateRegionCommandRequest createRegionCommandRequest)
    {
        if (ModelStateIsInvalid()) return CustomResponse(ModelState);

        var result = await _mediator.Send(createRegionCommandRequest);

        if (result)
            return CustomResponse();

        return BadRequest();
    }

    [HttpPut("Update")]
    public async Task<IActionResult> Update([FromBody] UpdateRegionCommandRequest updateRegionCommandRequest)
    {
        if (ModelStateIsInvalid()) return CustomResponse(ModelState);

        var result = await _mediator.Send(updateRegionCommandRequest);

        if (result)
            return NoContent();

        return CustomResponse();
    }
}