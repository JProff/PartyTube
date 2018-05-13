using System.Threading.Tasks;
using Moq;
using PartyTube.Model.Db;
using PartyTube.Repository.Interfaces;
using PartyTube.Service;
using Xunit;

namespace PartyTube.ServiceTests
{
    public class PlayerServiceTests
    {
        public PlayerServiceTests()
        {
            _nowPlayingRepository = new Mock<INowPlayingRepository>();
            _playerService = new PlayerService(_nowPlayingRepository.Object);
        }

        private readonly Mock<INowPlayingRepository> _nowPlayingRepository;
        private readonly PlayerService _playerService;

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task StartStopAsync_Should_Call_Repository(bool isPlaying)
        {
            await _playerService.StartStopAsync(isPlaying).ConfigureAwait(false);

            _nowPlayingRepository.Verify(repository => repository.StartStopAsync(It.Is<bool>(b => b == isPlaying)),
                                         Times.Once);
        }

        [Fact]
        public async Task GetNowPlayingAsync_Should_Call_Repository()
        {
            await _playerService.GetNowPlayingAsync().ConfigureAwait(false);

            _nowPlayingRepository.Verify(repository => repository.GetNowPlayingAsync(), Times.Once);
        }

        [Fact]
        public async Task PlayNextAsync_Should_Call_Repository()
        {
            var expected = new NowPlaying();
            _nowPlayingRepository.Setup(repository => repository.PlayNextAsync()).ReturnsAsync(expected);

            var actual = await _playerService.PlayNextAsync().ConfigureAwait(false);

            _nowPlayingRepository.Verify(repository => repository.PlayNextAsync(), Times.Once);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task PlayNowAsync_Should_Call_Repository()
        {
            var videoItem = new VideoItem();
            await _playerService.PlayNowAsync(videoItem).ConfigureAwait(false);

            _nowPlayingRepository.Verify(
                repository => repository.PlayNowAsync(It.Is<VideoItem>(item => item == videoItem)),
                Times.Once);
        }
    }
}