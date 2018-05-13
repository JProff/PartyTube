using JetBrains.Annotations;
using PartyTube.Model.Db;

namespace PartyTube.Model
{
    public class YoutubeSearchResult
    {
        public YoutubeSearchResult()
        {
            Videos = new VideoItem[0];
            Total = 0;
            NextPageToken = string.Empty;
        }

        public YoutubeSearchResult([NotNull] VideoItem[] videos, int total, [NotNull] string nextPageToken)
        {
            Videos = videos;
            Total = total;
            NextPageToken = nextPageToken;
        }

        [NotNull]
        public VideoItem[] Videos { get; set; }

        public int Total { get; set; }

        [NotNull]
        public string NextPageToken { get; set; }
    }
}