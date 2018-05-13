using JetBrains.Annotations;

namespace PartyTube.Model
{
    public class SearchPopupResult
    {
        public SearchPopupResult()
        {
            VideoName = string.Empty;
        }

        public SearchPopupResult([NotNull] string videoName, bool isLocalHistory = false, int viewCount = 0)
        {
            IsLocalHistory = isLocalHistory;
            VideoName = videoName;
            ViewCount = viewCount;
        }

        public bool IsLocalHistory { get; set; }

        [NotNull]
        public string VideoName { get; set; }

        public int ViewCount { get; set; }
    }
}