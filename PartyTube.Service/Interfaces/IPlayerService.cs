using System.Threading.Tasks;
using JetBrains.Annotations;
using PartyTube.Model.Db;

namespace PartyTube.Service.Interfaces
{
    public interface IPlayerService
    {
        [NotNull]
        [ItemNotNull]
        Task<NowPlaying> PlayNowAsync([NotNull] VideoItem videoItem);

        [NotNull]
        [ItemNotNull]
        Task<NowPlaying> GetNowPlayingAsync();

        [NotNull]
        [ItemNotNull]
        Task<NowPlaying> StartStopAsync(bool isPlaying);

        [NotNull]
        [ItemNotNull]
        Task<NowPlaying> PlayNextAsync();
    }
}