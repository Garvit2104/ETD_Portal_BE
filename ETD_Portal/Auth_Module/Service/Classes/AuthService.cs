using ETD_Portal.Auth_Module.DAL.Interfaces;
using ETD_Portal.Auth_Module.DTOs;
using ETD_Portal.Auth_Module.Service.Interfaces;
using ETD_Portal.Models;
using ETD_Portal.Shared.Helpers;
using System;
using BCrypt.Net;

namespace ETD_Portal.Auth_Module.Service.Classes
{
   
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repo;
        private readonly JwtHelper _jwtHelper;
        private readonly IConfiguration _config;
       

        public AuthService(IAuthRepository repo,
                           JwtHelper jwtHelper,
                           IConfiguration config)
        {
            _repo = repo;
            _jwtHelper = jwtHelper;
            _config = config;
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO dto)
        {
            var user = await _repo.GetUserLoginAsync(dto.EmailAddress);

            if (!(user != null && BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)))
                throw new UnauthorizedAccessException("Invalid email or password.");

            var accessToken = _jwtHelper.GenerateAccessToken(user);
            var refreshToken = _jwtHelper.GenerateRefreshToken();

            await _repo.SaveRefreshTokenAsync(new RefreshToken
            {
                EmployeeId = user.EmployeeId,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(
                    int.Parse(_config["Jwt:RefreshTokenExpiryDays"]))
            });

            return new LoginResponseDTO
            {
                EmployeeId = user.EmployeeId,
                FullName = $"{user.FirstName} {user.LastName}",
                Role = user.Role,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(
                    int.Parse(_config["Jwt:TokenExpiryMinutes"]))
            };
        }

        public async Task<LoginResponseDTO> RefreshTokenAsync(RefreshTokenRequestDTO dto)
        {
            var existing = await _repo.GetRefreshTokenAsync(dto.RefreshToken);

            if (existing == null || existing.EmployeeId != dto.EmployeeId)
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");

            // Revoke old token — single use (rotation)
            await _repo.RevokeRefreshTokenAsync(dto.RefreshToken);

            var user = await _repo.GetUserByEmployeeIdJwtClaims(
                // fetch user by EmployeeId — add this method to repo if needed
                existing.EmployeeId);

            var newAccessToken = _jwtHelper.GenerateAccessToken(user);
            var newRefreshToken = _jwtHelper.GenerateRefreshToken();

            await _repo.SaveRefreshTokenAsync(new RefreshToken
            {
                EmployeeId = user.EmployeeId,
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(
                    int.Parse(_config["Jwt:RefreshTokenExpiryDays"]))
            });

            return new LoginResponseDTO
            {
                EmployeeId = user.EmployeeId,
                FullName = $"{user.FirstName} {user.LastName}",
                Role = user.Role,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(
                    int.Parse(_config["Jwt:TokenExpiryMinutes"]))
            };
        }

        public async Task LogoutAsync(int employeeId)
        {
            // Revokes all active refresh tokens for this user
            await _repo.RevokeAllUserTokensAsync(employeeId);
        }

        public async Task ChangePasswordAsync(int employeeId, ChangePasswordRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NewPassword) || dto.NewPassword.Length < 6)
                throw new ArgumentException("New password must be at least 6 characters.");

            var user = await _repo.GetUserByEmployeeIdJwtClaims(employeeId)
                       ?? throw new KeyNotFoundException("User not found.");

            // Verify the OLD password before allowing change
            if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash))
                throw new UnauthorizedAccessException("Old password is incorrect.");

            // Hash and save the NEW password
            var newHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _repo.UpdatePasswordHashAsync(employeeId, newHash);
        }
    }
}
