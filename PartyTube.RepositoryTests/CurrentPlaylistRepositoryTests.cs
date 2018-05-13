using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PartyTube.Model.Db;
using PartyTube.Repository;
using Xunit;

namespace PartyTube.RepositoryTests
{
    public class CurrentPlaylistRepositoryTests
    {
        public CurrentPlaylistRepositoryTests()
        {
            _context = new PartyTubeDbContextMock();
            _videoRepository = new VideoRepository(_context.Context);
            _repository = new CurrentPlaylistRepository(_context.Context, _videoRepository);
        }

        private readonly PartyTubeDbContextMock _context;
        private readonly CurrentPlaylistRepository _repository;
        private readonly VideoRepository _videoRepository;

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task AddAsync_Should_Add_With_Right_Order(int itemsInPlaylist)
        {
            var expected = itemsInPlaylist + 1;
            var context = _context.Context;
            var fixture = new Fixture();
            for (var i = 0; i < itemsInPlaylist; i++)
            {
                var vi = fixture.Build<VideoItem>().Without(item => item.PlaylistVideoItems).Create();
                var playlistItem = fixture.Build<CurrentPlaylistItem>()
                                          .With(item => item.Order, i + 1)
                                          .With(item => item.Video, vi)
                                          .Create();
                context.CurrentPlaylist.Add(playlistItem);
            }

            context.SaveChanges();
            var videoItem = fixture.Build<VideoItem>().Without(item => item.PlaylistVideoItems).Create();

            var actual = await _repository.AddAsync(videoItem).ConfigureAwait(false);

            Assert.Equal(expected, actual.Order);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(3)]
        public async Task AddToStartAsync_Should_Add_With_Right_Order(int count)
        {
            var context = _context.Context;
            var fixture = new Fixture();
            if (count > 0)
            {
                var toAdd = fixture.Build<CurrentPlaylistItem>()
                                   .With(item => item.Video,
                                         fixture.Build<VideoItem>().Without(item => item.PlaylistVideoItems).Create())
                                   .CreateMany(count)
                                   .ToArray();
                for (var i = 0; i < toAdd.Length; i++)
                {
                    toAdd[i].Order = i + 1;
                }

                await context.CurrentPlaylist.AddRangeAsync(toAdd).ConfigureAwait(false);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }

            var expectedVideoItem = fixture.Build<VideoItem>().Without(item => item.PlaylistVideoItems).Create();
            var expectedCurrentPlaylistItem = fixture.Build<CurrentPlaylistItem>()
                                                     .With(item => item.Video, expectedVideoItem)
                                                     .With(item => item.Order, 1)
                                                     .Create();
            var actual = await _repository.AddToStartAsync(expectedVideoItem).ConfigureAwait(false);

            actual.Should()
                  .BeEquivalentTo(expectedCurrentPlaylistItem,
                                  options => options.Excluding(item => item.Id).Excluding(item => item.Video));
            actual.Video.Should()
                  .BeEquivalentTo(expectedVideoItem,
                                  options => options.Excluding(item => item.PlaylistVideoItems)
                                                    .Excluding(item => item.Id));

            var currentPlaylistItems = await context.CurrentPlaylist.AsNoTracking()
                                                    .OrderBy(o => o.Order)
                                                    .ToArrayAsync()
                                                    .ConfigureAwait(false);
            for (var i = 0; i < count; i++)
            {
                Assert.Equal(i + 1, currentPlaylistItems[i].Order);
            }
        }

        [Fact]
        public async Task AddAsync_Should_Find_Video_If_Id_0()
        {
            var fixture = new Fixture();
            var expected = fixture.Build<VideoItem>()
                                  .Without(item => item.Id)
                                  .Without(item => item.PlaylistVideoItems)
                                  .Create();
            var context = _context.Context;
            context.Video.Add(expected);
            context.SaveChanges();

            var actual = await _repository.AddAsync(expected).ConfigureAwait(false);

            actual.Video.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void AddAsync_Should_Throw_ArgumentNullException()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.ThrowsAsync<ArgumentNullException>(() => _repository.AddAsync(null)).ConfigureAwait(false);
        }

        [Fact]
        public void AddToStartAsync_Should_Throw_ArgumentNullException()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.ThrowsAsync<ArgumentNullException>(() => _repository.AddToStartAsync(null)).ConfigureAwait(false);
        }

        [Fact]
        public async Task ClearAsync_Should_Clear()
        {
            var context = _context.Context;
            var fixture = new Fixture();
            await context.CurrentPlaylist.AddRangeAsync(fixture.Build<CurrentPlaylistItem>()
                                                               .With(item => item.Video,
                                                                     fixture.Build<VideoItem>()
                                                                            .Without(item => item.PlaylistVideoItems)
                                                                            .Create())
                                                               .CreateMany(3))
                         .ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);

            await _repository.ClearAsync().ConfigureAwait(false);
            var actual = context.CurrentPlaylist.ToArray();

            Assert.Empty(actual);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_Valid_Result()
        {
            var context = _context.Context;
            var fixture = new Fixture();
            var item1 = new CurrentPlaylistItem(fixture.Build<VideoItem>()
                                                       .Without(item => item.PlaylistVideoItems)
                                                       .With(item => item.DurationInSeconds, 20)
                                                       .Create(),
                                                1);
            var item2 = new CurrentPlaylistItem(fixture.Build<VideoItem>()
                                                       .Without(item => item.PlaylistVideoItems)
                                                       .With(item => item.DurationInSeconds, 30)
                                                       .Create(),
                                                2);
            var item3 = new CurrentPlaylistItem(fixture.Build<VideoItem>()
                                                       .Without(item => item.PlaylistVideoItems)
                                                       .With(item => item.DurationInSeconds, 60)
                                                       .Create(),
                                                3);
            var expected = new[] {item1, item2, item3};
            await context.AddRangeAsync(item3, item1, item2).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);

            var actual = await _repository.GetAllAsync().ConfigureAwait(false);

            actual.CurrentPlaylistItems.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
            Assert.Equal(1, actual.TotalMinutes);
            Assert.Equal(50, actual.TotalSeconds);
        }

        [Fact]
        public async Task GetFirstAsync_Should_Return_Null()
        {
            var actual = await _repository.GetFirstAsync().ConfigureAwait(false);

            Assert.Null(actual);
        }

        [Fact]
        public async Task GetFirstAsync_Should_Return_Valid_Result()
        {
            var context = _context.Context;
            var fixture = new Fixture();
            var item1 = new CurrentPlaylistItem(fixture.Build<VideoItem>()
                                                       .Without(item => item.PlaylistVideoItems)
                                                       .Create(),
                                                1);
            var item2 = new CurrentPlaylistItem(fixture.Build<VideoItem>()
                                                       .Without(item => item.PlaylistVideoItems)
                                                       .Create(),
                                                2);
            var expected = item1;
            await context.AddRangeAsync(item2, item1).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);

            var actual = await _repository.GetFirstAsync().ConfigureAwait(false);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task RemoveAsync_Should_Remove_And_Reorder()
        {
            const int count = 4;
            var context = _context.Context;
            var fixture = new Fixture();
            for (var i = 0; i < count; i++)
            {
                var vi = fixture.Build<VideoItem>().Without(item => item.PlaylistVideoItems).Create();
                var playlistItem = fixture.Build<CurrentPlaylistItem>()
                                          .With(item => item.Id, i + 1)
                                          .With(item => item.Order, i + 1)
                                          .With(item => item.Video, vi)
                                          .Create();
                context.CurrentPlaylist.Add(playlistItem);
            }

            await context.SaveChangesAsync().ConfigureAwait(false);

            await _repository.RemoveAsync(2).ConfigureAwait(false);

            var actual = await context.CurrentPlaylist.OrderBy(o => o.Order).ToArrayAsync().ConfigureAwait(false);
            Assert.Equal(3, actual.Length);
            for (var i = 0; i < 3; i++)
            {
                Assert.Equal(i + 1, actual[i].Order);
            }
        }

        [Fact]
        public async Task ReorderAsync_Should_Reorder()
        {
            const int count = 3;
            var context = _context.Context;
            var fixture = new Fixture();
            var items = new CurrentPlaylistItem[count];
            for (var i = 0; i < count; i++)
            {
                var idAndOrder = i + 1;
                items[i] = fixture.Build<CurrentPlaylistItem>()
                                  .With(item => item.Id, idAndOrder)
                                  .With(item => item.Order, idAndOrder)
                                  .With(item => item.Video,
                                        fixture.Build<VideoItem>().Without(item => item.PlaylistVideoItems).Create())
                                  .Create();
            }

            await context.CurrentPlaylist.AddRangeAsync(items).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
            var ids = Enumerable.Range(1, count).OrderByDescending(o => o).ToArray();

            await _repository.ReorderAsync(ids).ConfigureAwait(false);

            var actual = context.CurrentPlaylist.OrderBy(o => o.Order).ToArray();
            for (var i = 0; i < count; i++)
            {
                var expectedOrder = i + 1;
                var expectedId = ids[i];
                Assert.Equal(expectedOrder, actual[i].Order);
                Assert.Equal(expectedId, actual[i].Id);
            }
        }

        [Fact]
        public async Task ReorderAsync_Should_Reorder_With_Not_In_Range()
        {
            const int count = 3;
            var context = _context.Context;
            var fixture = new Fixture();
            var items = new CurrentPlaylistItem[count];
            for (var i = 0; i < count; i++)
            {
                var idAndOrder = i + 1;
                items[i] = fixture.Build<CurrentPlaylistItem>()
                                  .With(item => item.Id, idAndOrder)
                                  .With(item => item.Order, idAndOrder)
                                  .With(item => item.Video,
                                        fixture.Build<VideoItem>().Without(item => item.PlaylistVideoItems).Create())
                                  .Create();
            }

            await context.CurrentPlaylist.AddRangeAsync(items).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
            var ids = new[] {count};
            var expected = new[]
            {
                (Id: 3, Order: 1),
                (Id: 1, Order: 2),
                (Id: 2, Order: 3)
            };

            await _repository.ReorderAsync(ids).ConfigureAwait(false);

            var actual = context.CurrentPlaylist.OrderBy(o => o.Order).ToArray();
            for (var i = 0; i < count; i++)
            {
                Assert.Equal(expected[i].Order, actual[i].Order);
                Assert.Equal(expected[i].Id, actual[i].Id);
            }
        }

        [Fact]
        public void ReorderAsync_Should_Throw_ArgumentNullException()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.ThrowsAsync<ArgumentNullException>(() => _repository.ReorderAsync(null)).ConfigureAwait(false);
        }
    }
}