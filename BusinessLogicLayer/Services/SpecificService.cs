using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Resonance.BusinessLogicLayer.DTOs;
using Resonance.BusinessLogicLayer.Interfaces;
using Resonance.DataAccessLayer.Interfaces;

namespace Resonance.BusinessLogicLayer.Services
{
    public class ItemService : IItemService
    {
        private readonly IUnitOfWork _uow;

        public ItemService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ItemDto> GetAsync(Guid id)
        {
            var entity = await _uow.ItemRepository.GetByIdAsync(id);
            if (entity == null) return null;

            return new ItemDto { Id = entity.Id, Name = entity.Name };
        }

        public async Task<IEnumerable<ItemDto>> GetAllAsync()
        {
            var list = await _uow.ItemRepository.GetAllAsync();
            return list.Select(e => new ItemDto { Id = e.Id, Name = e.Name });
        }
    }
}