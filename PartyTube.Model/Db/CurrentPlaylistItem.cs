using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace PartyTube.Model.Db
{
    public class CurrentPlaylistItem
    {
        public CurrentPlaylistItem()
        {
            Video = new VideoItem();
        }

        public CurrentPlaylistItem([NotNull] VideoItem video, int order = 1)
        {
            Order = order;
            Video = video;
        }

        public int Id { get; set; }
        public int Order { get; set; }

        [NotNull]
        [Required]
        public VideoItem Video { get; set; }
    }
}