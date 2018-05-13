using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using PartyTube.Model.Db;
using PartyTube.Model.Settings;
using PartyTube.Repository;
using PartyTube.Repository.Interfaces;
using Xunit;

namespace PartyTube.RepositoryTests
{
    public class NowPlayingRepositoryTests
    {
        public NowPlayingRepositoryTests()
        {
            _context = new PartyTubeDbContextMock();
            _videoRepository = new VideoRepository(_context.Context);
            _currentPlaylistRepository = new CurrentPlaylistRepository(_context.Context, _videoRepository);
            var appSettings = new Mock<AppSettings>();
            _historyRepository = new HistoryRepository(_context.Context, appSettings.Object, _videoRepository);
            _repository = new NowPlayingRepository(_context.Context,
                                                   _videoRepository,
                                                   _currentPlaylistRepository,
                                                   _historyRepository);
        }

        private readonly IVideoRepository _videoRepository;
        private readonly PartyTubeDbContextMock _context;
        private readonly NowPlayingRepository _repository;
        private readonly ICurrentPlaylistRepository _currentPlaylistRepository;
        private readonly IHistoryRepository _historyRepository;

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task StartStopAsync_Should_Return_Valid_Result(bool isPlaying)
        {
            var context = _context.Context;
            context.Player.IsPlaying = !isPlaying;
            await context.SaveChangesAsync().ConfigureAwait(false);

            var actual = await _repository.StartStopAsync(isPlaying).ConfigureAwait(false);

            Assert.Equal(isPlaying, actual.IsPlaying);
        }

        [Fact]
        public async Task GetNowPlayingAsync_Should_Return_Valid_Result()
        {
            var context = _context.Context;
            var fixture = new Fixture();
            var expected = fixture.Build<NowPlaying>()
                                  .With(playing => playing.IsPlaying, true)
                                  .With(playing => playing.Video,
                                        fixture.Build<VideoItem>().Without(item => item.PlaylistVideoItems).Create())
                                  .Create();
            context.Player.IsPlaying = expected.IsPlaying;
            context.Player.Video = expected.Video;
            await context.SaveChangesAsync().ConfigureAwait(false);

            var actual = await _repository.GetNowPlayingAsync().ConfigureAwait(false);

            Assert.Equal(expected.IsPlaying, actual.IsPlaying);
            actual.Video.Should().BeEquivalentTo(expected.Video, options => options.Excluding(item => item.Id));
        }

        [Fact]
        public async Task PlayNowAsync_Should_Return_Valid_Result()
        {
            var fixture = new Fixture();
            var expected = fixture.Build<NowPlaying>()
                                  .With(playing => playing.IsPlaying, true)
                                  .With(playing => playing.Video,
                                        fixture.Build<VideoItem>().Without(item => item.PlaylistVideoItems).Create())
                                  .Create();
            // ReSharper disable once AssignNullToNotNullAttribute
            var actual = await _repository.PlayNowAsync(expected.Video).ConfigureAwait(false);

            Assert.Equal(expected.IsPlaying, actual.IsPlaying);
            actual.Video.Should().BeEquivalentTo(expected.Video, options => options.Excluding(item => item.Id));
        }

        [Fact]
        public void PlayNowAsync_Should_Throw_ArgumentNullException()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.ThrowsAsync<ArgumentNullException>(() => _repository.PlayNowAsync(null)).ConfigureAwait(false);
        }
    }
}