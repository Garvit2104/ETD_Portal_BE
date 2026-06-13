using ETD_Portal.Auth_Module.DAL.Interfaces;
using ETD_Portal.Data;
using ETD_Portal.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Auth_Module.DAL.Classes
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ETDPortalDbContext _context;

        public AuthRepository(ETDPortalDbContext _context)
        {
            this._context = _context;   
        }

        public async Task<User?> GetUserLoginAsync(string email)
        {
           return await _context.Users.FirstOrDefaultAsync(u => u.EmailAddress == email);

        }

        public async Task<User?> GetUserByEmployeeIdJwtClaims(int employeeId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.EmployeeId == employeeId);
        }

        public async Task SaveRefreshTokenAsync(RefreshToken token)
        {
            await _context.RefreshTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        => await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token
                                   && !r.IsRevoked
                                   && r.ExpiresAt > DateTime.UtcNow);

        public async Task RevokeRefreshTokenAsync(string token)
        {
            var existing = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token);
            if (existing != null)
            {
                existing.IsRevoked = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RevokeAllUserTokensAsync(int employeeId)
        {
            var tokens = await _context.RefreshTokens.Where
                                      (r => r.EmployeeId == employeeId && !r.IsRevoked).ToListAsync();
            tokens.ForEach(t => t.IsRevoked = true);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePasswordHashAsync(int employeeId, object newPasswordHash)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.EmployeeId == employeeId);

            if (user == null)
                throw new KeyNotFoundException($"User {employeeId} not found.");

            user.PasswordHash = (string?)newPasswordHash;
            await _context.SaveChangesAsync();
        }


    }
}
