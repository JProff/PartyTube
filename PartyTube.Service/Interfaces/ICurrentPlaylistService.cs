using System.Threading.Tasks;
using JetBrains.Annotations;
using PartyTube.Model;
using PartyTube.Model.Db;

namespace PartyTube.Service.Interfaces
{
    public interface ICurrentPlaylistService
    {
        [NotNull]
        [ItemCanBeNull]
        Task<CurrentPlaylistItem> AddVideoByIdOrUrlAsync([CanBeNull] string idOrUrl);

        [NotNull]
        [ItemNotNull]
        Task<CurrentPlaylistItem> AddAsync([NotNull] VideoItem videoItem);

        [NotNull]
        [ItemNotNull]
        Task<CurrentPlaylistItem> AddToStartAsync([NotNull] VideoItem videoItem);

        [NotNull]
        [ItemNotNull]
        Task<CurrentPlaylistVideos> GetAllAsync();

        [NotNull]
        Task RemoveAsync(int id);

        [NotNull]
        Task ReorderAsync([NotNull] int[] ids);

        [NotNull]
        Task ClearAsync();
    }
}