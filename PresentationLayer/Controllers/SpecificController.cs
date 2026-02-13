using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Resonance.BusinessLogicLayer.DTOs;
using Resonance.BusinessLogicLayer.Interfaces;

namespace Resonance.PresentationLayer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _service;

        public ItemController(IItemService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> Get(Guid id)
        {
            var dto = await _service.GetAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }
    }
}