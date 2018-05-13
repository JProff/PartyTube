using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using PartyTube.Model;
using PartyTube.Model.Db;

namespace PartyTube.Service.Interfaces
{
    public interface IYoutubeSearchService
    {
        [NotNull]
        [ItemCanBeNull]
        Task<VideoItem> GetVideoItemByIdentifierAsync([CanBeNull] string identifier);

        [NotNull]
        [ItemNotNull]
        Task<IEnumerable<SearchPopupResult>> GetSearchPopupResultsAsync([CanBeNull] string searchTerm);

        [NotNull]
        [ItemNotNull]
        Task<YoutubeSearchResult> GetYoutubeSearchResultAsync([CanBeNull] string searchTerm,
                                                              int count,
                                                              [CanBeNull] string pageToken);
    }
}