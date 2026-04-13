using System.Threading.Tasks;

namespace Resonance.BusinessLogicLayer.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string message);
    }
}
