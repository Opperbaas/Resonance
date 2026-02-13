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

        public AuthService(IUnitOfWork uow, IPasswordHasher passwordHasher)
        {
            _uow = uow;
            _passwordHasher = passwordHasher;
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
            if (dto == null || string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                return new RegisterResultDto { Success = false, Message = "Invalid registration data." };

            var existingUser = await _uow.UserRepository.GetByUsernameAsync(dto.Username);
            if (existingUser != null)
                return new RegisterResultDto { Success = false, Message = "Username already exists." };

            var passwordHash = _passwordHasher.Hash(dto.Password);
            var user = new Resonance.DataAccessLayer.Models.User
            {
                Id = Guid.NewGuid(),
                Username = dto.Username,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.UserRepository.AddAsync(user);
            await _uow.SaveChangesAsync();

            return new RegisterResultDto { Success = true, Message = "Registration successful.", UserId = user.Id };
        }
    }
}