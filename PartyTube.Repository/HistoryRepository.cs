using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using PartyTube.DataAccess;
using PartyTube.Model;
using PartyTube.Model.Db;
using PartyTube.Model.Settings;
using PartyTube.Repository.Interfaces;

namespace PartyTube.Repository
{
    public class HistoryRepository : IHistoryRepository
    {
        private readonly AppSettings _appSettings;
        [NotNull] private readonly PartyTubeDbContext _context;
        [NotNull] private readonly IVideoRepository _videoRepository;

        public HistoryRepository([NotNull] PartyTubeDbContext context,
                                 [NotNull] AppSettings appSettings,
                                 [NotNull] IVideoRepository videoRepository)
        {
            _context = context;
            _appSettings = appSettings;
            _videoRepository = videoRepository;
        }

        [NotNull]
        [ItemNotNull]
        public async Task<IEnumerable<SearchPopupResult>> GetSearchPopupResultsAsync([CanBeNull] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return Enumerable.Empty<SearchPopupResult>();

            var result = await _context.History
                                       .AsNoTracking()
                                       .GroupBy(g => g.Video.Title)
                                       .Where(w => EF.Functions.Like(w.Key, $"%{searchTerm}%"))
                                       .OrderByDescending(o => o.Count())
                                       .ThenBy(o => o.Key)
                                       .Take(_appSettings.SearchSettings.PopupLocalMaxResultsCount)
                                       .Select(s => new SearchPopupResult(s.Key, true, s.Count()))
                                       .ToListAsync()
                                       .ConfigureAwait(false);

            return result;
        }

        /// <exception cref="ArgumentNullException">If <paramref name="videoItem" /> is null</exception>
        [NotNull]
        [ItemNotNull]
        public Task<HistoryItem> AddAsync([NotNull] VideoItem videoItem)
        {
            if (videoItem == null) throw new ArgumentNullException(nameof(videoItem));

            return AddPrivateAsync(videoItem, _context, true);
        }

        /// <exception cref="ArgumentNullException">If <paramref name="videoItem" /> is null</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="context" /> is null</exception>
        [NotNull]
        [ItemNotNull]
        public Task<HistoryItem> AddAsync([NotNull] VideoItem videoItem, [NotNull] PartyTubeDbContext context)
        {
            if (videoItem == null) throw new ArgumentNullException(nameof(videoItem));
            if (context == null) throw new ArgumentNullException(nameof(context));

            return AddPrivateAsync(videoItem, context, false);
        }

        [NotNull]
        [ItemNotNull]
        public async Task<IEnumerable<LocalSearchVideoItem>> SearchAsync([CanBeNull] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return Enumerable.Empty<LocalSearchVideoItem>();

            var videoItems = await _context.History
                                           .AsNoTracking()
                                           .Where(w => EF.Functions.Like(w.Video.Title, $"%{searchTerm}%"))
                                           .GroupBy(g => g.Video.Id)
                                           .Select(s => new LocalSearchVideoItem(s.First().Video, s.Count()))
                                           .ToArrayAsync()
                                           .ConfigureAwait(false);
            var result = videoItems.OrderByDescending(o => o.Count)
                                   .ThenBy(o => o.VideoItem.Title)
                                   .ToArray();
            return result;
        }

        [NotNull]
        public async Task DeleteByIdAsync(int id)
        {
            var del = await _context.History.FirstOrDefaultAsync(s => s.Id == id).ConfigureAwait(false);
            if (del == null)
                return;

            _context.History.Remove(del);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        [NotNull]
        public async Task DeleteByVideoIdentifierAsync([CanBeNull] string videoIdentifier)
        {
            if (string.IsNullOrWhiteSpace(videoIdentifier))
                return;

            var del = await _context.History.Where(s => s.Video.VideoIdentifier == videoIdentifier)
                                    .ToArrayAsync()
                                    .ConfigureAwait(false);
            if (del.Length == 0)
                return;

            _context.History.RemoveRange(del);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="skip" /> less than zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="take" /> less or equal to zero.</exception>
        [NotNull]
        [ItemNotNull]
        public Task<HistoryItem[]> GetAllAsync(int skip, int take)
        {
            if (skip < 0)
                throw new ArgumentOutOfRangeException(nameof(skip), skip, "Must be greater than 0.");
            if (take <= 0)
                throw new ArgumentOutOfRangeException(nameof(take), take, "Must be greater than 1.");

            var result = _context.History.AsNoTracking()
                                 .OrderByDescending(o => o.PlayedDateTime)
                                 .Skip(skip)
                                 .Take(take)
                                 .Include(i => i.Video)
                                 .ToArrayAsync();
            return result;
        }

        [NotNull]
        [ItemNotNull]
        private async Task<HistoryItem> AddPrivateAsync([NotNull] VideoItem videoItem,
                                                        [NotNull] PartyTubeDbContext context,
                                                        bool isSave)
        {
            videoItem = await _videoRepository.GetAttachedOfFoundedAsync(videoItem, context).ConfigureAwait(false);

            var historyItem = new HistoryItem(videoItem);
            await context.History.AddAsync(historyItem).ConfigureAwait(false);

            if (isSave)
                await context.SaveChangesAsync().ConfigureAwait(false);

            return historyItem;
        }
    }
}