using System.Threading.Tasks;
using JetBrains.Annotations;
using PartyTube.Model;
using PartyTube.Model.Db;
using PartyTube.Repository.Interfaces;
using PartyTube.Service.Interfaces;

namespace PartyTube.Service
{
    public class CurrentPlaylistService : ICurrentPlaylistService
    {
        private readonly ICurrentPlaylistRepository _currentPlaylistRepository;
        private readonly ISearchService _searchService;

        public CurrentPlaylistService([NotNull] ISearchService searchService,
                                      [NotNull] ICurrentPlaylistRepository currentPlaylistRepository)
        {
            _searchService = searchService;
            _currentPlaylistRepository = currentPlaylistRepository;
        }

        [NotNull]
        [ItemNotNull]
        public Task<CurrentPlaylistVideos> GetAllAsync()
        {
            return _currentPlaylistRepository.GetAllAsync();
        }

        [NotNull]
        public Task ReorderAsync([NotNull] int[] ids)
        {
            return _currentPlaylistRepository.ReorderAsync(ids);
        }

        [NotNull]
        [ItemCanBeNull]
        public async Task<CurrentPlaylistItem> AddVideoByIdOrUrlAsync([CanBeNull] string idOrUrl)
        {
            var videoItem = await _searchService.GetVideoItemByIdOrUrlAsync(idOrUrl).ConfigureAwait(false);

            if (videoItem == null)
                return await Task.FromResult<CurrentPlaylistItem>(null).ConfigureAwait(false);

            var result = await _currentPlaylistRepository.AddAsync(videoItem).ConfigureAwait(false);
            return result;
        }

        [NotNull]
        [ItemNotNull]
        public Task<CurrentPlaylistItem> AddAsync([NotNull] VideoItem videoItem)
        {
            return _currentPlaylistRepository.AddAsync(videoItem);
        }

        [NotNull]
        public Task RemoveAsync(int id)
        {
            return _currentPlaylistRepository.RemoveAsync(id);
        }

        [NotNull]
        public Task ClearAsync()
        {
            return _currentPlaylistRepository.ClearAsync();
        }

        [NotNull]
        [ItemNotNull]
        public Task<CurrentPlaylistItem> AddToStartAsync([NotNull] VideoItem videoItem)
        {
            return _currentPlaylistRepository.AddToStartAsync(videoItem);
        }
    }
}