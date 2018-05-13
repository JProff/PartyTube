using System;
using System.Xml;
using AutoMapper;
using Google.Apis.YouTube.v3.Data;
using PartyTube.Model.Db;

namespace PartyTube.ServiceTests.Mappers
{
    public class VideoItemToVideoConverter : ITypeConverter<VideoItem, Video>
    {
        #region Implementation of ITypeConverter<in VideoItem,Video>

        public Video Convert(VideoItem source, Video destination, ResolutionContext context)
        {
            const string youtubeVideoKind = "youtube#video";

            if (destination == null)
                destination = new Video();

            destination.Id = source.VideoIdentifier;
            destination.Kind = youtubeVideoKind;
            destination.Snippet = new VideoSnippet
            {
                Title = source.Title,
                Thumbnails = new ThumbnailDetails
                {
                    Default__ = new Thumbnail
                    {
                        Url = source.ThumbnailUrl
                    }
                }
            };
            destination.ContentDetails = new VideoContentDetails
            {
                Duration = XmlConvert.ToString(TimeSpan.FromSeconds(source.DurationInSeconds))
            };

            return destination;
        }

        #endregion
    }
}