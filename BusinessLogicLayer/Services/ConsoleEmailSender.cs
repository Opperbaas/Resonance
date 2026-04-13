using System;
using System.Threading.Tasks;
using Resonance.BusinessLogicLayer.Interfaces;

namespace Resonance.BusinessLogicLayer.Services
{
    public class ConsoleEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string to, string subject, string message)
        {
            Console.WriteLine("--- Email sent ---");
            Console.WriteLine($"To: {to}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine(message);
            Console.WriteLine("-------------------");
            return Task.CompletedTask;
        }
    }
}
