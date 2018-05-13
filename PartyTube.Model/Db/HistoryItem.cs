using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace PartyTube.Model.Db
{
    public class HistoryItem
    {
        public HistoryItem()
        {
            Video = new VideoItem();
        }

        public HistoryItem([NotNull] VideoItem video, DateTime playedDateTime)
        {
            PlayedDateTime = playedDateTime;
            Video = video;
        }

        public HistoryItem([NotNull] VideoItem video)
        {
            PlayedDateTime = DateTime.Now;
            Video = video;
        }

        public int Id { get; set; }
        public DateTime PlayedDateTime { get; set; }

        [NotNull]
        [Required]
        public VideoItem Video { get; set; }
    }
}