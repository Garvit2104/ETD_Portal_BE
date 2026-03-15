using Microsoft.AspNetCore.SignalR.Protocol;

namespace Reimbursement__Managment.DTOs.Document_DTO
{
    public class DocumentRequestDTO
    {
        public string fileName {  get; set; }

        public string filePath { get; set; }

        public byte[] fileData { get; set; }

        public string contentType { get; set; }


    }
}
