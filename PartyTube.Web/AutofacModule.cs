using System.Net.Http;
using Autofac;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using PartyTube.Repository;
using PartyTube.Repository.Interfaces;
using PartyTube.Service;
using PartyTube.Service.Helpers;
using PartyTube.Service.Interfaces;
using PartyTube.Web.Hubs;

namespace PartyTube.Web
{
    public class AutofacModule : Module
    {
        private readonly string _applicationName;
        private readonly string _youtubeApiKey;

        public AutofacModule(string youtubeApiKey, string applicationName)
        {
            _youtubeApiKey = youtubeApiKey;
            _applicationName = applicationName;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HttpClient>().AsSelf().InstancePerLifetimeScope();

            builder.RegisterType<Broadcaster>().As<IBroadcaster>().InstancePerLifetimeScope();
            builder.RegisterType<NowPlayingRepository>().As<INowPlayingRepository>().InstancePerLifetimeScope();
            builder.RegisterType<PlayerService>().As<IPlayerService>().InstancePerLifetimeScope();
            builder.RegisterType<HistoryRepository>().As<IHistoryRepository>().InstancePerLifetimeScope();
            builder.RegisterType<HistoryService>().As<IHistoryService>().InstancePerLifetimeScope();
            builder.RegisterType<CurrentPlaylistService>().As<ICurrentPlaylistService>().InstancePerLifetimeScope();
            builder.RegisterType<CurrentPlaylistRepository>()
                   .As<ICurrentPlaylistRepository>()
                   .InstancePerLifetimeScope();
            builder.RegisterType<VideoRepository>().As<IVideoRepository>().InstancePerLifetimeScope();
            builder.RegisterType<YoutubeSearchService>().As<IYoutubeSearchService>().InstancePerLifetimeScope();
            builder.RegisterType<SearchService>().As<ISearchService>().InstancePerLifetimeScope();
            builder.RegisterType<YoutubeHelpers>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<PartyTubeDbContextSeedData>().AsSelf().InstancePerLifetimeScope();

            builder.Register(c => new YouTubeService(new BaseClientService.Initializer
                    {
                        ApiKey = _youtubeApiKey,
                        ApplicationName = _applicationName
                    }))
                   .As<YouTubeService>()
                   .InstancePerLifetimeScope();
            builder.RegisterType<YouTubeServiceWrapper>().AsSelf().InstancePerLifetimeScope();
        }
    }
}