using PartyTube.Model.Db;
using Xunit;

namespace PartyTube.ModelTests
{
    public class VideoItemTests
    {
        [Theory]
        [InlineData(10, 0, 10)]
        [InlineData(60, 1, 0)]
        [InlineData(70, 1, 10)]
        [InlineData(3600, 60, 0)]
        [InlineData(3670, 61, 10)]
        public void Should_Return_Valid_Minutes_And_Seconds(int totalSeconds, int minutes, int seconds)
        {
            var videoItem = new VideoItem {DurationInSeconds = totalSeconds};

            Assert.Equal(minutes, videoItem.Minutes);
            Assert.Equal(seconds, videoItem.Seconds);
        }
    }
}