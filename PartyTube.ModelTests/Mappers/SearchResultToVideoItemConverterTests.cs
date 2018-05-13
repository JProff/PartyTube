using AutoFixture;
using FluentAssertions;
using Google.Apis.YouTube.v3.Data;
using PartyTube.Model.Db;
using PartyTube.Model.Mappers;
using Xunit;

namespace PartyTube.ModelTests.Mappers
{
    public class SearchResultToVideoItemConverterTests
    {
        public SearchResultToVideoItemConverterTests()
        {
            _converter = new SearchResultToVideoItemConverter();
        }

        private readonly SearchResultToVideoItemConverter _converter;

        [Theory]
        [InlineData(true, true, true, true)]
        [InlineData(false, true, true, true)]
        [InlineData(false, false, true, true)]
        [InlineData(false, false, false, true)]
        [InlineData(false, false, false, false)]
        public void Should_Run_Without_Exceptions(bool isNullResourceId,
                                                  bool isNullSearchResultSnippet,
                                                  bool isNullThumbnailDetails,
                                                  bool isNullDefaultThumbnail)
        {
            var searchResult = new SearchResult
            {
                Id = isNullResourceId ? null : new ResourceId(),
                Snippet = isNullSearchResultSnippet ? null : new SearchResultSnippet()
            };
            if (searchResult.Snippet != null)
            {
                searchResult.Snippet.Thumbnails = isNullThumbnailDetails ? null : new ThumbnailDetails();
                if (searchResult.Snippet.Thumbnails != null)
                    searchResult.Snippet.Thumbnails.Default__ = isNullDefaultThumbnail ? null : new Thumbnail();
            }

            var actual = _converter.Convert(searchResult, null, null);

            Assert.NotNull(actual);
        }

        [Fact]
        public void Should_Return_Valid_Result()
        {
            var fixture = new Fixture();
            var expected = fixture.Build<VideoItem>().Without(item => item.PlaylistVideoItems).Create();
            var searchResult = new SearchResult
            {
                Id = new ResourceId
                {
                    VideoId = expected.VideoIdentifier
                },
                Snippet = new SearchResultSnippet
                {
                    Title = expected.Title,
                    Thumbnails = new ThumbnailDetails
                    {
                        Default__ = new Thumbnail
                        {
                            Url = expected.ThumbnailUrl
                        }
                    }
                }
            };

            var actual = _converter.Convert(searchResult, null, null);

            actual.Should()
                  .BeEquivalentTo(expected,
                                  options => options.Excluding(item => item.Id)
                                                    .Excluding(item => item.DurationInSeconds)
                                                    .Excluding(item => item.Seconds)
                                                    .Excluding(item => item.Minutes));
        }
    }
}