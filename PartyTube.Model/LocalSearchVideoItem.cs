using JetBrains.Annotations;
using PartyTube.Model.Db;

namespace PartyTube.Model
{
    public class LocalSearchVideoItem
    {
        public LocalSearchVideoItem()
        {
            VideoItem = new VideoItem();
        }

        public LocalSearchVideoItem([NotNull] VideoItem videoItem, int count)
        {
            VideoItem = videoItem;
            Count = count;
        }

        [NotNull]
        public VideoItem VideoItem { get; set; }

        public int Count { get; set; }
    }
}