using System.Threading.Tasks;
using JetBrains.Annotations;
using PartyTube.Model.Db;

namespace PartyTube.Web.Hubs
{
    public interface IBroadcaster
    {
        [NotNull]
        Task NowPlayingAsync([NotNull] NowPlaying nowPlaying);

        [NotNull]
        Task CurrentPlaylistAsync();

        [NotNull]
        Task ClearCurrentPlaylistAsync();
    }
}