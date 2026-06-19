using System;
using System.Threading.Tasks;
using Resonance.BusinessLogicLayer.DTOs;
using Resonance.BusinessLogicLayer.Interfaces;
using Resonance.DataAccessLayer.Interfaces;
using Resonance.DataAccessLayer.Models;

namespace Resonance.BusinessLogicLayer.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUnitOfWork _uow;

        public ProfileService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ProfileDto?> GetProfileAsync(Guid userId)
        {
            var user = await _uow.UserRepository.GetByIdAsync(userId);
            if (user == null)
                return null;

            var profile = await _uow.ProfileRepository.GetByUserIdAsync(userId);

            return new ProfileDto
            {
                Username = user.Username,
                Email = user.Email,
                PreferredLocale = profile?.PreferredLocale ?? "en",
                TimeZone = profile?.TimeZone ?? "UTC",
                PrivacyLevel = profile?.PrivacyLevel ?? PrivacyLevel.Private
            };
        }

        public async Task<bool> IsUsernameTakenAsync(string username, Guid excludeUserId)
        {
            var user = await _uow.UserRepository.GetByUsernameAsync(username);
            return user != null && user.Id != excludeUserId;
        }

        public async Task<bool> UpdateUsernameAsync(Guid userId, string newUsername)
        {
            var user = await _uow.UserRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            user.Username = newUsername;
            _uow.UserRepository.Update(user);
            await _uow.SaveChangesAsync();
            return true;
        }
    }
}
