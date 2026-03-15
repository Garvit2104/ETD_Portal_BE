namespace Reservation_Managmeent_App.DTOs.DownloadDTOs
{
    public class DocDownloadDTO
    {
        public byte[] FileBytes { get; set; } // actual file content from disk
        public string FileName { get; set; } // name of the file
    }
}
