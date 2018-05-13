using System;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using JetBrains.Annotations;

namespace PartyTube.Service
{
    public class YouTubeServiceWrapper
    {
        private const string PartForListRequest = "snippet,contentDetails,statistics";
        private const string PartForIdsListRequest = "contentDetails";
        private const string PartForSearchListRequest = "snippet";
        private const string OnlyVideoType = "video";
        public const int YoutubeSearchMaxResults = 50;
        private readonly Func<YouTubeService> _youTubeServiceBuilder;

        public YouTubeServiceWrapper([NotNull] Func<YouTubeService> youTubeServiceBuilder)
        {
            _youTubeServiceBuilder = youTubeServiceBuilder;
        }

        [NotNull]
        [ItemCanBeNull]
        public virtual Task<VideoListResponse> VideosListExecuteAsync([CanBeNull] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Task.FromResult<VideoListResponse>(null);

            var service = _youTubeServiceBuilder.Invoke();
            var listRequest = service.Videos.List(PartForListRequest);
            listRequest.Id = id;
            listRequest.MaxResults = 1;
            return listRequest.ExecuteAsync();
        }

        [NotNull]
        [ItemNotNull]
        public virtual Task<VideoListResponse> VideosListFromIdsExecuteAsync([NotNull] string ids)
        {
            var service = _youTubeServiceBuilder.Invoke();
            var listRequest = service.Videos.List(PartForIdsListRequest);
            listRequest.Id = ids;
            return listRequest.ExecuteAsync();
        }

        [NotNull]
        [ItemCanBeNull]
        public virtual Task<SearchListResponse> SearchListExecuteAsync([CanBeNull] string searchTerm,
                                                                       int count,
                                                                       [CanBeNull] string pageToken)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Task.FromResult<SearchListResponse>(null);

            if (count <= 0 || count > YoutubeSearchMaxResults)
                throw new ArgumentOutOfRangeException(nameof(count),
                                                      count,
                                                      $"Acceptable values are 1 to {YoutubeSearchMaxResults}, inclusive");

            var service = _youTubeServiceBuilder.Invoke();
            var listRequest = service.Search.List(PartForSearchListRequest);
            listRequest.Q = searchTerm;
            listRequest.MaxResults = count;
            listRequest.PageToken = pageToken ?? string.Empty;
            listRequest.Type = OnlyVideoType;
            return listRequest.ExecuteAsync();
        }
    }
}