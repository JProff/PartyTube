using AutoMapper;
using Google.Apis.YouTube.v3.Data;
using PartyTube.Model.Db;

namespace PartyTube.ServiceTests.Mappers
{
    public class ServiceTestsProfile : Profile
    {
        public ServiceTestsProfile()
        {
            CreateMap<VideoItem, SearchResult>().ConvertUsing<VideoItemToSearchResultConverter>();
            CreateMap<VideoItem, Video>().ConvertUsing<VideoItemToVideoConverter>();
        }
    }
}