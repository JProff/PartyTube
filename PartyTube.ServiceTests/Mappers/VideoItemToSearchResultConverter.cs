using AutoMapper;
using Google.Apis.YouTube.v3.Data;
using PartyTube.Model.Db;

namespace PartyTube.ServiceTests.Mappers
{
    public class VideoItemToSearchResultConverter : ITypeConverter<VideoItem, SearchResult>
    {
        #region Implementation of ITypeConverter<in VideoItem,SearchResult>

        public SearchResult Convert(VideoItem source, SearchResult destination, ResolutionContext context)
        {
            if (destination == null)
                destination = new SearchResult();

            destination.Id = new ResourceId
            {
                VideoId = source.VideoIdentifier
            };

            destination.Snippet = new SearchResultSnippet
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

            return destination;
        }

        #endregion
    }
}