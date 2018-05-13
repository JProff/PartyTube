using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using PartyTube.Model.Db;

namespace PartyTube.Repository.Interfaces
{
    public interface INowPlayingRepository
    {
        /// <exception cref="ArgumentNullException">If <paramref name="videoItem" /> is null</exception>
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