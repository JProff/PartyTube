using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using PartyTube.DataAccess;
using PartyTube.Model.Db;

namespace PartyTube.Web
{
    public class PartyTubeDbContextSeedData
    {
        private static readonly VideoItem[] SeedVideoItems =
        {
            new VideoItem("yFE6qQ3ySXE",
                          "Lily Allen | Fuck You (Official Video - Explicit Version)",
                          "https://i.ytimg.com/vi/yFE6qQ3ySXE/default.jpg",
                          217) {Id = 1},
            new VideoItem("t99KH0TR-J4",
                          "Queen - The Show Must Go On (Official Video)",
                          "https://i.ytimg.com/vi/t99KH0TR-J4/default.jpg",
                          263) {Id = 2},
            new VideoItem("37eEUsd1ASA",
                          "DATA - Don\'t Sing feat. Benny Sings (official video)",
                          "https://i.ytimg.com/vi/37eEUsd1ASA/default.jpg",
                          232) {Id = 3},
            new VideoItem("UbQgXeY_zi4",
                          "Caravan Palace - Lone Digger",
                          "https://i.ytimg.com/vi/UbQgXeY_zi4/default.jpg",
                          171) {Id = 4}
        };

        private static readonly HistoryItem[] SeedHistories =
        {
            new HistoryItem(SeedVideoItems[0], new DateTime(2000, 1, 1).AddHours(20)),
            new HistoryItem(SeedVideoItems[1], new DateTime(2000, 1, 1).AddHours(22)),
            new HistoryItem(SeedVideoItems[2], new DateTime(2001, 1, 1).AddHours(22)),
            new HistoryItem(SeedVideoItems[3], new DateTime(2002, 1, 1).AddHours(22)),
            new HistoryItem(SeedVideoItems[0], new DateTime(2003, 1, 1).AddHours(22))
        };

        private readonly PartyTubeDbContext _context;
        private readonly IHostingEnvironment _env;

        public PartyTubeDbContextSeedData(PartyTubeDbContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }

        private static IEnumerable<HistoryItem> GetFakeHistories(int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new HistoryItem(new VideoItem
                                             {
                                                 Id = 100 + i,
                                                 DurationInSeconds = 100,
                                                 ThumbnailUrl = "https://i.ytimg.com/vi/UbQgXeY_zi4/default.jpg",
                                                 Title = $"Title {i}",
                                                 VideoIdentifier = $"VideoIdentifier-{i}"
                                             },
                                             new DateTime(1999, 1, 1));
            }
        }

        public void SeedData()
        {
            _context.Database.Migrate();

            if (_env.IsProduction())
            {
                return;
            }

            _context.Video.RemoveRange(_context.Video);
            _context.SaveChanges();

            _context.Video.AddRange(SeedVideoItems);
            _context.SaveChanges();

            _context.History.AddRange(SeedHistories);
            _context.SaveChanges();

            _context.History.AddRange(GetFakeHistories(1000));
            _context.SaveChanges();

            for (var j = 0; j < 10; j++)
            {
                for (var i = 0; i < SeedVideoItems.Length; i++)
                {
                    var vi = SeedVideoItems[i];
                    _context.CurrentPlaylist.Add(
                        new CurrentPlaylistItem(vi, SeedVideoItems.Length - i + SeedVideoItems.Length * j));
                }
            }

            _context.SaveChanges();

            _context.Player.Video = SeedVideoItems[2];
            _context.SaveChanges();
        }
    }
}