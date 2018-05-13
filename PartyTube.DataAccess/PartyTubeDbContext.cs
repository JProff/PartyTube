using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using PartyTube.Model.Db;

namespace PartyTube.DataAccess
{
    public class PartyTubeDbContext : DbContext
    {
        public PartyTubeDbContext([NotNull] DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<HistoryItem> History { get; set; }
        public virtual DbSet<CurrentPlaylistItem> CurrentPlaylist { get; set; }
        public virtual DbSet<Playlist> Playlist { get; set; }
        private DbSet<NowPlaying> NowPlaying { get; set; }

        [NotNull]
        public NowPlaying Player
        {
            get
            {
                var playing = NowPlaying.Include(nowPlaying => nowPlaying.Video).FirstOrDefault();
                if (playing != null) return playing;

                playing = new NowPlaying();
                NowPlaying.Add(playing);
                SaveChanges();
                return playing;
            }
        }

        public virtual DbSet<VideoItem> Video { get; set; }

        #region Overrides of DbContext

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<NowPlaying>()
                        .HasOne(np => np.Video)
                        .WithOne()
                        .HasForeignKey(typeof(NowPlaying).ToString(), "VideoId")
                        .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Playlist>().HasIndex(playlist => playlist.PlaylistName).IsUnique();
            modelBuilder.Entity<VideoItem>().HasIndex(item => item.VideoIdentifier).IsUnique();

            modelBuilder.Entity<PlaylistVideoItem>()
                        .HasKey(pv => new {pv.PlaylistId, pv.VideoItemId});

            modelBuilder.Entity<PlaylistVideoItem>()
                        .HasOne(pv => pv.Playlist)
                        .WithMany(p => p.PlaylistVideoItems)
                        .HasForeignKey(pv => pv.PlaylistId);

            modelBuilder.Entity<PlaylistVideoItem>()
                        .HasOne(pv => pv.VideoItem)
                        .WithMany(v => v.PlaylistVideoItems)
                        .HasForeignKey(pv => pv.PlaylistId);
        }

        #endregion
    }
}