using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Resonance.BusinessLogicLayer.DTOs;

namespace Resonance.BusinessLogicLayer.Interfaces
{
    public interface IItemService
    {
        Task<ItemDto> GetAsync(Guid id);
        Task<IEnumerable<ItemDto>> GetAllAsync();
    }
}