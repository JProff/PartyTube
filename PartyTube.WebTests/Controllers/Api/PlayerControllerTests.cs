using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PartyTube.Model.Db;
using PartyTube.Service.Interfaces;
using PartyTube.Web.Controllers.Api;
using PartyTube.Web.Hubs;
using Xunit;

namespace PartyTube.WebTests.Controllers.Api
{
    public class PlayerControllerTests
    {
        public PlayerControllerTests()
        {
            _playerService = new Mock<IPlayerService>();
            _broadcaster = new Mock<IBroadcaster>();
            _playerController =
                new PlayerController(_playerService.Object, _broadcaster.Object);
        }

        private readonly Mock<IPlayerService> _playerService;
        private readonly PlayerController _playerController;
        private readonly Mock<IBroadcaster> _broadcaster;

        [Fact]
        public async Task GetNowPlaying_Should_Return_Valid_Result()
        {
            var fixture = new Fixture();
            var expected = fixture.Build<NowPlaying>()
                                  .With(item => item.Video,
                                        fixture.Build<VideoItem>().Without(item => item.PlaylistVideoItems).Create())
                                  .Create();
            _playerService.Setup(service => service.GetNowPlayingAsync()).ReturnsAsync(expected);

            var actual = await _playerController.GetNowPlaying().ConfigureAwait(false);

            _playerService.Verify(service => service.GetNowPlayingAsync(), Times.Once);
            var ok = actual as OkObjectResult;
            Assert.NotNull(ok);
            var actualItem = ok.Value as NowPlaying;
            Assert.NotNull(actualItem);
            actualItem.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task PlayNext_Should_Return_Valid_Result()
        {
            var fixture = new Fixture();
            var expected = fixture.Build<NowPlaying>()
                                  .With(item => item.Video,
                                        fixture.Build<VideoItem>().Without(item => item.PlaylistVideoItems).Create())
                                  .Create();
            _playerService.Setup(service => service.PlayNextAsync()).ReturnsAsync(expected);

            var actual = await _playerController.PlayNext().ConfigureAwait(false);

            _playerService.Verify(service => service.PlayNextAsync(), Times.Once);
            var ok = actual as OkObjectResult;
            Assert.NotNull(ok);
            var actualItem = ok.Value as NowPlaying;
            Assert.NotNull(actualItem);
            actualItem.Should().BeEquivalentTo(expected);
            _broadcaster.Verify(broadcaster => broadcaster.NowPlayingAsync(It.Is<NowPlaying>(np => np == actualItem)),
                                Times.Once);
            _broadcaster.Verify(broadcaster => broadcaster.CurrentPlaylistAsync(), Times.Once);
        }

        [Fact]
        public async Task PlayNow_Should_Return_Valid_Result()
        {
            var fixture = new Fixture();
            var expected = fixture.Build<NowPlaying>()
                                  .With(item => item.Video,
                                        fixture.Build<VideoItem>().Without(item => item.PlaylistVideoItems).Create())
                                  .Create();
            _playerService.Setup(service => service.PlayNowAsync(It.IsAny<VideoItem>())).ReturnsAsync(expected);

            var actual = await _playerController.PlayNow(expected.Video).ConfigureAwait(false);

            _playerService.Verify(service => service.PlayNowAsync(It.Is<VideoItem>(item => item == expected.Video)),
                                  Times.Once);
            var ok = actual as OkObjectResult;
            Assert.NotNull(ok);
            var actualItem = ok.Value as NowPlaying;
            Assert.NotNull(actualItem);
            actualItem.Should().BeEquivalentTo(expected);
            _broadcaster.Verify(broadcaster => broadcaster.NowPlayingAsync(It.Is<NowPlaying>(np => np == actualItem)),
                                Times.Once);
        }

        [Fact]
        public async Task StartStop_Should_Return_Valid_Result()
        {
            var fixture = new Fixture();
            var expected = fixture.Build<NowPlaying>()
                                  .With(item => item.Video,
                                        fixture.Build<VideoItem>().Without(item => item.PlaylistVideoItems).Create())
                                  .Create();
            _playerService.Setup(service => service.StartStopAsync(It.IsAny<bool>())).ReturnsAsync(expected);

            var actual = await _playerController.StartStop(!expected.IsPlaying).ConfigureAwait(false);

            _playerService.Verify(service => service.StartStopAsync(It.Is<bool>(b => b == !expected.IsPlaying)),
                                  Times.Once);
            var ok = actual as OkObjectResult;
            Assert.NotNull(ok);
            var actualItem = ok.Value as NowPlaying;
            Assert.NotNull(actualItem);
            actualItem.Should().BeEquivalentTo(expected);
            _broadcaster.Verify(broadcaster => broadcaster.NowPlayingAsync(It.Is<NowPlaying>(np => np == actualItem)),
                                Times.Once);
        }
    }
}