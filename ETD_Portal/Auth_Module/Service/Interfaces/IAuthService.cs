using ETD_Portal.Auth_Module.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Auth_Module.Service.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO dto);
        Task<LoginResponseDTO> RefreshTokenAsync(RefreshTokenRequestDTO dto);
        Task LogoutAsync(int employeeId);

        Task ChangePasswordAsync(int employeeId, ChangePasswordRequestDTO dto);
    }
}
