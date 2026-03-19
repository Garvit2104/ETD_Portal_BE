using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Reservation_Mgmt.DTOs.DownloadDTO
{
    public class DocDownloadDTO
    {
        public byte[] FileBytes { get; set; } // actual file content from disk
        public string FileName { get; set; } // name of the file
    }
}
