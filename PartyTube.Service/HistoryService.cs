using System.Threading.Tasks;
using JetBrains.Annotations;
using PartyTube.Model.Db;
using PartyTube.Repository.Interfaces;
using PartyTube.Service.Interfaces;

namespace PartyTube.Service
{
    public class HistoryService : IHistoryService
    {
        [NotNull] private readonly IHistoryRepository _historyRepository;

        public HistoryService([NotNull] IHistoryRepository historyRepository)
        {
            _historyRepository = historyRepository;
        }

        [NotNull]
        [ItemNotNull]
        public Task<HistoryItem[]> GetAllAsync(int skip, int take)
        {
            return _historyRepository.GetAllAsync(skip, take);
        }

        [NotNull]
        public Task DeleteByIdAsync(int id)
        {
            return _historyRepository.DeleteByIdAsync(id);
        }

        [NotNull]
        public Task DeleteByVideoIdentifierAsync([CanBeNull] string videoIdentifier)
        {
            return _historyRepository.DeleteByVideoIdentifierAsync(videoIdentifier);
        }
    }
}