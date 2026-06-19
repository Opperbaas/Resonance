using System;
using System.Threading.Tasks;
using Resonance.BusinessLogicLayer.DTOs;
using Resonance.BusinessLogicLayer.Interfaces;
using Resonance.BusinessLogicLayer.Services;
using Resonance.DataAccessLayer.Models;
using Xunit;

namespace Resonance.Tests
{
    public class AuthServiceTests
    {
        [Fact]
        public async Task LoginAsync_ReturnsSuccess_WhenCredentialsAreValid()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                PasswordHash = "HASHED:password"
            };

            var unitOfWork = new FakeUnitOfWork
            {
                UserRepository = new FakeUserRepository(new[] { user })
            };
            var service = new AuthService(unitOfWork, new FakePasswordHasher(), new FakeEmailSender());

            var result = await service.LoginAsync(new LoginDto
            {
                Username = "testuser",
                Password = "password"
            });

            Assert.True(result.Success);
            Assert.Equal("Login successful.", result.Message);
            Assert.Equal(user.Id, result.UserId);
            Assert.False(string.IsNullOrWhiteSpace(result.Token));
        }

        [Fact]
        public async Task LoginAsync_ReturnsFailure_WhenUserDoesNotExist()
        {
            var unitOfWork = new FakeUnitOfWork
            {
                UserRepository = new FakeUserRepository(Array.Empty<User>())
            };
            var service = new AuthService(unitOfWork, new FakePasswordHasher(), new FakeEmailSender());

            var result = await service.LoginAsync(new LoginDto
            {
                Username = "unknown",
                Password = "password"
            });

            Assert.False(result.Success);
            Assert.Equal("User not found.", result.Message);
            Assert.Null(result.UserId);
            Assert.Equal(string.Empty, result.Token);
        }

        [Fact]
        public async Task LoginAsync_ReturnsFailure_WhenPasswordIsInvalid()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                PasswordHash = "HASHED:correct"
            };

            var unitOfWork = new FakeUnitOfWork
            {
                UserRepository = new FakeUserRepository(new[] { user })
            };
            var service = new AuthService(unitOfWork, new FakePasswordHasher(), new FakeEmailSender());

            var result = await service.LoginAsync(new LoginDto
            {
                Username = "testuser",
                Password = "wrong"
            });

            Assert.False(result.Success);
            Assert.Equal("Wrong username or password.", result.Message);
            Assert.Null(result.UserId);
            Assert.Equal(string.Empty, result.Token);
        }

        [Fact]
        public async Task LoginAsync_ReturnsFailure_WhenCredentialsAreMissing()
        {
            var unitOfWork = new FakeUnitOfWork
            {
                UserRepository = new FakeUserRepository(Array.Empty<User>())
            };
            var service = new AuthService(unitOfWork, new FakePasswordHasher(), new FakeEmailSender());

            var result = await service.LoginAsync(new LoginDto
            {
                Username = string.Empty,
                Password = string.Empty
            });

            Assert.False(result.Success);
            Assert.Equal("Invalid credentials.", result.Message);
            Assert.Null(result.UserId);
            Assert.Equal(string.Empty, result.Token);
        }
    }

    internal class FakePasswordHasher : IPasswordHasher
    {
        public string Hash(string password) => $"HASHED:{password}";
        public bool Verify(string hashedPassword, string providedPassword) => hashedPassword == Hash(providedPassword);
    }

    internal class FakeEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string to, string subject, string message) => Task.CompletedTask;
    }
}
