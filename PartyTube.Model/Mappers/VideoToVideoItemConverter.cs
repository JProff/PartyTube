using System.Xml;
using AutoMapper;
using Google.Apis.YouTube.v3.Data;
using PartyTube.Model.Db;

namespace PartyTube.Model.Mappers
{
    public class VideoToVideoItemConverter : ITypeConverter<Video, VideoItem>
    {
        #region Implementation of ITypeConverter<in Video,VideoItem>

        public VideoItem Convert(Video source, VideoItem destination, ResolutionContext context)
        {
            if (destination == null)
                destination = new VideoItem();

            destination.VideoIdentifier = source.Id;

            if (source.Snippet != null)
            {
                destination.Title = source.Snippet.Title;
                if (source.Snippet.Thumbnails?.Default__ != null)
                    destination.ThumbnailUrl = source.Snippet.Thumbnails.Default__.Url;
            }

            if (!string.IsNullOrWhiteSpace(source.ContentDetails?.Duration))
                destination.DurationInSeconds =
                    (int) XmlConvert.ToTimeSpan(source.ContentDetails.Duration).TotalSeconds;

            return destination;
        }

        #endregion
    }
}