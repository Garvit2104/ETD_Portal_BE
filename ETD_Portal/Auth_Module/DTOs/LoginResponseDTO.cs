using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Auth_Module.DTOs
{
    public class LoginResponseDTO
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AccessTokenExpiry { get; set; }
    }
}
