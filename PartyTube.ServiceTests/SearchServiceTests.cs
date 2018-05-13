using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using PartyTube.Model;
using PartyTube.Model.Db;
using PartyTube.Model.Settings;
using PartyTube.Repository.Interfaces;
using PartyTube.Service;
using PartyTube.Service.Helpers;
using PartyTube.Service.Interfaces;
using Xunit;

namespace PartyTube.ServiceTests
{
    public class SearchServiceTests
    {
        public SearchServiceTests()
        {
            _historyRepository = new Mock<IHistoryRepository>();
            _youtubeSearchService = new Mock<IYoutubeSearchService>();
            _appSettings = new Mock<AppSettings>();
            _videoRepository = new Mock<IVideoRepository>();
            _youtubeHelpers = new Mock<YoutubeHelpers>();
            _searchService = new SearchService(_historyRepository.Object,
                                               _youtubeSearchService.Object,
                                               _appSettings.Object,
                                               _videoRepository.Object,
                                               _youtubeHelpers.Object);
        }

        private readonly Mock<IHistoryRepository> _historyRepository;
        private readonly Mock<IYoutubeSearchService> _youtubeSearchService;
        private readonly Mock<AppSettings> _appSettings;
        private readonly SearchService _searchService;
        private readonly Mock<IVideoRepository> _videoRepository;
        private readonly Mock<YoutubeHelpers> _youtubeHelpers;

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetSearchPopupResultsAsync_Should_Return_In_Valid_Order(bool isPopupLocalFirst)
        {
            const string searchTerm = "searchTerm";
            var localData = new List<SearchPopupResult>
            {
                new SearchPopupResult("Local 1", true, 1),
                new SearchPopupResult("Local 2", true, 2)
            };
            var youtubeData = new List<SearchPopupResult>
            {
                new SearchPopupResult("youtube 1", false, 1),
                new SearchPopupResult("youtube 2", false, 2)
            };

            _appSettings.Object.SearchSettings.IsPopupLocalFirst = isPopupLocalFirst;
            _appSettings.Object.SearchSettings.PopupLocalMaxResultsCount = 2;
            _appSettings.Object.SearchSettings.PopupYoutubeMaxResultsCount = 2;

            _historyRepository.Setup(repository => repository.GetSearchPopupResultsAsync(It.IsAny<string>()))
                              .ReturnsAsync(localData);
            _youtubeSearchService.Setup(service => service.GetSearchPopupResultsAsync(It.IsAny<string>()))
                                 .ReturnsAsync(youtubeData);

            var expected = new List<SearchPopupResult>();
            if (isPopupLocalFirst)
            {
                expected.AddRange(localData);
                expected.AddRange(youtubeData);
            }
            else
            {
                expected.AddRange(youtubeData);
                expected.AddRange(localData);
            }

            var actual = await _searchService.GetSearchPopupResultsAsync(searchTerm).ConfigureAwait(false);

            actual.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }

        [Fact]
        public async Task GetLocalSearchVideoItemsAsync_Should_Return_Valid_Result()
        {
            const string searchTerm = "searchTerm";
            var fixture = new Fixture();
            var expected = fixture.Build<LocalSearchVideoItem>()
                                  .With(item => item.VideoItem,
                                        fixture.Build<VideoItem>().Without(item => item.PlaylistVideoItems).Create())
                                  .CreateMany(3)
                                  .ToArray();
            _historyRepository.Setup(repository => repository.SearchAsync(It.IsAny<string>())).ReturnsAsync(expected);

            var actual = await _searchService.GetLocalSearchVideoItemsAsync(searchTerm).ConfigureAwait(false);

            _historyRepository.Verify(repository => repository.SearchAsync(It.Is<string>(s => s == searchTerm)),
                                      Times.Once);
            actual.Should().BeEquivalentTo(expected, setup => setup.WithStrictOrdering());
        }

        [Fact]
        public async Task GetSearchPopupResultsAsync_Should_Return_Valid_Count()
        {
            const string searchTerm = "searchTerm";
            var localData = new List<SearchPopupResult>
            {
                new SearchPopupResult("Local 1", true, 1),
                new SearchPopupResult("Local 2", true, 2)
            };
            var youtubeData = new List<SearchPopupResult>
            {
                new SearchPopupResult("youtube 1", false, 1),
                new SearchPopupResult("youtube 2", false, 2),
                new SearchPopupResult("youtube 3", false, 2),
                new SearchPopupResult("youtube 4", false, 2),
                new SearchPopupResult("youtube 5", false, 2)
            };

            var settings = _appSettings.Object.SearchSettings;
            settings.IsPopupLocalFirst = true;
            settings.PopupLocalMaxResultsCount = 3;
            settings.PopupYoutubeMaxResultsCount = 3;

            _historyRepository.Setup(repository => repository.GetSearchPopupResultsAsync(It.IsAny<string>()))
                              .ReturnsAsync(localData);
            _youtubeSearchService.Setup(service => service.GetSearchPopupResultsAsync(It.IsAny<string>()))
                                 .ReturnsAsync(youtubeData);

            var expected = new List<SearchPopupResult>();
            expected.AddRange(localData);
            var take = settings.PopupYoutubeMaxResultsCount + (settings.PopupLocalMaxResultsCount - localData.Count);
            expected.AddRange(youtubeData.Take(take));

            var actual = await _searchService.GetSearchPopupResultsAsync(searchTerm).ConfigureAwait(false);

            actual.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }

        [Fact]
        public async Task GetVideoItemByIdOrUrlAsync_Should_Use_Local_Search_First()
        {
            var fixture = new Fixture();
            const string idOrUrl = "idOrUrl";
            var expected = fixture.Build<VideoItem>().Without(item => item.PlaylistVideoItems).Create();
            _youtubeHelpers.Setup(helpers => helpers.GetIdFromUrl(It.IsAny<string>())).Returns(idOrUrl);
            _videoRepository.Setup(repository => repository.GetByIdentifierAsync(It.IsAny<string>()))
                            .ReturnsAsync(expected);

            var actual = await _searchService.GetVideoItemByIdOrUrlAsync(idOrUrl).ConfigureAwait(false);

            _youtubeHelpers.Verify(helpers => helpers.GetIdFromUrl(It.Is<string>(s => s == idOrUrl)), Times.Once);
            _videoRepository.Verify(repository => repository.GetByIdentifierAsync(It.Is<string>(s => s == idOrUrl)),
                                    Times.Once);
            _youtubeSearchService.Verify(service => service.GetVideoItemByIdentifierAsync(It.IsAny<string>()),
                                         Times.Never);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetVideoItemByIdOrUrlAsync_Should_Use_Youtube_Search_Second()
        {
            var fixture = new Fixture();
            const string idOrUrl = "idOrUrl";
            var expected = fixture.Build<VideoItem>().Without(item => item.PlaylistVideoItems).Create();

            _youtubeHelpers.Setup(helpers => helpers.GetIdFromUrl(It.IsAny<string>())).Returns(idOrUrl);
            _videoRepository.Setup(repository => repository.GetByIdentifierAsync(It.IsAny<string>()))
                            .ReturnsAsync((VideoItem) null);
            _youtubeSearchService.Setup(repository => repository.GetVideoItemByIdentifierAsync(It.IsAny<string>()))
                                 .ReturnsAsync(expected);

            var actual = await _searchService.GetVideoItemByIdOrUrlAsync(idOrUrl).ConfigureAwait(false);

            _youtubeHelpers.Verify(helpers => helpers.GetIdFromUrl(It.Is<string>(s => s == idOrUrl)), Times.Once);
            _videoRepository.Verify(repository => repository.GetByIdentifierAsync(It.Is<string>(s => s == idOrUrl)),
                                    Times.Once);
            _youtubeSearchService.Verify(
                service => service.GetVideoItemByIdentifierAsync(It.Is<string>(s => s == idOrUrl)),
                Times.Once);
            actual.Should().BeEquivalentTo(expected);
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
            _youtubeSearchService
               .Setup(service => service.GetYoutubeSearchResultAsync(It.IsAny<string>(),
                                                                     It.IsAny<int>(),
                                                                     It.IsAny<string>()))
               .ReturnsAsync(expected);

            var actual = await _searchService.GetYoutubeSearchResultAsync(searchTerm, count, pageToken)
                                             .ConfigureAwait(false);

            actual.Should().BeEquivalentTo(expected, options => options.WithStrictOrderingFor(result => result.Videos));
        }
    }
}