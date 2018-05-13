using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using PartyTube.DataAccess;
using PartyTube.Model.Db;
using PartyTube.Repository.Interfaces;

namespace PartyTube.Repository
{
    public class NowPlayingRepository : INowPlayingRepository
    {
        [NotNull] private readonly PartyTubeDbContext _context;
        [NotNull] private readonly ICurrentPlaylistRepository _currentPlaylistRepository;
        [NotNull] private readonly IHistoryRepository _historyRepository;
        [NotNull] private readonly IVideoRepository _videoRepository;

        public NowPlayingRepository([NotNull] PartyTubeDbContext context,
                                    [NotNull] IVideoRepository videoRepository,
                                    [NotNull] ICurrentPlaylistRepository currentPlaylistRepository,
                                    [NotNull] IHistoryRepository historyRepository)
        {
            _context = context;
            _videoRepository = videoRepository;
            _currentPlaylistRepository = currentPlaylistRepository;
            _historyRepository = historyRepository;
        }

        /// <exception cref="ArgumentNullException">If <paramref name="videoItem" /> is null</exception>
        [NotNull]
        [ItemNotNull]
        public Task<NowPlaying> PlayNowAsync([NotNull] VideoItem videoItem)
        {
            if (videoItem == null) throw new ArgumentNullException(nameof(videoItem));

            return PlayNowPrivateAsync(videoItem, true);
        }

        [NotNull]
        [ItemNotNull]
        public Task<NowPlaying> GetNowPlayingAsync()
        {
            return Task.FromResult(_context.Player);
        }

        [NotNull]
        [ItemNotNull]
        public async Task<NowPlaying> StartStopAsync(bool isPlaying)
        {
            _context.Player.IsPlaying = isPlaying;
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return _context.Player;
        }

        [NotNull]
        [ItemNotNull]
        public async Task<NowPlaying> PlayNextAsync()
        {
            if (_context.Player.Video != null)
            {
                await _historyRepository.AddAsync(_context.Player.Video, _context).ConfigureAwait(false);
                _context.Player.Video = null;
                _context.Player.IsPlaying = false;
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }

            var first = await _currentPlaylistRepository.GetFirstAsync().ConfigureAwait(false);
            if (first == null) return _context.Player;

            var nowPlaying = await PlayNowPrivateAsync(first.Video, false).ConfigureAwait(false);
            await _currentPlaylistRepository.RemoveAsync(first.Id, _context).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return nowPlaying;
        }

        [NotNull]
        [ItemNotNull]
        private async Task<NowPlaying> PlayNowPrivateAsync([NotNull] VideoItem videoItem, bool isSave)
        {
            videoItem = await _videoRepository.GetAttachedOfFoundedAsync(videoItem, _context).ConfigureAwait(false);
            _context.Player.Video = videoItem;
            _context.Player.IsPlaying = true;
            if (isSave)
                await _context.SaveChangesAsync().ConfigureAwait(false);
            return _context.Player;
        }
    }
}