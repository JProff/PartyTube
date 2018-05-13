using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Google.Apis.YouTube.v3.Data;
using Moq;
using PartyTube.Model;
using PartyTube.Model.Db;
using PartyTube.Model.Mappers;
using PartyTube.Service;
using PartyTube.ServiceTests.Mappers;
using Xunit;

namespace PartyTube.ServiceTests
{
    public class YoutubeSearchServiceTests
    {
        public YoutubeSearchServiceTests()
        {
            _httpClientMock = new HttpClientMock();
            _youTubeServiceWrapper = new Mock<YouTubeServiceWrapper>(null);
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ModelProfile>();
                cfg.AddProfile<ServiceTestsProfile>();
            });
            _mapper = config.CreateMapper();
            _youtubeSearchService =
                new YoutubeSearchService(() => _httpClientMock.HttpClient.Object,
                                         _youTubeServiceWrapper.Object,
                                         _mapper);
        }

        private readonly YoutubeSearchService _youtubeSearchService;
        private readonly HttpClientMock _httpClientMock;
        private readonly Mock<YouTubeServiceWrapper> _youTubeServiceWrapper;
        private readonly IMapper _mapper;

        [Fact]
        public async Task GetVideoItemByIdentifierAsync_Should_Return_ValidResult()
        {
            var fixture = new Fixture();

            var expected = fixture.Build<VideoItem>().Without(w => w.PlaylistVideoItems).Create();

            var myResponse = new VideoListResponse
            {
                Items = new List<Video> {_mapper.Map<Video>(expected)}
            };

            _youTubeServiceWrapper.Setup(wrapper => wrapper.VideosListExecuteAsync(It.IsAny<string>()))
                                  .ReturnsAsync(myResponse);

            var actual = await _youtubeSearchService
                              .GetVideoItemByIdentifierAsync(expected.VideoIdentifier)
                              .ConfigureAwait(false);
            actual.Should()
                  .BeEquivalentTo(expected,
                                  options => options.Excluding(item => item.PlaylistVideoItems)
                                                    .Excluding(item => item.Id));
        }

        [Fact]
        public async Task GetYoutubeSearchResultAsync_Should_Return_Valid_Result()
        {
            const string searchTerm = "searchTerm";
            const int count = 2;
            const string pageToken = "pageToken";
            var fixture = new Fixture();
            var expected = fixture.Build<YoutubeSearchResult>()
                                  .With(result => result.Videos,
                                        fixture.Build<VideoItem>()
                                               .Without(item => item.PlaylistVideoItems)
                                               .CreateMany(count)
                                               .ToArray())
                                  .Create();
            var searchListResponse = new SearchListResponse
            {
                PageInfo = new PageInfo
                {
                    TotalResults = expected.Total
                },
                Items = _mapper.Map<List<SearchResult>>(expected.Videos),
                NextPageToken = expected.NextPageToken
            };
            var videoListResponse = new VideoListResponse
            {
                Items = _mapper.Map<List<Video>>(expected.Videos)
            };
            _youTubeServiceWrapper
               .Setup(
                    wrapper => wrapper.SearchListExecuteAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
               .ReturnsAsync(searchListResponse);
            _youTubeServiceWrapper.Setup(wrapper => wrapper.VideosListFromIdsExecuteAsync(It.IsAny<string>()))
                                  .ReturnsAsync(videoListResponse);

            var actual = await _youtubeSearchService
                              .GetYoutubeSearchResultAsync(searchTerm, count, pageToken)
                              .ConfigureAwait(false);

            actual.Should().BeEquivalentTo(expected, options => options.Excluding(result => result.Videos));
            actual.Videos.Should()
                  .BeEquivalentTo(expected.Videos, options => options.WithStrictOrdering().Excluding(item => item.Id));
        }

        [Fact]
        public async Task Popup_Should_Return_Empty_And_Not_Execute_SendAsync()
        {
            var actual = await _youtubeSearchService.GetSearchPopupResultsAsync(string.Empty).ConfigureAwait(false);

            Assert.Empty(actual);
            _httpClientMock.VerifySendAsync(Times.Never());
        }

        [Fact]
        public async Task Popup_Should_Return_Valid_Result_And_Valid_Count()
        {
            const string readAsStringResult = "[\"searchTerm\",[\"5\",\"1\",\"3\"]]";
            const string validSearchTerm = "searchTerm";
            var expected = new[]
            {
                new SearchPopupResult("5"),
                new SearchPopupResult("1"),
                new SearchPopupResult("3")
            };

            _httpClientMock.SetResponseMessage(readAsStringResult);
            var actual = await _youtubeSearchService.GetSearchPopupResultsAsync(validSearchTerm).ConfigureAwait(false);

            actual.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }

        [Fact]
        public async Task PopupSearch_Should_Run_Once_SendAsync_With_Valid_Url()
        {
            const string searchTerm = "тест";
            string expected = $"http://suggestqueries.google.com/complete/search?ds=yt&client=firefox&hl=ru&q={searchTerm}";
            await _youtubeSearchService.GetSearchPopupResultsAsync(searchTerm).ConfigureAwait(false);

            _httpClientMock.VerifySendAsyncUri(Times.Once(), expected);
        }
    }
}