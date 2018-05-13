using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using PartyTube.Model.Db;
using PartyTube.Repository.Interfaces;
using PartyTube.Service;
using Xunit;

namespace PartyTube.ServiceTests
{
    public class HistoryServiceTests
    {
        public HistoryServiceTests()
        {
            _historyRepository = new Mock<IHistoryRepository>();
            _historyService = new HistoryService(_historyRepository.Object);
        }

        private readonly Mock<IHistoryRepository> _historyRepository;
        private readonly HistoryService _historyService;

        [Fact]
        public async Task DeleteByIdAsync_Should_Call_Repository()
        {
            const int id = 1;
            await _historyService.DeleteByIdAsync(id).ConfigureAwait(false);

            _historyRepository.Verify(repository => repository.DeleteByIdAsync(It.Is<int>(i => i == id)), Times.Once);
        }

        [Fact]
        public async Task DeleteByVideoIdentifierAsync_Should_Call_Repository()
        {
            const string videoIdentifier = "videoIdentifier";
            await _historyService.DeleteByVideoIdentifierAsync(videoIdentifier).ConfigureAwait(false);

            _historyRepository.Verify(repository =>
                                          repository.DeleteByVideoIdentifierAsync(
                                              It.Is<string>(s => s == videoIdentifier)),
                                      Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_Valid_Result()
        {
            const int skip = 1;
            const int take = 2;
            var fixture = new Fixture();
            var expected = fixture.Build<HistoryItem>()
                                  .With(item => item.Video,
                                        fixture.Build<VideoItem>().Without(item => item.PlaylistVideoItems).Create())
                                  .CreateMany(2)
                                  .ToArray();
            _historyRepository.Setup(repository => repository.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                              .ReturnsAsync(expected);

            var actual = await _historyService.GetAllAsync(skip, take).ConfigureAwait(false);

            actual.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
            _historyRepository.Verify(repository =>
                                          repository.GetAllAsync(It.Is<int>(i => i == skip),
                                                                 It.Is<int>(i => i == take)),
                                      Times.Once);
        }
    }
}