using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Resonance.BusinessLogicLayer.DTOs;
using Resonance.BusinessLogicLayer.Interfaces;

namespace Resonance.PresentationLayer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpecificController : ControllerBase
    {
        private readonly ISpecificService _service;

        public SpecificController(ISpecificService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SpecificDto>> Get(Guid id)
        {
            var dto = await _service.GetAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }
    }
}