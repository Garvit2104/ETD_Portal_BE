using ETD_Portal.Models;
using ETD_Portal.Reservation_Mgmt.BLL.Interfaces;
using ETD_Portal.Reservation_Mgmt.DAL.Interfaces;
using ETD_Portal.Reservation_Mgmt.DTOs.DownloadDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Reservation_Mgmt.BLL.Classes
{
    public class ReservationDocServices : IReservationDocServices
    {
        private readonly IReservationDocRepo _reservationDocRepo;
        public ReservationDocServices(IReservationDocRepo _reservationDocRepo) 
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
