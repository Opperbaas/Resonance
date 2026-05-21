using System;
using System.Threading.Tasks;
using Resonance.BusinessLogicLayer.DTOs;

namespace Resonance.BusinessLogicLayer.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardAsync(Guid userId);
    }
}
