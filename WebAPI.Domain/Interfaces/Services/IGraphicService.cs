﻿using WebAPI.Application.Services.Graphics;
using WebAPI.Domain.Models.Charts;

namespace WebAPI.Domain.Interfaces.Services;

public interface IBaseGraphicService<TGraphic> : IDisposable where TGraphic : class
{
    void BuildGraphic(TGraphic graphic);
    void CalculateGraphic(TGraphic graphic);
}


public interface IGraphicLineService : IBaseGraphicService<GraphicLineModel> 
{
}

public interface IGraphicBarService : IBaseGraphicService<GraphicBarModel>
{
}