using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PartyTube.Model.Db;
using PartyTube.Service.Interfaces;
using PartyTube.Web.Controllers.Api;
using Xunit;

namespace PartyTube.WebTests.Controllers.Api
{
    public class HistoryControllerTests
    {
        public HistoryControllerTests()
        {
            _historyService = new Mock<IHistoryService>();
            _historyController = new HistoryController(_historyService.Object);
        }

        private readonly Mock<IHistoryService> _historyService;
        private readonly HistoryController _historyController;

        [Fact]
        public async Task DeleteById_Should_Return_Ok()
        {
            const int id = 1;

            var actual = await _historyController.DeleteById(id).ConfigureAwait(false);

            _historyService.Verify(service => service.DeleteByIdAsync(It.Is<int>(i => i == id)), Times.Once);
            var ok = actual as OkResult;
            Assert.NotNull(ok);
        }

        [Fact]
        public async Task DeleteByVideoIdentifier_Should_Return_Ok()
        {
            const string id = "id";

            var actual = await _historyController.DeleteByVideoIdentifier(id).ConfigureAwait(false);

            _historyService.Verify(service => service.DeleteByVideoIdentifierAsync(It.Is<string>(s => s == id)),
                                   Times.Once);
            var ok = actual as OkResult;
            Assert.NotNull(ok);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_NotFound()
        {
            const int skip = 1;
            const int take = 2;
            _historyService.Setup(service => service.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                           .ReturnsAsync(new HistoryItem[0]);

            var actual = await _historyController.GetAllAsync(skip, take).ConfigureAwait(false);
            var notFound = actual as NotFoundResult;

            _historyService.Verify(
                service => service.GetAllAsync(It.Is<int>(i => i == skip), It.Is<int>(i => i == take)),
                Times.Once);

            Assert.NotNull(notFound);
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
            _historyService.Setup(service => service.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                           .ReturnsAsync(expected);

            var actual = await _historyController.GetAllAsync(skip, take).ConfigureAwait(false);
            var ok = actual as OkObjectResult;

            _historyService.Verify(
                service => service.GetAllAsync(It.Is<int>(i => i == skip), It.Is<int>(i => i == take)),
                Times.Once);

            Assert.NotNull(ok);
            ok.Value.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }
    }
}