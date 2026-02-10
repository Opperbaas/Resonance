using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Resonance.BusinessLogicLayer.DTOs;

namespace Resonance.BusinessLogicLayer.Interfaces
{
    public interface ISpecificService
    {
        Task<SpecificDto> GetAsync(Guid id);
        Task<IEnumerable<SpecificDto>> GetAllAsync();
    }
}