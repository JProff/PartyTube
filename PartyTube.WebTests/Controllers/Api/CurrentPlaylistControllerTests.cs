using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PartyTube.Model;
using PartyTube.Model.Db;
using PartyTube.Service.Interfaces;
using PartyTube.Web.Controllers.Api;
using PartyTube.Web.Hubs;
using Xunit;

namespace PartyTube.WebTests.Controllers.Api
{
    public class CurrentPlaylistControllerTests
    {
        public CurrentPlaylistControllerTests()
        {
            _currentPlaylistService = new Mock<ICurrentPlaylistService>();
            _broadcaster = new Mock<IBroadcaster>();
            _controller = new CurrentPlaylistController(_currentPlaylistService.Object, _broadcaster.Object);
        }

        private readonly CurrentPlaylistController _controller;
        private readonly Mock<ICurrentPlaylistService> _currentPlaylistService;
        private readonly Mock<IBroadcaster> _broadcaster;

        [Fact]
        public async Task AddToEnd_Should_Return_Valid_Result()
        {
            var fixture = new Fixture();
            var expected = fixture.Build<CurrentPlaylistItem>()
                                  .With(item => item.Video,
                                        fixture.Build<VideoItem>().Without(item => item.PlaylistVideoItems).Create())
                                  .Create();
            _currentPlaylistService.Setup(service => service.AddAsync(It.IsAny<VideoItem>())).ReturnsAsync(expected);

            var actual = await _controller.AddToEnd(expected.Video).ConfigureAwait(false);

            var ok = actual as OkObjectResult;
            Assert.NotNull(ok);
            var actualItem = ok.Value as CurrentPlaylistItem;
            Assert.NotNull(actualItem);
            actualItem.Should().BeEquivalentTo(expected);
            _broadcaster.Verify(broadcaster => broadcaster.CurrentPlaylistAsync(), Times.Once);
        }

        [Fact]
        public async Task AddToEndIdOrUrl_Should_Return_Not_Found()
        {
            const string idOrUrl = "idOrUrl";
            _currentPlaylistService.Setup(service => service.AddVideoByIdOrUrlAsync(It.IsAny<string>()))
                                   .ReturnsAsync((CurrentPlaylistItem) null);

            var actual = await _controller.Put(idOrUrl).ConfigureAwait(false);

            _currentPlaylistService.Verify(service => service.AddVideoByIdOrUrlAsync(It.Is<string>(s => s == idOrUrl)),
                                           Times.Once);

            var notFound = actual as NotFoundResult;
            Assert.NotNull(notFound);
            _broadcaster.Verify(broadcaster => broadcaster.CurrentPlaylistAsync(), Times.Never);
        }

        [Fact]
        public async Task AddToEndIdOrUrl_Should_Return_Valid_Result()
        {
            var fixture = new Fixture();
            var expectedVideoItem = fixture.Build<VideoItem>().Without(item => item.PlaylistVideoItems).Create();
            var expected = fixture.Build<CurrentPlaylistItem>().With(item => item.Video, expectedVideoItem).Create();
            const string idOrUrl = "https://www.youtube.com/watch?v=queryString";
            _currentPlaylistService.Setup(service => service.AddVideoByIdOrUrlAsync(It.IsAny<string>()))
                                   .ReturnsAsync(expected);

            var actual = await _controller.Put(idOrUrl).ConfigureAwait(false);

            _currentPlaylistService.Verify(
                service => service.AddVideoByIdOrUrlAsync(
                    It.Is<string>(s => s == idOrUrl)),
                Times.Once);

            var ok = actual as OkObjectResult;
            Assert.NotNull(ok);
            ok.Value.Should().BeEquivalentTo(expected);
            _broadcaster.Verify(broadcaster => broadcaster.CurrentPlaylistAsync(), Times.Once);
        }


        [Fact]
        public async Task AddToStart_Should_Return_Valid_Result()
        {
            var fixture = new Fixture();
            var expected = fixture.Build<CurrentPlaylistItem>()
                                  .With(item => item.Video,
                                        fixture.Build<VideoItem>().Without(item => item.PlaylistVideoItems).Create())
                                  .Create();
            _currentPlaylistService.Setup(service => service.AddToStartAsync(It.IsAny<VideoItem>()))
                                   .ReturnsAsync(expected);

            var actual = await _controller.AddToStart(expected.Video).ConfigureAwait(false);

            var ok = actual as OkObjectResult;
            Assert.NotNull(ok);
            var actualItem = ok.Value as CurrentPlaylistItem;
            Assert.NotNull(actualItem);
            actualItem.Should().BeEquivalentTo(expected);
            _broadcaster.Verify(broadcaster => broadcaster.CurrentPlaylistAsync(), Times.Once);
        }

        [Fact]
        public async Task Clear_Should_Return_Valid_Result()
        {
            var actual = await _controller.Clear().ConfigureAwait(false);

            var ok = actual as OkResult;
            Assert.NotNull(ok);
            _currentPlaylistService.Verify(service => service.ClearAsync(), Times.Once);
            _broadcaster.Verify(broadcaster => broadcaster.ClearCurrentPlaylistAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAll_Should_Return_Valid_Result()
        {
            var expected = new CurrentPlaylistVideos();
            _currentPlaylistService.Setup(service => service.GetAllAsync()).ReturnsAsync(expected);

            var actual = await _controller.GetAll().ConfigureAwait(false);

            _currentPlaylistService.Verify(service => service.GetAllAsync(), Times.Once);
            var ok = actual as OkObjectResult;
            Assert.NotNull(ok);
            var actualItem = ok.Value as CurrentPlaylistVideos;
            Assert.NotNull(actualItem);
            actualItem.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Remove_Should_Return_Valid_Result()
        {
            const int id = 666;
            var actual = await _controller.Remove(id).ConfigureAwait(false);

            var ok = actual as OkResult;
            Assert.NotNull(ok);
            _currentPlaylistService.Verify(service => service.RemoveAsync(It.Is<int>(i => i == id)), Times.Once);
            _broadcaster.Verify(broadcaster => broadcaster.CurrentPlaylistAsync(), Times.Once);
        }

        [Fact]
        public async Task Reorder_Should_Return_Valid_Result()
        {
            var ids = new[] {666};
            var actual = await _controller.Reorder(ids).ConfigureAwait(false);

            var ok = actual as OkResult;
            Assert.NotNull(ok);
            _currentPlaylistService.Verify(service => service.ReorderAsync(It.Is<int[]>(i => i == ids)), Times.Once);
            _broadcaster.Verify(broadcaster => broadcaster.CurrentPlaylistAsync(), Times.Once);
        }
    }
}