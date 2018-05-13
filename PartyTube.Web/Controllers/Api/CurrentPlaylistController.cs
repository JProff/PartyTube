using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using PartyTube.Model.Db;
using PartyTube.Service.Interfaces;
using PartyTube.Web.Hubs;

namespace PartyTube.Web.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/CurrentPlaylist/[action]")]
    public class CurrentPlaylistController : Controller
    {
        [NotNull] private readonly IBroadcaster _broadcaster;
        private readonly ICurrentPlaylistService _currentPlaylistService;

        public CurrentPlaylistController([NotNull] ICurrentPlaylistService currentPlaylistService,
                                         [NotNull] IBroadcaster broadcaster)
        {
            _currentPlaylistService = currentPlaylistService;
            _broadcaster = broadcaster;
        }

        [HttpPut]
        [ActionName("AddToEndIdOrUrl")]
        public async Task<IActionResult> Put(string idOrUrl)
        {
            var result = await _currentPlaylistService.AddVideoByIdOrUrlAsync(idOrUrl).ConfigureAwait(false);

            if (result == null) return NotFound();

            await _broadcaster.CurrentPlaylistAsync().ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPut]
        [ActionName("AddToEnd")]
        public async Task<IActionResult> AddToEnd([FromBody] VideoItem videoItem)
        {
            var result = await _currentPlaylistService.AddAsync(videoItem).ConfigureAwait(false);

            await _broadcaster.CurrentPlaylistAsync().ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPut]
        [ActionName("Reorder")]
        public async Task<IActionResult> Reorder([FromBody] int[] ids)
        {
            await _currentPlaylistService.ReorderAsync(ids).ConfigureAwait(false);

            await _broadcaster.CurrentPlaylistAsync().ConfigureAwait(false);
            return Ok();
        }

        [HttpPut]
        [ActionName("AddToStart")]
        public async Task<IActionResult> AddToStart([FromBody] VideoItem videoItem)
        {
            var result = await _currentPlaylistService.AddToStartAsync(videoItem).ConfigureAwait(false);

            await _broadcaster.CurrentPlaylistAsync().ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet]
        [ActionName("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _currentPlaylistService.GetAllAsync().ConfigureAwait(false);

            return Ok(result);
        }

        [HttpDelete]
        [ActionName("Remove")]
        public async Task<IActionResult> Remove(int id)
        {
            await _currentPlaylistService.RemoveAsync(id).ConfigureAwait(false);
            await _broadcaster.CurrentPlaylistAsync().ConfigureAwait(false);
            return Ok();
        }

        [HttpDelete]
        [ActionName("Clear")]
        public async Task<IActionResult> Clear()
        {
            await _currentPlaylistService.ClearAsync().ConfigureAwait(false);
            await _broadcaster.ClearCurrentPlaylistAsync().ConfigureAwait(false);
            return Ok();
        }
    }
}