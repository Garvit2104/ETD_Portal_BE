using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Reimbursement_Mgmt.DTOs.ResquestDTO
{
    public class DocumentRequestDTO
    {
        public string fileName { get; set; }

        public string filePath { get; set; }

        public byte[] fileData { get; set; }

        public string contentType { get; set; }

    }
}
