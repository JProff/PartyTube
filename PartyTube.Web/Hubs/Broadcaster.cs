using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.SignalR;
using PartyTube.Model.Db;

namespace PartyTube.Web.Hubs
{
    public class Broadcaster : IBroadcaster
    {
        [NotNull] private readonly IHubContext<PlayerHub> _playerHubContext;

        public Broadcaster([NotNull] IHubContext<PlayerHub> playerHubContext)
        {
            _playerHubContext = playerHubContext;
        }

        [NotNull]
        public Task NowPlayingAsync([NotNull] NowPlaying nowPlaying)
        {
            return _playerHubContext.Clients.All.SendAsync(Method.NowPlaying.ToString(), nowPlaying);
        }

        [NotNull]
        public Task CurrentPlaylistAsync()
        {
            return _playerHubContext.Clients.All.SendAsync(Method.CurrentPlaylist.ToString());
        }

        [NotNull]
        public Task ClearCurrentPlaylistAsync()
        {
            return _playerHubContext.Clients.All.SendAsync(Method.ClearCurrentPlaylist.ToString());
        }

        private enum Method
        {
            NowPlaying,
            CurrentPlaylist,
            ClearCurrentPlaylist
        }
    }
}