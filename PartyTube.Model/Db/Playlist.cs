using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace PartyTube.Model.Db
{
    public class Playlist
    {
        public Playlist()
        {
            PlaylistVideoItems = new List<PlaylistVideoItem>();
        }

        public int Id { get; set; }

        [NotNull]
        [Required]
        public string PlaylistName { get; set; }

        [NotNull]
        public List<PlaylistVideoItem> PlaylistVideoItems { get; set; }
    }
}