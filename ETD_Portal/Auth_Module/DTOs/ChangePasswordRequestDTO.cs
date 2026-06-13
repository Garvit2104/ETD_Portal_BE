using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Auth_Module.DTOs
{
    public class ChangePasswordRequestDTO
    {
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }

    }
}
