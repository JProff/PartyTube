using PartyTube.Service.Helpers;
using Xunit;

namespace PartyTube.ServiceTests.Helpers
{
    public class YoutubeHelpersTests
    {
        private readonly YoutubeHelpers _youtubeHelpers;

        public YoutubeHelpersTests()
        {
            _youtubeHelpers = new YoutubeHelpers();
        }

        [Theory]
        [MemberData(nameof(YoutubeHelpersTestsData.GetIdFromUrlData), MemberType = typeof(YoutubeHelpersTestsData))]
        public void Should_Get_Valid_Id_From_Url(string url, string expected)
        {
            var actual = _youtubeHelpers.GetIdFromUrl(url);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData("      ", "")]
        public void Should_Return_Null_Or_Empty_If_Url_Is_Null_Or_Empty(string url, string expected)
        {
            var actual = _youtubeHelpers.GetIdFromUrl(url);
            Assert.Equal(expected, actual);
        }
    }
}