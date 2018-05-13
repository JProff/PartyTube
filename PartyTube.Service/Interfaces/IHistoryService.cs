using System.Threading.Tasks;
using JetBrains.Annotations;
using PartyTube.Model.Db;

namespace PartyTube.Service.Interfaces
{
    public interface IHistoryService
    {
        [NotNull]
        [ItemNotNull]
        Task<HistoryItem[]> GetAllAsync(int skip, int take);

        [NotNull]
        Task DeleteByIdAsync(int id);

        [NotNull]
        Task DeleteByVideoIdentifierAsync([CanBeNull] string videoIdentifier);
    }
}