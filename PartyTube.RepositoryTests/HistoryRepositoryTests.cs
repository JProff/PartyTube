using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using PartyTube.Model;
using PartyTube.Model.Db;
using PartyTube.Model.Settings;
using PartyTube.Repository;
using PartyTube.Repository.Interfaces;
using Xunit;

namespace PartyTube.RepositoryTests
{
    public class HistoryRepositoryTests
    {
        public HistoryRepositoryTests()
        {
            _context = new PartyTubeDbContextMock();
            _appSettings = new AppSettings();
            _videoRepository = new VideoRepository(_context.Context);
            _historyRepository = new HistoryRepository(_context.Context, _appSettings, _videoRepository);
        }

        private readonly IVideoRepository _videoRepository;
        private readonly HistoryRepository _historyRepository;
        private readonly PartyTubeDbContextMock _context;
        private readonly AppSettings _appSettings;

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("     ")]
        [InlineData("videoIdentifier")]
        public async Task DeleteByVideoIdentifierAsync_Should_Run_Without_Exception(string videoIdentifier)
        {
            await _historyRepository.DeleteByVideoIdentifierAsync(videoIdentifier).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        [InlineData("searchTerm")]
        public async Task Should_Return_Empty_In_SearchAsync(string searchTerm)
        {
            var actual = await _historyRepository.SearchAsync(searchTerm).ConfigureAwait(false);

            Assert.Empty(actual);
        }

        [Theory]
        [InlineData(-1, 1)]
        [InlineData(0, 0)]
        public void GetAllAsync_Should_Throw_ArgumentOutOfRangeException(int skip, int take)
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _historyRepository.GetAllAsync(skip, take))
                  .ConfigureAwait(false);
        }

        [Fact]
        public async Task AddAsync_Should_Add_Duplicate()
        {
            var context = _context.Context;
            var fixture = new Fixture();
            var expected = fixture.Build<VideoItem>()
                                  .Without(item => item.Id)
                                  .Without(item => item.PlaylistVideoItems)
                                  .Create();

            await _historyRepository.AddAsync(expected).ConfigureAwait(false);
            var videoItem = new VideoItem(expected.VideoIdentifier,
                                          expected.Title,
                                          expected.ThumbnailUrl,
                                          expected.DurationInSeconds);
            await _historyRepository.AddAsync(videoItem).ConfigureAwait(false);

            var histories = context.History.ToArray();
            var videoItems = context.Video.ToArray();
            Assert.Single(videoItems);
            Assert.Equal(2, histories.Length);
            histories[0].Video.Should().BeEquivalentTo(expected);
            histories[1].Video.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task AddAsync_Should_Add_To_Db()
        {
            var context = _context.Context;
            var fixture = new Fixture();
            var expected = fixture.Build<VideoItem>().Without(item => item.PlaylistVideoItems).Create();
            context.Video.Add(expected);
            context.SaveChanges();

            var actual = await _historyRepository.AddAsync(expected).ConfigureAwait(false);

            actual.Video.Should().BeEquivalentTo(expected);

            var fromDb = context.History.Single();
            fromDb.Video.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void AddAsync_Should_Throw_ArgumentNullException()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.ThrowsAsync<ArgumentNullException>(() => _historyRepository.AddAsync(null)).ConfigureAwait(false);
        }

        [Fact]
        public async Task DeleteByIdAsync_Should_Delete()
        {
            const int count = 3;
            var fixture = new Fixture();
            var historyItems = fixture.Build<HistoryItem>()
                                      .With(item => item.Video,
                                            fixture.Build<VideoItem>()
                                                   .Without(item => item.PlaylistVideoItems)
                                                   .Create())
                                      .CreateMany(count)
                                      .ToArray();
            var expected = historyItems.Skip(1).ToArray();
            var context = _context.Context;
            await context.History.AddRangeAsync(expected).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);

            await _historyRepository.DeleteByIdAsync(historyItems[0].Id).ConfigureAwait(false);

            var actual = context.History.ToArray();
            // ReSharper disable once CoVariantArrayConversion
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task DeleteByIdAsync_Should_Run_Without_Exception()
        {
            await _historyRepository.DeleteByIdAsync(1).ConfigureAwait(false);
        }

        [Fact]
        public async Task DeleteByVideoIdentifierAsync_Should_Delete()
        {
            const int count = 3;
            const string videoIdentifier = "videoIdentifier";
            const string videoIdentifier2 = "videoIdentifier2";
            var fixture = new Fixture();
            var historyItems = fixture.Build<HistoryItem>()
                                      .With(item => item.Video,
                                            fixture.Build<VideoItem>()
                                                   .Without(item => item.PlaylistVideoItems)
                                                   .With(item => item.VideoIdentifier, videoIdentifier)
                                                   .Create())
                                      .CreateMany(count)
                                      .ToArray();
            var expected = historyItems.Take(1).ToArray();
            expected[0].Video.VideoIdentifier = videoIdentifier2;
            var context = _context.Context;
            await context.History.AddRangeAsync(expected).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);

            await _historyRepository.DeleteByVideoIdentifierAsync(videoIdentifier).ConfigureAwait(false);

            var actual = context.History.ToArray();
            // ReSharper disable once CoVariantArrayConversion
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_Empty()
        {
            const int skip = 0;
            const int take = 1;

            var actual = await _historyRepository.GetAllAsync(skip, take).ConfigureAwait(false);

            Assert.Empty(actual);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_Valid_Result()
        {
            const int skip = 1;
            const int take = 2;
            var fixture = new Fixture();
            var videoItems = fixture.Build<VideoItem>()
                                    .Without(item => item.PlaylistVideoItems)
                                    .CreateMany(4)
                                    .ToArray();
            var context = _context.Context;
            await context.Video.AddRangeAsync(videoItems).ConfigureAwait(false);
            context.SaveChanges();
            var historyItem0 = new HistoryItem(videoItems[0], new DateTime(2000, 1, 1));
            var historyItem1 = new HistoryItem(videoItems[1], new DateTime(2001, 1, 1));
            var historyItem2 = new HistoryItem(videoItems[2], new DateTime(2002, 1, 1));
            var historyItem3 = new HistoryItem(videoItems[3], new DateTime(2003, 1, 1));
            await context.History.AddRangeAsync(historyItem0, historyItem1, historyItem2, historyItem3)
                         .ConfigureAwait(false);
            context.SaveChanges();
            var expected = new[] {historyItem2, historyItem1};

            var actual = await _historyRepository.GetAllAsync(skip, take).ConfigureAwait(false);

            actual.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }


        [Fact]
        public async Task Should_Return_Empty_In_GetSearchPopupResultsAsync()
        {
            var actual = await _historyRepository.GetSearchPopupResultsAsync(string.Empty).ConfigureAwait(false);

            Assert.Empty(actual);
        }

        [Fact]
        public async Task Should_Return_Valid_Result_GetSearchPopupResultsAsync()
        {
            var content = _context.Context;
            const string searchTerm = "itL";

            var row1 = new HistoryItem {Video = {Title = "1TITLE"}};
            var row2 = new HistoryItem {Video = {Title = "2title"}};
            var row3 = new HistoryItem {Video = {Title = "3tiTle"}};
            var row4 = new HistoryItem {Video = {Title = "4t itl e"}};
            var row5 = new HistoryItem {Video = {Title = "5t itle"}};
            var row6 = new HistoryItem {Video = {Title = "6qwe"}};
            var row7 = new HistoryItem {Video = {Title = "2title"}};
            var row8 = new HistoryItem {Video = {Title = "8iTl"}};

            var expected = new[]
            {
                new SearchPopupResult(row2.Video.Title, true, 2),
                new SearchPopupResult(row1.Video.Title, true, 1),
                new SearchPopupResult(row3.Video.Title, true, 1),
                new SearchPopupResult(row4.Video.Title, true, 1),
                new SearchPopupResult(row5.Video.Title, true, 1)
            };
            _appSettings.SearchSettings.PopupLocalMaxResultsCount = expected.Length;

            await content.History.AddRangeAsync(row8, row7, row6, row5, row1, row2, row3, row4).ConfigureAwait(false);

            await content.SaveChangesAsync().ConfigureAwait(false);

            var actual = await _historyRepository.GetSearchPopupResultsAsync(searchTerm).ConfigureAwait(false);

            actual.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }

        [Fact]
        public async Task Should_Return_Valid_Result_SearchAsync()
        {
            const string searchTerm = "term";
            var content = _context.Context;

            var video1 = new VideoItem(1, string.Empty, $"1{searchTerm}", string.Empty, 0);
            var video2 = new VideoItem(2, string.Empty, $"2{searchTerm}", string.Empty, 0);
            var video3 = new VideoItem(3, string.Empty, $"3{searchTerm}", string.Empty, 0);
            var video4 = new VideoItem(4, string.Empty, "4", string.Empty, 0);

            var historyItem1 = new HistoryItem(video1);
            var historyItem2 = new HistoryItem(video2);
            var historyItem3 = new HistoryItem(video3);
            var historyItem4 = new HistoryItem(video4);
            var historyItem5 = new HistoryItem(video2);

            var expected = new[]
            {
                new LocalSearchVideoItem(video2, 2),
                new LocalSearchVideoItem(video1, 1),
                new LocalSearchVideoItem(video3, 1)
            };

            await content.Video.AddRangeAsync(video4, video3, video2, video1).ConfigureAwait(false);
            content.SaveChanges();
            await content.History.AddRangeAsync(historyItem5, historyItem4, historyItem3, historyItem2, historyItem1)
                         .ConfigureAwait(false);
            content.SaveChanges();

            var actual = await _historyRepository.SearchAsync(searchTerm).ConfigureAwait(false);

            actual.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }
    }
}