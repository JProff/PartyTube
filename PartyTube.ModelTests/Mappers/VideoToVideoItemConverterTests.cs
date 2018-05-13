using System;
using System.Xml;
using AutoFixture;
using FluentAssertions;
using Google.Apis.YouTube.v3.Data;
using PartyTube.Model.Db;
using PartyTube.Model.Mappers;
using Xunit;

namespace PartyTube.ModelTests.Mappers
{
    public class VideoToVideoItemConverterTests
    {
        public VideoToVideoItemConverterTests()
        {
            _converter = new VideoToVideoItemConverter();
        }

        private readonly VideoToVideoItemConverter _converter;

        [Theory]
        [InlineData(true, true, true, true)]
        [InlineData(false, true, true, true)]
        [InlineData(false, false, true, true)]
        [InlineData(false, false, false, true)]
        [InlineData(false, false, false, false)]
        public void Should_Run_Without_Exceptions(bool isNullSnippet,
                                                  bool isNullThumbnails,
                                                  bool isNullDefault,
                                                  bool isNullContentDetails)
        {
            var video = new Video
            {
                Snippet = isNullSnippet ? null : new VideoSnippet()
            };
            if (video.Snippet != null)
            {
                video.Snippet.Thumbnails = isNullThumbnails ? null : new ThumbnailDetails();
                if (video.Snippet.Thumbnails != null)
                    video.Snippet.Thumbnails.Default__ = isNullDefault ? null : new Thumbnail();
            }

            video.ContentDetails = isNullContentDetails ? null : new VideoContentDetails();

            var actual = _converter.Convert(video, null, null);

            Assert.NotNull(actual);
        }

        [Fact]
        public void Should_Return_Valid_Result()
        {
            var fixture = new Fixture();

            var expected = fixture.Build<VideoItem>().Without(item => item.PlaylistVideoItems).Create();
            var video = new Video
            {
                Id = expected.VideoIdentifier,
                Snippet = new VideoSnippet
                {
                    Title = expected.Title,
                    Thumbnails = new ThumbnailDetails
                    {
                        Default__ = new Thumbnail
                        {
                            Url = expected.ThumbnailUrl
                        }
                    }
                },
                ContentDetails = new VideoContentDetails
                {
                    Duration = XmlConvert.ToString(TimeSpan.FromSeconds(expected.DurationInSeconds))
                }
            };

            var actual = _converter.Convert(video, null, null);

            actual.Should().BeEquivalentTo(expected, options => options.Excluding(item => item.Id));
        }
    }
}