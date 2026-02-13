using System.Threading.Tasks;
using Resonance.BusinessLogicLayer.DTOs;

namespace Resonance.BusinessLogicLayer.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResultDto> LoginAsync(LoginDto dto);
        Task<RegisterResultDto> RegisterAsync(RegisterDto dto);
    }
}