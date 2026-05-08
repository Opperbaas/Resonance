using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Resonance.BusinessLogicLayer.DTOs;
using Resonance.BusinessLogicLayer.Interfaces;

namespace Resonance.BusinessLogicLayer.Services
{
    public class MusicImportService : IMusicImportService
    {
        private readonly IEnumerable<IMusicImportProvider> _providers;

        public MusicImportService(IEnumerable<IMusicImportProvider> providers)
        {
            _providers = providers;
        }

        public IEnumerable<string> GetAvailableProviders()
        {
            return _providers.Select(provider => provider.ProviderName);
        }

        public IMusicImportProvider? GetProvider(string providerName)
        {
            return _providers.FirstOrDefault(provider => string.Equals(provider.ProviderName, providerName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<TrackDto>> ImportLibraryAsync(Guid userId, string providerName)
        {
            var provider = GetProvider(providerName);
            if (provider == null)
            {
                throw new ArgumentException($"Music provider '{providerName}' is not registered.", nameof(providerName));
            }

            return await provider.ImportUserLibraryAsync(userId);
        }

        public async Task<IEnumerable<PlayEventDto>> SyncRecentPlaysAsync(Guid userId, string providerName, DateTime since)
        {
            var provider = GetProvider(providerName);
            if (provider == null)
            {
                throw new ArgumentException($"Music provider '{providerName}' is not registered.", nameof(providerName));
            }

            return await provider.ImportRecentPlaysAsync(userId, since);
        }
    }
}
