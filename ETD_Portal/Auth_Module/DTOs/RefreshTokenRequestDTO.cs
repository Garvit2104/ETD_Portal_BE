using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Auth_Module.DTOs
{
    public class RefreshTokenRequestDTO
    {
        public int EmployeeId { get; set; }
        public string RefreshToken { get; set; }
    }
}
