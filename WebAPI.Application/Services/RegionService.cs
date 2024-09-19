﻿using WebAPI.Application.Factory;
using WebAPI.Application.Generic;
using WebAPI.Application.InterfacesRepository;
using WebAPI.Domain.ExtensionMethods;
using WebAPI.Domain.Models;

namespace WebAPI.Application.Services;

public class RegionService : GenericService, IRegionService
{
    private readonly IRegionRepository _regionRepository;

    public RegionService(IRegionRepository regionRepository, INotificationMessageService notificationMessageService) : base(notificationMessageService)
    {
        _regionRepository = regionRepository;
    }

    public async Task<IEnumerable<Region>> GetAllRegionAsync()
    {
        return await _regionRepository.GetAll().ToListAsync();
    }

    public bool ExistRegionById(long regionId)
    {
        return _regionRepository.Exist(param => param.Id.Value == regionId);
    }

    public bool ExistRegion()
    {
        return _regionRepository.Exist(param => param.IsActive == true);
    }

    public Task AddRegionsAsync(IEnumerable<Region> list)
    {
        _regionRepository.AddRange(list);
        return Task.CompletedTask;
    }

    public async Task RefreshRegionAsync(IEnumerable<States> listStatesAPI)
    {
        try
        {
            IEnumerable<Region> regions = await _regionRepository.GetAll().ToListAsync();
            IEnumerable<Region> tmpRegion = listStatesAPI.Select(x => new Region()
            {
                Name = x.Region.Name,
                Initials = x.Region.Initials,
                IsActive = true
            });

            IEnumerable<Region> listaRegiaoAPI = tmpRegion.GroupBy(x => new { x.Name, x.Initials }).Select(g => g.First());

            foreach (var item in listaRegiaoAPI)
            {

                Region region = regions.FirstOrDefault(x => x.Initials == item.Initials && x.IsActive == true);
                if (GuardClauses.ObjectIsNotNull(region))
                {
                    region.UpdateTime = region.GetNewUpDateTime();
                    region.Name = item.Name;
                    region.Initials = item.Initials;
                    _regionRepository.Update(region);
                }
                else
                {
                    _regionRepository.Add(item);
                }
            }
        }
        catch
        {
            Notify(FixConstants.ERROR_IN_REFRESHREGION);
        }
    }

    public async Task<bool> UpdateStatusByIdAsync(long id)
    {
        try
        {
            Region record = await Task.FromResult(_regionRepository.GetById(id));
            if (GuardClauses.ObjectIsNotNull(record))
            {
                record.IsActive = record.IsActive == true ? false : true;
                _regionRepository.Update(record);
                return true;
            }
            return false;
        }
        catch
        {
            Notify(FixConstants.ERROR_IN_UPDATESTATUS);
            return false;
        }
    }

    public async Task<IEnumerable<Region>> GetAllWithLikeAsync(string parameter) => await _regionRepository.FindBy(x => EF.Functions.Like(x.Name, $"%{parameter}%")).ToListAsync();

    public async Task<PagedResult<Region>> GetAllWithPaginateAsync(RegionFilter filter)
    {
        try
        {
            var query = await GetAllWithFilterAsync(filter);
            var queryCount = GetCount(GetPredicate(filter));

            var queryResult = from x in query.AsQueryable()
                              orderby x.Name ascending
                              select new Region
                              {
                                  Id = x.Id,
                                  Name = x.Name,
                                  Initials = x.Initials,
                                  IsActive = x.IsActive
                              };

            return PagedFactory.GetPaged(queryResult, PagedFactory.GetDefaultPageIndex(filter.PageIndex), PagedFactory.GetDefaultPageSize(filter.PageSize));
        }
        catch (Exception ex)
        {
            Notify(FixConstants.ERROR_IN_GETALL);
            return PagedFactory.GetPaged(Enumerable.Empty<Region>().AsQueryable(), PagedFactory.GetDefaultPageIndex(filter.PageIndex), PagedFactory.GetDefaultPageSize(filter.PageSize));
        }
    }

    public long GetCount(Expression<Func<Region, bool>> predicate)
    {
        return _regionRepository.GetCount(predicate);
    }

    private async Task<IQueryable<Region>> GetAllWithFilterAsync(RegionFilter filter)
    {
        return await Task.FromResult(_regionRepository.GetAll().Where(GetPredicate(filter)).AsQueryable());
    }

    private Expression<Func<Region, bool>> GetPredicate(RegionFilter filter)
    {
        return p =>
               (GuardClauses.IsNullOrWhiteSpace(filter.Name) || p.Name.StartsWith(filter.Name.ApplyTrim()));
    }
}
