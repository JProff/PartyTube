using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using PartyTube.DataAccess;
using PartyTube.Model;
using PartyTube.Model.Db;

namespace PartyTube.Repository.Interfaces
{
    public interface IHistoryRepository
    {
        [NotNull]
        [ItemNotNull]
        Task<IEnumerable<SearchPopupResult>> GetSearchPopupResultsAsync([CanBeNull] string searchTerm);

        [NotNull]
        [ItemNotNull]
        Task<IEnumerable<LocalSearchVideoItem>> SearchAsync([CanBeNull] string searchTerm);

        /// <exception cref="ArgumentNullException">If <paramref name="videoItem" /> is null</exception>
        [NotNull]
        [ItemNotNull]
        Task<HistoryItem> AddAsync([NotNull] VideoItem videoItem);

        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="skip" /> less than zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="take" /> less or equal to zero.</exception>
        [NotNull]
        [ItemNotNull]
        Task<HistoryItem[]> GetAllAsync(int skip, int take);

        [NotNull]
        Task DeleteByIdAsync(int id);

        [NotNull]
        Task DeleteByVideoIdentifierAsync([CanBeNull] string videoIdentifier);

        /// <exception cref="ArgumentNullException">If <paramref name="videoItem" /> is null</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="context" /> is null</exception>
        [NotNull]
        [ItemNotNull]
        Task<HistoryItem> AddAsync([NotNull] VideoItem videoItem, [NotNull] PartyTubeDbContext context);
    }
}