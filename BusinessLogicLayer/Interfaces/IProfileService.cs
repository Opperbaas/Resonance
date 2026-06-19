using System;
using System.Threading.Tasks;
using Resonance.BusinessLogicLayer.DTOs;

namespace Resonance.BusinessLogicLayer.Interfaces
{
    public interface IProfileService
    {
        Task<ProfileDto?> GetProfileAsync(Guid userId);
        Task<bool> IsUsernameTakenAsync(string username, Guid excludeUserId);
        Task<bool> UpdateUsernameAsync(Guid userId, string newUsername);
    }
}
