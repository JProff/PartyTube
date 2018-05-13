using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using PartyTube.DataAccess;
using PartyTube.Model.Db;

namespace PartyTube.Repository.Interfaces
{
    public interface IVideoRepository
    {
        [NotNull]
        [ItemCanBeNull]
        Task<VideoItem> GetByIdentifierAsync([CanBeNull] string identifier);

        /// <exception cref="ArgumentNullException">If <paramref name="videoItem" /> is null</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="context" /> is null</exception>
        [NotNull]
        [ItemNotNull]
        Task<VideoItem> GetAttachedOfFoundedAsync([NotNull] VideoItem videoItem,
                                                  [NotNull] PartyTubeDbContext context);
    }
}