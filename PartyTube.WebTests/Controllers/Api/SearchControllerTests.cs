using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PartyTube.Model;
using PartyTube.Model.Db;
using PartyTube.Service.Interfaces;
using PartyTube.Web.Controllers.Api;
using Xunit;

namespace PartyTube.WebTests.Controllers.Api
{
    public class SearchControllerTests
    {
        public SearchControllerTests()
        {
            _searchService = new Mock<ISearchService>();
            _searchController = new SearchController(_searchService.Object);
        }

        private readonly Mock<ISearchService> _searchService;
        private readonly SearchController _searchController;

        [Fact]
        public async Task GetExternalAsync_Should_Return_NotFound()
        {
            const string term = "term";
            const int count = 2;
            const string pageToken = "pageToken";
            _searchService
               .Setup(service => service.GetYoutubeSearchResultAsync(It.IsAny<string>(),
                                                                     It.IsAny<int>(),
                                                                     It.IsAny<string>()))
               .ReturnsAsync(new YoutubeSearchResult());

            var actual = await _searchController.GetExternalAsync(term, count, pageToken).ConfigureAwait(false);
            var notFound = actual as NotFoundResult;

            _searchService.Verify(service => service.GetYoutubeSearchResultAsync(
                                      It.Is<string>(s => s == term),
                                      It.Is<int>(i => i == count),
                                      It.Is<string>(s => s == pageToken)),
                                  Times.Once);

            Assert.NotNull(notFound);
        }

        [Fact]
        public async Task GetExternalAsync_Should_Return_Valid_Result()
        {
            const string term = "term";
            const int count = 2;
            const string pageToken = "pageToken";
            var fixture = new Fixture();
            var expected = fixture.Build<YoutubeSearchResult>()
                                  .With(item => item.Videos,
                                        fixture.Build<VideoItem>()
                                               .Without(item => item.PlaylistVideoItems)
                                               .CreateMany(count)
                                               .ToArray())
                                  .Create();
            _searchService
               .Setup(service => service.GetYoutubeSearchResultAsync(It.IsAny<string>(),
                                                                     It.IsAny<int>(),
                                                                     It.IsAny<string>()))
               .ReturnsAsync(expected);


            var actual = await _searchController.GetExternalAsync(term, count, pageToken).ConfigureAwait(false);
            var ok = actual as OkObjectResult;

            _searchService.Verify(service => service.GetYoutubeSearchResultAsync(
                                      It.Is<string>(s => s == term),
                                      It.Is<int>(i => i == count),
                                      It.Is<string>(s => s == pageToken)),
                                  Times.Once);
            Assert.NotNull(ok);

            var value = ok.Value as YoutubeSearchResult;
            Assert.NotNull(value);

            value.Should().BeEquivalentTo(expected, options => options.Excluding(result => result.Videos));
            value.Videos.Should()
                 .BeEquivalentTo(expected.Videos, options => options.WithStrictOrdering().Excluding(item => item.Id));
        }

        [Fact]
        public async Task GetLocalAsync_Should_Return_NotFound()
        {
            const string term = "term";
            _searchService.Setup(service => service.GetLocalSearchVideoItemsAsync(It.IsAny<string>()))
                          .ReturnsAsync(Enumerable.Empty<LocalSearchVideoItem>());

            var actual = await _searchController.GetLocalAsync(term).ConfigureAwait(false);
            var notFound = actual as NotFoundResult;

            _searchService.Verify(service => service.GetLocalSearchVideoItemsAsync(It.Is<string>(s => s == term)),
                                  Times.Once);

            Assert.NotNull(notFound);
        }

        [Fact]
        public async Task GetLocalAsync_Should_Return_Valid_Result()
        {
            const string term = "term";
            var fixture = new Fixture();
            var expected = fixture.Build<LocalSearchVideoItem>()
                                  .With(item => item.VideoItem,
                                        fixture.Build<VideoItem>().Without(item => item.PlaylistVideoItems).Create())
                                  .CreateMany(3)
                                  .ToArray();
            _searchService.Setup(service => service.GetLocalSearchVideoItemsAsync(It.IsAny<string>()))
                          .ReturnsAsync(expected);

            var actual = await _searchController.GetLocalAsync(term).ConfigureAwait(false);
            var ok = actual as OkObjectResult;

            Assert.NotNull(ok);
            ok.Value.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
            _searchService.Verify(service => service.GetLocalSearchVideoItemsAsync(It.Is<string>(s => s == term)),
                                  Times.Once);
        }

        [Fact]
        public async Task GetPopup_Should_Return_Valid_Result()
        {
            const string term = "term";
            var expected = new[]
            {
                new SearchPopupResult("videoName1", true, 10),
                new SearchPopupResult("videoName 2", false, 5)
            };
            _searchService.Setup(service => service.GetSearchPopupResultsAsync(It.IsAny<string>()))
                          .ReturnsAsync(expected);

            var result = await _searchController.GetPopupAsync(term).ConfigureAwait(false);
            var ok = result as OkObjectResult;

            _searchService.Verify(service => service.GetSearchPopupResultsAsync(It.Is<string>(s => s == term)),
                                  Times.Once);
            Assert.NotNull(ok);
            ok.Value.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }
    }
}