using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using PartyTube.Model;
using PartyTube.Model.Db;
using PartyTube.Model.Settings;
using PartyTube.Repository.Interfaces;
using PartyTube.Service.Helpers;
using PartyTube.Service.Interfaces;

namespace PartyTube.Service
{
    public class SearchService : ISearchService
    {
        private readonly AppSettings _appSettings;
        private readonly IHistoryRepository _historyRepository;
        private readonly IVideoRepository _videoRepository;
        private readonly YoutubeHelpers _youtubeHelpers;
        private readonly IYoutubeSearchService _youtubeSearchService;

        public SearchService([NotNull] IHistoryRepository historyRepository,
                             [NotNull] IYoutubeSearchService youtubeSearchService,
                             [NotNull] AppSettings appSettings,
                             [NotNull] IVideoRepository videoRepository,
                             [NotNull] YoutubeHelpers youtubeHelpers)
        {
            _historyRepository = historyRepository;
            _youtubeSearchService = youtubeSearchService;
            _appSettings = appSettings;
            _videoRepository = videoRepository;
            _youtubeHelpers = youtubeHelpers;
        }


        [NotNull]
        [ItemNotNull]
        public Task<YoutubeSearchResult> GetYoutubeSearchResultAsync([CanBeNull] string searchTerm,
                                                                     int count,
                                                                     [CanBeNull] string pageToken)
        {
            return _youtubeSearchService.GetYoutubeSearchResultAsync(searchTerm, count, pageToken);
        }

        [NotNull]
        [ItemNotNull]
        public Task<IEnumerable<LocalSearchVideoItem>> GetLocalSearchVideoItemsAsync([CanBeNull] string searchTerm)
        {
            return _historyRepository.SearchAsync(searchTerm);
        }

        [NotNull]
        [ItemNotNull]
        public Task<IEnumerable<SearchPopupResult>> GetSearchPopupResultsAsync([CanBeNull] string searchTerm)
        {
            var local = _historyRepository.GetSearchPopupResultsAsync(searchTerm);
            var youtube = _youtubeSearchService.GetSearchPopupResultsAsync(searchTerm);

            return MergePopupResultsAsync(local, youtube);
        }

        [NotNull]
        [ItemCanBeNull]
        public async Task<VideoItem> GetVideoItemByIdOrUrlAsync([CanBeNull] string idOrUrl)
        {
            var id = _youtubeHelpers.GetIdFromUrl(idOrUrl);
            var video = await _videoRepository.GetByIdentifierAsync(id).ConfigureAwait(false);
            if (video != null) return video;

            video = await _youtubeSearchService.GetVideoItemByIdentifierAsync(id).ConfigureAwait(false);
            return video;
        }

        [ItemNotNull]
        private Task<IEnumerable<SearchPopupResult>> MergePopupResultsAsync(
            [NotNull] Task<IEnumerable<SearchPopupResult>> local,
            [NotNull] Task<IEnumerable<SearchPopupResult>> youtube)
        {
            Task.WaitAll(local, youtube);

            return Task.Run(() =>
            {
                var localResults = local.Result.ToList();
                var youtubeCount = _appSettings.SearchSettings.PopupYoutubeMaxResultsCount
                                 + _appSettings.SearchSettings.PopupLocalMaxResultsCount
                                 - localResults.Count;
                var youtubeResults = youtube.Result.Take(youtubeCount).ToList();

                IEnumerable<SearchPopupResult> result;
                if (_appSettings.SearchSettings.IsPopupLocalFirst)
                {
                    localResults.AddRange(youtubeResults);
                    result = localResults;
                }
                else
                {
                    youtubeResults.AddRange(localResults);
                    result = youtubeResults;
                }

                return result;
            });
        }
    }
}