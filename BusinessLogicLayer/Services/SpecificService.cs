using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Resonance.BusinessLogicLayer.DTOs;
using Resonance.BusinessLogicLayer.Interfaces;
using Resonance.DataAccessLayer.Interfaces;

namespace Resonance.BusinessLogicLayer.Services
{
    public class SpecificService : ISpecificService
    {
        private readonly IUnitOfWork _uow;

        public SpecificService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SpecificDto> GetAsync(Guid id)
        {
            var entity = await _uow.SpecificRepository.GetByIdAsync(id);
            if (entity == null) return null;

            return new SpecificDto { Id = entity.Id, Name = entity.Name };
        }

        public async Task<IEnumerable<SpecificDto>> GetAllAsync()
        {
            var list = await _uow.SpecificRepository.GetAllAsync();
            return list.Select(e => new SpecificDto { Id = e.Id, Name = e.Name });
        }
    }
}