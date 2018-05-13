using System.Threading.Tasks;
using JetBrains.Annotations;
using PartyTube.Model.Db;
using PartyTube.Repository.Interfaces;
using PartyTube.Service.Interfaces;

namespace PartyTube.Service
{
    public class PlayerService : IPlayerService
    {
        [NotNull] private readonly INowPlayingRepository _nowPlayingRepository;

        public PlayerService([NotNull] INowPlayingRepository nowPlayingRepository)
        {
            _nowPlayingRepository = nowPlayingRepository;
        }

        [NotNull]
        [ItemNotNull]
        public Task<NowPlaying> PlayNowAsync([NotNull] VideoItem videoItem)
        {
            return _nowPlayingRepository.PlayNowAsync(videoItem);
        }

        [NotNull]
        [ItemNotNull]
        public Task<NowPlaying> GetNowPlayingAsync()
        {
            return _nowPlayingRepository.GetNowPlayingAsync();
        }

        [NotNull]
        [ItemNotNull]
        public Task<NowPlaying> StartStopAsync(bool isPlaying)
        {
            return _nowPlayingRepository.StartStopAsync(isPlaying);
        }

        [NotNull]
        [ItemNotNull]
        public Task<NowPlaying> PlayNextAsync()
        {
            return _nowPlayingRepository.PlayNextAsync();
        }
    }
}