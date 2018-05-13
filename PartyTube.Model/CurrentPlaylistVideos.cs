using System;
using System.Linq;
using JetBrains.Annotations;
using PartyTube.Model.Db;

namespace PartyTube.Model
{
    public class CurrentPlaylistVideos
    {
        public CurrentPlaylistVideos()
        {
            CurrentPlaylistItems = new CurrentPlaylistItem[0];
        }

        public CurrentPlaylistVideos([NotNull] CurrentPlaylistItem[] currentPlaylistItems)
        {
            CurrentPlaylistItems = currentPlaylistItems;
        }

        [NotNull]
        public CurrentPlaylistItem[] CurrentPlaylistItems { get; set; }

        public int TotalMinutes =>
            (int) TimeSpan.FromSeconds(CurrentPlaylistItems.Sum(s => s.Video.DurationInSeconds)).TotalMinutes;

        public int TotalSeconds =>
            TimeSpan.FromSeconds(CurrentPlaylistItems.Sum(s => s.Video.DurationInSeconds)).Seconds;
    }
}