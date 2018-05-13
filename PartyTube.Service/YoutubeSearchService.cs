using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using Google.Apis.YouTube.v3.Data;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PartyTube.Model;
using PartyTube.Model.Db;
using PartyTube.Service.Interfaces;

namespace PartyTube.Service
{
    public class YoutubeSearchService : IYoutubeSearchService
    {
        private const string SearchUrl = "http://suggestqueries.google.com/complete/search";
        private const string SearchParamName = "q";
        private const string YoutubeVideoKind = "youtube#video";

        private static readonly (string param, string value)[] DefaultParams =
        {
            ("client", "firefox"),
            ("ds", "yt"),
            ("hl", "ru"),
        };

        private readonly Func<HttpClient> _httpClientBuilder;
        private readonly IMapper _mapper;
        private readonly YouTubeServiceWrapper _youTubeServiceWrapper;

        public YoutubeSearchService([NotNull] Func<HttpClient> httpClientBuilder,
                                    [NotNull] YouTubeServiceWrapper youTubeServiceWrapper,
                                    [NotNull] IMapper mapper)
        {
            _httpClientBuilder = httpClientBuilder;
            _youTubeServiceWrapper = youTubeServiceWrapper;
            _mapper = mapper;
        }

        // todo сделать проверку на отсутствие соединения
        [NotNull]
        [ItemCanBeNull]
        public async Task<VideoItem> GetVideoItemByIdentifierAsync([CanBeNull] string identifier)
        {
            var response = await _youTubeServiceWrapper.VideosListExecuteAsync(identifier).ConfigureAwait(false);

            if (response == null || response.Items.Count != 1 || response.Items[0].Kind != YoutubeVideoKind)
                return await Task.FromResult<VideoItem>(null).ConfigureAwait(false);

            var item = response.Items[0];
            var result = _mapper.Map<VideoItem>(item);
            return result;
        }

        [NotNull]
        [ItemNotNull]
        public async Task<YoutubeSearchResult> GetYoutubeSearchResultAsync([CanBeNull] string searchTerm,
                                                                           int count,
                                                                           [CanBeNull] string pageToken)
        {
            var response = await _youTubeServiceWrapper
                                .SearchListExecuteAsync(searchTerm, count, pageToken)
                                .ConfigureAwait(false);

            if (response?.Items == null || response.Items.Count == 0)
                return new YoutubeSearchResult();

            var videoItems = await GetVideoItemsFromSearchResponseAsync(response).ConfigureAwait(false);

            var total = response.PageInfo.TotalResults ?? 0;
            var result = new YoutubeSearchResult(videoItems, total, response.NextPageToken);
            return result;
        }

        // todo сделать нормальную проверку всего
        [NotNull]
        [ItemNotNull]
        public async Task<IEnumerable<SearchPopupResult>> GetSearchPopupResultsAsync([CanBeNull] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Enumerable.Empty<SearchPopupResult>();

            var client = _httpClientBuilder.Invoke();
            var uri = GetUriForPopup(searchTerm);
            var response = await client.GetAsync(uri).ConfigureAwait(false);
            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            // todo сделать потом нормальную проверку. Это временно.
            if (string.IsNullOrWhiteSpace(responseString))
                return Enumerable.Empty<SearchPopupResult>();

            var jArray = JsonConvert.DeserializeObject<JArray>(responseString);
            return jArray[1]
                  .Values<string>()
                  .Select(s => new SearchPopupResult(s.ToString()));
        }

        [NotNull]
        [ItemNotNull]
        private async Task<VideoItem[]> GetVideoItemsFromSearchResponseAsync([NotNull] SearchListResponse response)
        {
            var videoItemsDic = _mapper.Map<VideoItem[]>(response.Items).ToDictionary(d => d.VideoIdentifier, d => d);
            var ids = string.Join(", ", videoItemsDic.Keys);

            var idsResponse = await _youTubeServiceWrapper.VideosListFromIdsExecuteAsync(ids).ConfigureAwait(false);
            var idsDuration = _mapper.Map<VideoItem[]>(idsResponse.Items);

            var result = new List<VideoItem>();
            foreach (var duration in idsDuration)
            {
                if (!videoItemsDic.ContainsKey(duration.VideoIdentifier)) continue;

                var toResult = videoItemsDic[duration.VideoIdentifier];
                toResult.DurationInSeconds = duration.DurationInSeconds;
                result.Add(toResult);
            }

            return result.ToArray();
        }

        [NotNull]
        private static Uri GetUriForPopup([NotNull] string searchTerm)
        {
            var builder = new UriBuilder(SearchUrl);
            var query = HttpUtility.ParseQueryString(builder.Query);

            foreach (var (param, value) in DefaultParams)
            {
                query[param] = value;
            }

            query[SearchParamName] = searchTerm;
            builder.Query = Uri.EscapeUriString(HttpUtility.UrlDecode(query.ToString()));
            var uri = builder.Uri;
            return uri;
        }
    }
}