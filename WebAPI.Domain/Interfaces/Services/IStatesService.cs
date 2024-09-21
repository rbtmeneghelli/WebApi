﻿using WebAPI.Domain.Entities.Others;
using WebAPI.Domain.Filters.Others;
using WebAPI.Domain.Models;

namespace WebAPI.Domain.Interfaces.Services;

public interface IStatesService
{
    Task AddStatesAsync(IEnumerable<States> list);
    Task<long> GetStateByInitialsAsync(string initials);
    Task<List<States>> GetAllStatesAsync();
    Task RefreshStatesAsync(RefreshStates refreshStates);
    Task<bool> UpdateStatusByIdAsync(long id);
    Task<IEnumerable<States>> GetAllWithLikeAsync(string param);
    Task<PagedResult<States>> GetAllWithPaginateAsync(StateFilter filter);
}

