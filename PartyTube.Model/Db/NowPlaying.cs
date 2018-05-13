using JetBrains.Annotations;

namespace PartyTube.Model.Db
{
    public class NowPlaying
    {
        public NowPlaying()
        {
            Video = null;
            IsPlaying = false;
        }

        public NowPlaying([CanBeNull] VideoItem video = null, bool isPlaying = false)
        {
            IsPlaying = isPlaying;
            Video = video;
        }

        public int Id { get; set; }

        public bool IsPlaying { get; set; }

        [CanBeNull]
        public VideoItem Video { get; set; }
    }
}