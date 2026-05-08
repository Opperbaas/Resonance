using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Resonance.BusinessLogicLayer.DTOs;

namespace Resonance.BusinessLogicLayer.Interfaces
{
    public interface IMusicImportService
    {
        IEnumerable<string> GetAvailableProviders();
        IMusicImportProvider? GetProvider(string providerName);
        Task<IEnumerable<TrackDto>> ImportLibraryAsync(Guid userId, string providerName);
        Task<IEnumerable<PlayEventDto>> SyncRecentPlaysAsync(Guid userId, string providerName, DateTime since);
    }
}
