using JetBrains.Annotations;

namespace PartyTube.Model.Settings
{
    public class AppSettings
    {
        public AppSettings()
        {
            SearchSettings = new SearchSettings();
            ApplicationName = string.Empty;
            YoutubeApiKey = string.Empty;
        }

        [NotNull]
        public SearchSettings SearchSettings { get; set; }

        [NotNull]
        public string ApplicationName { get; set; }

        [NotNull]
        public string YoutubeApiKey { get; set; }
    }
}