using AutoMapper;
using Google.Apis.YouTube.v3.Data;
using PartyTube.Model.Db;

namespace PartyTube.Model.Mappers
{
    public class SearchResultToVideoItemConverter : ITypeConverter<SearchResult, VideoItem>
    {
        #region Implementation of ITypeConverter<in SearchResult,VideoItem>

        public VideoItem Convert(SearchResult source, VideoItem destination, ResolutionContext context)
        {
            if (destination == null)
                destination = new VideoItem();

            if (source.Id != null)
            {
                destination.VideoIdentifier = source.Id.VideoId;
            }

            if (source.Snippet != null)
            {
                destination.Title = source.Snippet.Title;
                if (source.Snippet.Thumbnails?.Default__ != null)
                    destination.ThumbnailUrl = source.Snippet.Thumbnails.Default__.Url;
            }

            return destination;
        }

        #endregion
    }
}