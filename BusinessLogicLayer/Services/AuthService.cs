using System;
using System.Threading.Tasks;
using Resonance.BusinessLogicLayer.DTOs;
using Resonance.BusinessLogicLayer.Interfaces;
using Resonance.DataAccessLayer.Interfaces;

namespace Resonance.BusinessLogicLayer.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _uow;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEmailSender _emailSender;

        public AuthService(IUnitOfWork uow, IPasswordHasher passwordHasher, IEmailSender emailSender)
        {
            _uow = uow;
            _passwordHasher = passwordHasher;
            _emailSender = emailSender;
        }

        public async Task<LoginResultDto> LoginAsync(LoginDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                return new LoginResultDto { Success = false, Message = "Invalid credentials." };

            var user = await _uow.UserRepository.GetByUsernameAsync(dto.Username);
            if (user == null)
                return new LoginResultDto { Success = false, Message = "User not found." };

            var valid = _passwordHasher.Verify(user.PasswordHash, dto.Password);
            if (!valid)
                return new LoginResultDto { Success = false, Message = "Wrong username or password." };

            // For example purposes return a simple GUID as token. Replace with JWT for production.
            var token = Guid.NewGuid().ToString();

            return new LoginResultDto { Success = true, Message = "Login successful.", Token = token, UserId = user.Id };
        }

        public async Task<RegisterResultDto> RegisterAsync(RegisterDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password) || string.IsNullOrWhiteSpace(dto.Email))
                return new RegisterResultDto { Success = false, Message = "Invalid registration data." };

            var existingUser = await _uow.UserRepository.GetByUsernameAsync(dto.Username);
            if (existingUser != null)
                return new RegisterResultDto { Success = false, Message = "Username already exists." };

            var existingEmail = await _uow.UserRepository.GetByEmailAsync(dto.Email);
            if (existingEmail != null)
                return new RegisterResultDto { Success = false, Message = "Email is already registered." };

            var passwordHash = _passwordHasher.Hash(dto.Password);
            var user = new Resonance.DataAccessLayer.Models.User
            {
                Id = Guid.NewGuid(),
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.UserRepository.AddAsync(user);
            await _uow.SaveChangesAsync();

            return new RegisterResultDto { Success = true, Message = "Registration successful.", UserId = user.Id };
        }

        public async Task<(bool Success, string Message)> RequestPasswordResetAsync(ForgotPasswordDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email))
                return (false, "Please enter a valid email address.");

            var user = await _uow.UserRepository.GetByEmailAsync(dto.Email);
            if (user == null)
                return (true, "If this email is registered, you will receive a reset link.");

            user.PasswordResetToken = Guid.NewGuid().ToString("N");
            user.PasswordResetTokenExpiresAt = DateTime.UtcNow.AddHours(1);
            _uow.UserRepository.Update(user);
            await _uow.SaveChangesAsync();

            var resetLink = string.IsNullOrWhiteSpace(dto.CallbackUrl)
                ? $"/reset-password?token={user.PasswordResetToken}"
                : dto.CallbackUrl.TrimEnd('/') + $"?token={user.PasswordResetToken}";

            await _emailSender.SendEmailAsync(user.Email,
                "Password reset request",
                $"Use this link to reset your password:\n\n{resetLink}\n\nThe link expires in one hour.");

            return (true, "If this email is registered, you will receive a reset link.");
        }

        public async Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Token) || string.IsNullOrWhiteSpace(dto.Password))
                return (false, "Invalid reset request.");

            if (dto.Password != dto.ConfirmPassword)
                return (false, "Passwords do not match.");

            var user = await _uow.UserRepository.GetByResetTokenAsync(dto.Token);
            if (user == null || !user.PasswordResetTokenExpiresAt.HasValue || user.PasswordResetTokenExpiresAt.Value < DateTime.UtcNow)
                return (false, "Reset link is invalid or has expired.");

            user.PasswordHash = _passwordHasher.Hash(dto.Password);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiresAt = null;
            _uow.UserRepository.Update(user);
            await _uow.SaveChangesAsync();

            return (true, "Your password has been reset. You can now log in.");
        }
    }
}