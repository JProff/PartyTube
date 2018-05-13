using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using PartyTube.Model.Db;
using PartyTube.Service.Interfaces;
using PartyTube.Web.Hubs;

namespace PartyTube.Web.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/Player/[action]")]
    public class PlayerController : Controller
    {
        [NotNull] private readonly IBroadcaster _broadcaster;
        [NotNull] private readonly IPlayerService _playerService;

        public PlayerController([NotNull] IPlayerService playerService,
                                [NotNull] IBroadcaster broadcaster)
        {
            _playerService = playerService;
            _broadcaster = broadcaster;
        }

        [HttpPut]
        [ActionName("PlayNow")]
        public async Task<IActionResult> PlayNow([FromBody] VideoItem videoItem)
        {
            var result = await _playerService.PlayNowAsync(videoItem).ConfigureAwait(false);
            await _broadcaster.NowPlayingAsync(result).ConfigureAwait(false);

            return Ok(result);
        }

        [HttpPut]
        [ActionName("PlayNext")]
        public async Task<IActionResult> PlayNext()
        {
            var result = await _playerService.PlayNextAsync().ConfigureAwait(false);
            await _broadcaster.NowPlayingAsync(result).ConfigureAwait(false);
            await _broadcaster.CurrentPlaylistAsync().ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPut]
        [ActionName("StartStop")]
        public async Task<IActionResult> StartStop(bool isPlaying)
        {
            var result = await _playerService.StartStopAsync(isPlaying).ConfigureAwait(false);
            await _broadcaster.NowPlayingAsync(result).ConfigureAwait(false);

            return Ok(result);
        }

        [HttpGet]
        [ActionName("")]
        public async Task<IActionResult> GetNowPlaying()
        {
            var result = await _playerService.GetNowPlayingAsync().ConfigureAwait(false);

            return Ok(result);
        }
    }
}