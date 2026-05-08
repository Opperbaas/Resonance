using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Resonance.BusinessLogicLayer.DTOs;
using Resonance.BusinessLogicLayer.Interfaces;

namespace Resonance.PresentationLayer.Controllers
{
    [ApiController]
    [Route("tracks")]
    public class TracksController : ControllerBase
    {
        private readonly ITrackService _trackService;

        public TracksController(ITrackService trackService)
        {
            _trackService = trackService;
        }

        // GET /tracks?userId={guid}
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest(new { Message = "userId query parameter is required." });

            var tracks = await _trackService.GetTracksForUserAsync(userId);
            return Ok(tracks);
        }

        // POST /tracks
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TrackDto dto, [FromQuery] Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest(new { Message = "userId query parameter is required." });
            if (dto == null)
                return BadRequest(new { Message = "Body required." });

            await _trackService.AddTrackAsync(dto, userId);
            return Ok(new { Message = "Track added." });
        }
    }
}
