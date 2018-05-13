using System;
using System.Threading.Tasks;
using PartyTube.Service;
using Xunit;

namespace PartyTube.ServiceTests
{
    public class YouTubeServiceWrapperTests
    {
        private readonly YouTubeServiceWrapper _wrapper;

        public YouTubeServiceWrapperTests()
        {
            _wrapper = new YouTubeServiceWrapper(() => null);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(YouTubeServiceWrapper.YoutubeSearchMaxResults + 1)]
        public void SearchListExecuteAsync_Should_Throw_ArgumentOutOfRangeExceptionAsync(int count)
        {
            const string searchTerm = "searchTerm";
            const string pageToken = "pageToken";

            Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                       () => _wrapper.SearchListExecuteAsync(searchTerm, count, pageToken))
                  .ConfigureAwait(false);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("       ")]
        public async Task VideosListExecuteAsync_Should_Return_Null(string id)
        {
            var actual = await _wrapper.VideosListExecuteAsync(id).ConfigureAwait(false);
            Assert.Null(actual);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("       ")]
        public async Task SearchListExecuteAsync_Should_Return_Null(string searchTerm)
        {
            const int count = 2;
            const string pageToken = "pageToken";

            var actual = await _wrapper.SearchListExecuteAsync(searchTerm, count, pageToken).ConfigureAwait(false);

            Assert.Null(actual);
        }
    }
}