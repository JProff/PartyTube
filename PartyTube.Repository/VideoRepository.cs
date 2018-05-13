using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using PartyTube.DataAccess;
using PartyTube.Model.Db;
using PartyTube.Repository.Interfaces;

namespace PartyTube.Repository
{
    public class VideoRepository : IVideoRepository
    {
        [NotNull] private readonly PartyTubeDbContext _context;

        public VideoRepository([NotNull] PartyTubeDbContext context)
        {
            _context = context;
        }

        [NotNull]
        [ItemCanBeNull]
        public async Task<VideoItem> GetByIdentifierAsync([CanBeNull] string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                return await Task.FromResult<VideoItem>(null).ConfigureAwait(false);

            return await _context.Video.SingleOrDefaultAsync(item => item.VideoIdentifier == identifier)
                                 .ConfigureAwait(false);
        }

        /// <exception cref="ArgumentNullException">If <paramref name="videoItem" /> is null</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="context" /> is null</exception>
        [NotNull]
        [ItemNotNull]
        public async Task<VideoItem> GetAttachedOfFoundedAsync([NotNull] VideoItem videoItem,
                                                               [NotNull] PartyTubeDbContext context)
        {
            if (videoItem == null) throw new ArgumentNullException(nameof(videoItem));
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (videoItem.Id == 0)
            {
                var identifier = videoItem.VideoIdentifier;
                var search = await context.Video.FirstOrDefaultAsync(s => s.VideoIdentifier == identifier)
                                          .ConfigureAwait(false);
                if (search != null)
                    videoItem = search;
            }
            else if (context.Entry(videoItem).State == EntityState.Detached)
            {
                context.Video.Attach(videoItem);
            }

            return videoItem;
        }
    }
}