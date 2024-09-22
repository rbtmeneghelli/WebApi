﻿using WebAPI.Domain.CQRS.Command;
using MediatR;
using FixConstants = WebAPI.Domain.Constants.FixConstants;
using WebAPI.Domain.Entities.Others;
using WebAPI.Domain.Interfaces.Repository;
using WebAPI.Domain.CQRS.Queries;

namespace WebAPI.V1.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize("Bearer")]
[AllowAnonymous]

public sealed class MediatorController : GenericController
{
    private readonly IMediator _mediator;

    public MediatorController(
        IMediator mediator,
        IMapper iMapperService, 
        IHttpContextAccessor iHttpContextAccessor,
        IGenericNotifyLogsService iGenericNotifyLogsService) 
        : base(iMapperService, iHttpContextAccessor, iGenericNotifyLogsService)
    {
        _mediator = mediator;
    }

    [HttpPost("GetAllPaginate")]
    public async Task<IActionResult> GetAllPaginate([FromBody] RegionQueryFilterRequest findRegionQueryFilterHandler)
    {
        if (ModelStateIsInvalid()) return CustomResponse(ModelState);

        var model = await _mediator.Send(findRegionQueryFilterHandler);

        var result = ApplyMapToEntity<IEnumerable<Region>, IEnumerable<RegionQueryFilterResponse>>(model);

        return CustomResponse(result, FixConstants.SUCCESS_IN_GETALLPAGINATE);
    }

    [HttpPost("GetById")]
    public async Task<IActionResult> GetById([FromBody] RegionQueryByIdRequest regionQueryByIdRequest)
    {
        var model = await _mediator.Send(regionQueryByIdRequest);

        var result = ApplyMapToEntity<Region, RegionQueryFilterResponse>(model);

        return CustomResponse(result, FixConstants.SUCCESS_IN_GETID);
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