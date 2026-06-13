using ETD_Portal.Auth_Module.DTOs;
using ETD_Portal.Auth_Module.Service.Interfaces;
using ETD_Portal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETD_Portal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
            => _authService = authService;

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO dto)
        {
            var result = await _authService.LoginAsync(dto);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDTO dto)
        {
            var result = await _authService.RefreshTokenAsync(dto);
            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var employeeId = int.Parse(
                User.FindFirst("EmployeeId")!.Value);
            await _authService.LogoutAsync(employeeId);
            return Ok(new { message = "Logged out successfully." });
        }

        [HttpPost("change-password")]
        [Authorize]   // any logged-in user can change THEIR OWN password
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDTO dto)
        {
            var employeeId = int.Parse(User.FindFirst("EmployeeId")!.Value);
            await _authService.ChangePasswordAsync(employeeId, dto);
            return Ok(new { message = "Password changed successfully." });
        }
    }
}
