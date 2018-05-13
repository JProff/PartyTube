using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PartyTube.Model.Db;
using PartyTube.Repository;
using Xunit;

namespace PartyTube.RepositoryTests
{
    public class VideoRepositoryTests
    {
        public VideoRepositoryTests()
        {
            _context = new PartyTubeDbContextMock();
            _videoRepository = new VideoRepository(_context.Context);
        }

        private readonly PartyTubeDbContextMock _context;
        private readonly VideoRepository _videoRepository;

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("identifier")]
        public async Task GetByIdentifierAsync_Should_Return_Null(string identifier)
        {
            var actual = await _videoRepository.GetByIdentifierAsync(identifier).ConfigureAwait(false);

            Assert.Null(actual);
        }

        [Fact]
        public async Task GetAttachedOfFoundedAsync_Should_Attach()
        {
            var context = _context.Context;
            var fixture = new Fixture();
            var expected = fixture.Build<VideoItem>().Without(item => item.PlaylistVideoItems).Create();
            await context.Video.AddAsync(expected).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
            context.Entry(expected).State = EntityState.Detached;
            var videoItem = new VideoItem(expected.Id,
                                          expected.VideoIdentifier,
                                          expected.Title,
                                          expected.ThumbnailUrl,
                                          expected.DurationInSeconds);

            var actual = await _videoRepository.GetAttachedOfFoundedAsync(videoItem, context).ConfigureAwait(false);

            Assert.Equal(EntityState.Unchanged, context.Entry(actual).State);
        }

        [Fact]
        public async Task GetAttachedOfFoundedAsync_Should_Find()
        {
            var context = _context.Context;
            var fixture = new Fixture();
            var expected = fixture.Build<VideoItem>().Without(item => item.PlaylistVideoItems).Create();
            await context.Video.AddAsync(expected).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);

            var videoItem = new VideoItem {VideoIdentifier = expected.VideoIdentifier};
            var actual = await _videoRepository.GetAttachedOfFoundedAsync(videoItem, context).ConfigureAwait(false);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GetAttachedOfFoundedAsync_Should_Throw_ArgumentNullException_For_context()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.ThrowsAsync<ArgumentNullException>(
                       () => _videoRepository.GetAttachedOfFoundedAsync(new VideoItem(), null))
                  .ConfigureAwait(false);
        }

        [Fact]
        public void GetAttachedOfFoundedAsync_Should_Throw_ArgumentNullException_For_videoItem()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.ThrowsAsync<ArgumentNullException>(
                       () => _videoRepository.GetAttachedOfFoundedAsync(null, _context.Context))
                  .ConfigureAwait(false);
        }

        [Fact]
        public async Task GetByIdentifierAsync_Should_Be_Case_Sensitive()
        {
            var context = _context.Context;
            const string validIdentifier = "validIdentifier";
            context.Video.Add(new VideoItem {VideoIdentifier = validIdentifier.ToUpper()});
            context.SaveChanges();

            var actual = await _videoRepository.GetByIdentifierAsync(validIdentifier).ConfigureAwait(false);

            Assert.Null(actual);
        }

        [Fact]
        public async Task GetByIdentifierAsync_Should_Return_Valid_Result()
        {
            var expected = new VideoItem {VideoIdentifier = "videoIdentifier"};
            var context = _context.Context;
            context.Video.Add(expected);
            context.Video.Add(new VideoItem {VideoIdentifier = "id"});
            context.SaveChanges();

            var actual = await _videoRepository.GetByIdentifierAsync(expected.VideoIdentifier).ConfigureAwait(false);

            Assert.NotNull(actual);
            Assert.Equal(expected.VideoIdentifier, actual.VideoIdentifier);
        }
    }
}