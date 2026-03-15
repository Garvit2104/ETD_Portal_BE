using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing.Constraints;
using Reservation_Managmeent_App.DAL.ReservationDocs;
using Reservation_Managmeent_App.DTOs.DownloadDTOs;
using Reservation_Managmeent_App.DTOs.ReservationDocs;
using Reservation_Managmeent_App.Models;

namespace Reservation_Managmeent_App.BLL.ReservationDocs
{
    public class ReservationDocsService : IReservationDocsService
    {
        private readonly IReservationDocsRepo _reservationDocRepo;

        public ReservationDocsService(IReservationDocsRepo _reservationDocRepo)
        {
            this._reservationDocRepo = _reservationDocRepo;
        }

        public class DocumentSizeLimitExceededException : Exception 
        { 
            public DocumentSizeLimitExceededException(string message) : base(message) { } 
        }

        public async Task UploadReservationDocs(int reservationId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No Document is uploaded");

            if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Only PDF documents are allowed");

            if (file.Length > 1024 * 1024)
                throw new DocumentSizeLimitExceededException("File size exceed 1 MB");

           
            string originalName = Path.GetFileName(file.FileName);
            string fileName = $"Reservation_{reservationId}_{originalName}";
;            

            // ── Save to disk ──
            string uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(), "ReservationDocs");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // save metadata in db or to entity
            var uploadesDocsEntity = new ReservationDoc
            {
                ReservationId = reservationId,
                DocumentUrl = fileName
            };

            await _reservationDocRepo.AddReservatonDocs(uploadesDocsEntity);

        }

        public async Task<DocDownloadDTO> GetReservationDoc(int reservationId)
        {
            var docMeta = await _reservationDocRepo.GetReservationDocByReservationId(reservationId);

            if (docMeta == null)
                return null;

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "ReservationDocs");
            string filePath = Path.Combine(uploadsFolder, docMeta.DocumentUrl);

            if (!System.IO.File.Exists(filePath)) 
                return null; 
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            return new DocDownloadDTO
            {
                FileBytes = fileBytes,
                FileName = docMeta.DocumentUrl
            };
        }
    }
}
