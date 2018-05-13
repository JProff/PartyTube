using AutoMapper;
using Google.Apis.YouTube.v3.Data;
using PartyTube.Model.Db;

namespace PartyTube.Model.Mappers
{
    public class ModelProfile : Profile
    {
        public ModelProfile()
        {
            CreateMap<Video, VideoItem>().ConvertUsing<VideoToVideoItemConverter>();
            CreateMap<SearchResult, VideoItem>().ConvertUsing<SearchResultToVideoItemConverter>();
        }
    }
}