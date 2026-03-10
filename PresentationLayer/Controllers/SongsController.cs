using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Resonance.BusinessLogicLayer.DTOs;
using Resonance.BusinessLogicLayer.Interfaces;

namespace Resonance.PresentationLayer.Controllers
{
    [ApiController]
    [Route("songs")]
    public class SongsController : ControllerBase
    {
        private readonly ISongService _songService;

        public SongsController(ISongService songService)
        {
            _songService = songService;
        }

        // GET /songs?userId={guid}
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest(new { Message = "userId query parameter is required." });

            var songs = await _songService.GetSongsForUserAsync(userId);
            return Ok(songs);
        }

        // POST /songs
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SongDto dto, [FromQuery] Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest(new { Message = "userId query parameter is required." });
            if (dto == null)
                return BadRequest(new { Message = "Body required." });

            await _songService.AddSongAsync(dto, userId);
            return Ok(new { Message = "Song added." });
        }
    }
}
