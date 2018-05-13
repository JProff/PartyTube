using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using PartyTube.DataAccess;
using PartyTube.Model;
using PartyTube.Model.Db;
using PartyTube.Repository.Interfaces;

namespace PartyTube.Repository
{
    public class CurrentPlaylistRepository : ICurrentPlaylistRepository
    {
        [NotNull] private readonly PartyTubeDbContext _context;
        [NotNull] private readonly IVideoRepository _videoRepository;

        public CurrentPlaylistRepository([NotNull] PartyTubeDbContext context,
                                         [NotNull] IVideoRepository videoRepository)
        {
            _context = context;
            _videoRepository = videoRepository;
        }

        /// <exception cref="ArgumentNullException">If <paramref name="ids" /> is null</exception>
        [NotNull]
        public async Task ReorderAsync([NotNull] int[] ids)
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));

            var items = await _context.CurrentPlaylist.ToDictionaryAsync(item => item.Id, item => item)
                                      .ConfigureAwait(false);
            var i = 1;
            foreach (var id in ids)
            {
                if (!items.ContainsKey(id)) continue;
                items[id].Order = i;
                i++;
            }

            var notInRange = items.Where(w => !ids.Contains(w.Key))
                                  .Select(s => s.Value)
                                  .OrderBy(o => o.Order)
                                  .ToArray();

            foreach (var item in notInRange)
            {
                item.Order = i;
                i++;
            }

            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        [NotNull]
        [ItemNotNull]
        public async Task<CurrentPlaylistVideos> GetAllAsync()
        {
            var items = await _context.CurrentPlaylist.AsNoTracking()
                                      .Include(i => i.Video)
                                      .OrderBy(o => o.Order)
                                      .ToArrayAsync()
                                      .ConfigureAwait(false);
            return new CurrentPlaylistVideos(items);
        }

        /// <exception cref="ArgumentNullException">If <paramref name="videoItem" /> is null</exception>
        [NotNull]
        [ItemNotNull]
        public async Task<CurrentPlaylistItem> AddAsync([NotNull] VideoItem videoItem)
        {
            if (videoItem == null) throw new ArgumentNullException(nameof(videoItem));

            videoItem = await _videoRepository.GetAttachedOfFoundedAsync(videoItem, _context).ConfigureAwait(false);

            var maxOrder = await _context.CurrentPlaylist.AsNoTracking()
                                         .Select(s => s.Order)
                                         .DefaultIfEmpty(0)
                                         .MaxAsync()
                                         .ConfigureAwait(false);
            var playlistItem = new CurrentPlaylistItem(videoItem, maxOrder + 1);
            await _context.CurrentPlaylist.AddAsync(playlistItem).ConfigureAwait(false);
            _context.SaveChanges();

            return playlistItem;
        }

        /// <exception cref="ArgumentNullException">If <paramref name="videoItem" /> is null</exception>
        [NotNull]
        [ItemNotNull]
        public async Task<CurrentPlaylistItem> AddToStartAsync([NotNull] VideoItem videoItem)
        {
            if (videoItem == null) throw new ArgumentNullException(nameof(videoItem));

            var newOrder = _context.CurrentPlaylist.ToArray();
            foreach (var item in newOrder)
            {
                item.Order++;
            }

            _context.CurrentPlaylist.UpdateRange(newOrder);
            videoItem = await _videoRepository.GetAttachedOfFoundedAsync(videoItem, _context).ConfigureAwait(false);
            var currentPlaylistItem = new CurrentPlaylistItem(videoItem);
            await _context.CurrentPlaylist.AddAsync(currentPlaylistItem).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return currentPlaylistItem;
        }

        [NotNull]
        public Task RemoveAsync(int id)
        {
            return RemovePrivateAsync(id, _context, true);
        }

        /// <exception cref="ArgumentNullException">If <paramref name="context" /> is null</exception>
        [NotNull]
        public Task RemoveAsync(int id, [NotNull] PartyTubeDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            return RemovePrivateAsync(id, context, false);
        }

        [NotNull]
        [ItemCanBeNull]
        public Task<CurrentPlaylistItem> GetFirstAsync()
        {
            return _context.CurrentPlaylist.AsNoTracking()
                           .OrderBy(o => o.Order)
                           .Include(i => i.Video)
                           .FirstOrDefaultAsync();
        }

        [NotNull]
        public Task ClearAsync()
        {
            _context.CurrentPlaylist.RemoveRange(_context.CurrentPlaylist);
            return _context.SaveChangesAsync();
        }

        [NotNull]
        private async Task RemovePrivateAsync(int id, [NotNull] PartyTubeDbContext context, bool isSave)
        {
            var toRemove = await context.CurrentPlaylist.FirstOrDefaultAsync(s => s.Id == id).ConfigureAwait(false);
            if (toRemove == null)
                return;

            var reorder = await context.CurrentPlaylist.Where(w => w.Order > toRemove.Order)
                                       .ToArrayAsync()
                                       .ConfigureAwait(false);
            foreach (var item in reorder)
            {
                item.Order--;
            }

            context.CurrentPlaylist.Remove(toRemove);
            if (isSave)
                await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}