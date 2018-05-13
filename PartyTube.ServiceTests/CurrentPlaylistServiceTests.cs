using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using PartyTube.Model.Db;
using PartyTube.Repository.Interfaces;
using PartyTube.Service;
using PartyTube.Service.Interfaces;
using Xunit;

namespace PartyTube.ServiceTests
{
    public class CurrentPlaylistServiceTests
    {
        public CurrentPlaylistServiceTests()
        {
            _searchService = new Mock<ISearchService>();
            _currentPlaylistRepository = new Mock<ICurrentPlaylistRepository>();
            _service = new CurrentPlaylistService(_searchService.Object, _currentPlaylistRepository.Object);
        }

        private readonly Mock<ISearchService> _searchService;
        private readonly Mock<ICurrentPlaylistRepository> _currentPlaylistRepository;
        private readonly CurrentPlaylistService _service;

        [Fact]
        public async Task AddAsync_Should_Call_Repository()
        {
            var videoItem = new VideoItem();
            await _service.AddAsync(videoItem).ConfigureAwait(false);

            _currentPlaylistRepository.Verify(
                repository => repository.AddAsync(It.Is<VideoItem>(item => item == videoItem)),
                Times.Once);
        }

        [Fact]
        public async Task AddToStartAsync_Should_Call_Repository()
        {
            var videoItem = new VideoItem();
            await _service.AddToStartAsync(videoItem).ConfigureAwait(false);

            _currentPlaylistRepository.Verify(
                repository => repository.AddToStartAsync(It.Is<VideoItem>(item => item == videoItem)),
                Times.Once);
        }


        [Fact]
        public async Task AddVideoByIdOrUrlAsync_Should_Return_Null_If_Not_Found()
        {
            const string idOrUrl = "idOrUrl";
            _searchService.Setup(service => service.GetVideoItemByIdOrUrlAsync(It.IsAny<string>()))
                          .ReturnsAsync((VideoItem) null);

            var actual = await _service.AddVideoByIdOrUrlAsync(idOrUrl).ConfigureAwait(false);

            _searchService.Verify(service => service.GetVideoItemByIdOrUrlAsync(It.Is<string>(s => s == idOrUrl)),
                                  Times.Once);
            _currentPlaylistRepository.Verify(repository => repository.AddAsync(It.IsAny<VideoItem>()), Times.Never);
            Assert.Null(actual);
        }

        [Fact]
        public async Task AddVideoByIdOrUrlAsync_Should_Return_Valid_Result()
        {
            var fixture = new Fixture();
            var expectedVideoItem = fixture.Build<VideoItem>().Without(item => item.PlaylistVideoItems).Create();
            var expected = fixture.Build<CurrentPlaylistItem>().With(item => item.Video, expectedVideoItem).Create();
            const string idOrUrl = "idOrUrl";
            _searchService.Setup(service => service.GetVideoItemByIdOrUrlAsync(It.IsAny<string>()))
                          .ReturnsAsync(expectedVideoItem);
            _currentPlaylistRepository.Setup(repository => repository.AddAsync(It.IsAny<VideoItem>()))
                                      .ReturnsAsync(expected);

            var actual = await _service.AddVideoByIdOrUrlAsync(idOrUrl).ConfigureAwait(false);

            _searchService.Verify(service => service.GetVideoItemByIdOrUrlAsync(It.Is<string>(s => s == idOrUrl)),
                                  Times.Once);
            _currentPlaylistRepository.Verify(repository =>
                                                  repository.AddAsync(
                                                      It.Is<VideoItem>(item => item.Equals(expectedVideoItem))),
                                              Times.Once);
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task ClearAsync_Should_Call_Repository()
        {
            await _service.ClearAsync().ConfigureAwait(false);

            _currentPlaylistRepository.Verify(repository => repository.ClearAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_Should_Call_Repository()
        {
            await _service.GetAllAsync().ConfigureAwait(false);

            _currentPlaylistRepository.Verify(repository => repository.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task RemoveAsync_Should_Call_Repository()
        {
            const int id = 666;
            await _service.RemoveAsync(id).ConfigureAwait(false);

            _currentPlaylistRepository.Verify(repository => repository.RemoveAsync(It.Is<int>(i => i == id)),
                                              Times.Once);
        }

        [Fact]
        public async Task ReorderAsync_Should_Call_Repository()
        {
            var ids = new[] {666};
            await _service.ReorderAsync(ids).ConfigureAwait(false);

            _currentPlaylistRepository.Verify(repository => repository.ReorderAsync(It.Is<int[]>(i => i == ids)),
                                              Times.Once);
        }
    }
}