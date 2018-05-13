using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using PartyTube.DataAccess;
using PartyTube.Model;
using PartyTube.Model.Db;

namespace PartyTube.Repository.Interfaces
{
    public interface ICurrentPlaylistRepository
    {
        /// <exception cref="ArgumentNullException">If <paramref name="videoItem" /> is null</exception>
        [NotNull]
        [ItemNotNull]
        Task<CurrentPlaylistItem> AddAsync([NotNull] VideoItem videoItem);

        /// <exception cref="ArgumentNullException">If <paramref name="videoItem" /> is null</exception>
        [NotNull]
        [ItemNotNull]
        Task<CurrentPlaylistItem> AddToStartAsync([NotNull] VideoItem videoItem);

        [NotNull]
        [ItemNotNull]
        Task<CurrentPlaylistVideos> GetAllAsync();

        [NotNull]
        Task RemoveAsync(int id);

        /// <exception cref="ArgumentNullException">If <paramref name="ids" /> is null</exception>
        [NotNull]
        Task ReorderAsync([NotNull] int[] ids);

        [NotNull]
        [ItemCanBeNull]
        Task<CurrentPlaylistItem> GetFirstAsync();

        /// <exception cref="ArgumentNullException">If <paramref name="context" /> is null</exception>
        [NotNull]
        Task RemoveAsync(int id, [NotNull] PartyTubeDbContext context);

        [NotNull]
        Task ClearAsync();
    }
}