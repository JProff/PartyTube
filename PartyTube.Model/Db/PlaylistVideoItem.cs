using JetBrains.Annotations;

namespace PartyTube.Model.Db
{
    /* Example of use

 var videos = context.Playlist
       .Where(p => p.Id == playlistId)
       .SelectMany(p => p.PlaylistVideoItems);
       .Select(pv => pv.VideoItem);

     */
    public class PlaylistVideoItem
    {
        public PlaylistVideoItem()
        {
            Playlist = new Playlist();
            VideoItem = new VideoItem();
        }

        public int PlaylistId { get; set; }

        [NotNull]
        public Playlist Playlist { get; set; }

        public int VideoItemId { get; set; }

        [NotNull]
        public VideoItem VideoItem { get; set; }
    }
}