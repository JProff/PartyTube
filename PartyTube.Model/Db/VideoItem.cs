using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;

namespace PartyTube.Model.Db
{
    public class VideoItem
    {
        public VideoItem()
        {
            PlaylistVideoItems = new List<PlaylistVideoItem>();
            VideoIdentifier = string.Empty;
            Title = string.Empty;
            ThumbnailUrl = string.Empty;
        }

        public VideoItem([NotNull] string videoIdentifier,
                         [NotNull] string title,
                         [NotNull] string thumbnailUrl,
                         int durationInSeconds) : this()
        {
            VideoIdentifier = videoIdentifier;
            Title = title;
            ThumbnailUrl = thumbnailUrl;
            DurationInSeconds = durationInSeconds;
        }

        public VideoItem(int id,
                         [NotNull] string videoIdentifier,
                         [NotNull] string title,
                         [NotNull] string thumbnailUrl,
                         int durationInSeconds) : this(videoIdentifier, title, thumbnailUrl, durationInSeconds)
        {
            Id = id;
        }

        public int Id { get; set; }

        [NotNull]
        [Required]
        public string VideoIdentifier { get; set; }

        [NotNull]
        [Required]
        public string Title { get; set; }

        [NotNull]
        public string ThumbnailUrl { get; set; }

        public int DurationInSeconds { get; set; }

        [NotMapped]
        public int Minutes => (int) TimeSpan.FromSeconds(DurationInSeconds).TotalMinutes;

        [NotMapped]
        public int Seconds => TimeSpan.FromSeconds(DurationInSeconds).Seconds;

        [NotNull]
        public List<PlaylistVideoItem> PlaylistVideoItems { get; set; }
    }
}