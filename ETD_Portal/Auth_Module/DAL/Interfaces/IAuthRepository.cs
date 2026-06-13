using ETD_Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Auth_Module.DAL.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetUserLoginAsync(string email);
        Task<User?> GetUserByEmployeeIdJwtClaims(int employeeId);
        Task SaveRefreshTokenAsync(RefreshToken token);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task RevokeRefreshTokenAsync(string token);
        Task RevokeAllUserTokensAsync(int employeeId);

       
        Task UpdatePasswordHashAsync(int employeeId, object newHash);
    }
}
