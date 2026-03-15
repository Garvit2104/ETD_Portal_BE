using Reservation_Managmeent_App.ClientMicroServices.Client_HR;
using Reservation_Managmeent_App.ClientMicroServices.Client_TP;
using Reservation_Managmeent_App.DAL.Reservations;
using Reservation_Managmeent_App.DTOs.ClientDTO;
using Reservation_Managmeent_App.DTOs.Reservations;
using Reservation_Managmeent_App.DTOs.ReservationsDTO;
using Reservation_Managmeent_App.Models;
using Reservation_Managmeent_App.DAL.ReservationDocs;
using Reservation_Managmeent_App.DTOs.ReservationDocs;
using Reservation_Managmeent_App.BLL.ReservationTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Reservation_Managmeent_App.BLL.Reservations
{
    public class ReservationsServiceClass: IReservationService
    {
        private readonly IReservationRepos _reservationRepos;
        private readonly IClient_HR_Service _hrServices;
        private readonly IClient_TP_Service _tpServices;
        private readonly IReservationDocsRepo _reservationDocs;
        private readonly IReservationTypeService _reservationTypes;

        public ReservationsServiceClass(IReservationRepos _reservationRepos, 
            IClient_HR_Service _hrServices, IClient_TP_Service _tpServices, 
            IReservationDocsRepo _reservationDocs, IReservationTypeService _reservationTypes)
        {
            this._reservationRepos = _reservationRepos;
            this._hrServices = _hrServices;
            this._tpServices = _tpServices;
            this._reservationDocs = _reservationDocs;
            this._reservationTypes = _reservationTypes;
        }

        public async Task<List<ReservationResponseDTO>> GetReservationByTravelRequestId(int trid)
        {
            var reservations =  await _reservationRepos.GetReservationByTravelRequestId(trid);


            List<ReservationResponseDTO> result = new List<ReservationResponseDTO>();

            foreach (var item in reservations)
            {
                ReservationResponseDTO dto = new ReservationResponseDTO
                {
                    Id = item.Id,
                    ReservationDoneByEmployeeId = item.ReservationDoneByEmployeeId,
                    TravelRequestId = item.TravelRequestId,
                    ReservationTypeId = item.ReservationTypeId,
                    CreatedOn = item.CreatedOn,
                    ReservationDoneWithEntity = item.ReservationDoneWithEntity,
                    ReservationDate = item.ReservationDate,
                    Amount = item.Amount,
                    ConfirmationId = item.ConfirmationId,
                    Remarks = item.Remarks
                };
                result.Add(dto);
            }

            return result;

        }
            public async Task<ReservationResponseDTO> AddReservation(ReservationRequestDTO addReservationRecord)
        {
            // Validate employee exists and is TravelDeskExec
            if (!addReservationRecord.ReservationDoneByEmployeeId.HasValue)
                throw new ArgumentException("Reservation Dome by Employee Id is required");

            int employeeId = addReservationRecord.ReservationDoneByEmployeeId.Value;

            EmployeeResponseDTO user = await _hrServices.GetEmployeeById(employeeId);
            if (user == null || !string.Equals(user.role, "TravelDeskExe", StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("Travel Desk executive can only do reservation");

            // Check is travel request exist

            if (!addReservationRecord.TravelRequestId.HasValue)
                throw new ArgumentException("TravelRequestId is required.");

            int travelRequestId = addReservationRecord.TravelRequestId.Value;

            TravelResponseDTO tr = await _tpServices.GetTravelRequestById(travelRequestId);
            if (tr == null)
                throw new ArgumentException("No Travel Request found for this travelRequestId");


            if (tr.from_date == null || tr.from_date == default)
                throw new InvalidOperationException("Travel request is missing FromDate.");

            DateOnly fromDate = DateOnly.FromDateTime(tr.from_date!.Value);


            // --- Validate ReservationDate  and ReservatinTypeId  ---

            if (!addReservationRecord.ReservationTypeId.HasValue)
                throw new ArgumentException("ReservationTypeId is required.");
            if (!addReservationRecord.ReservationDate.HasValue)
                throw new ArgumentException("ReservationDate is required.");


            int typeId = addReservationRecord.ReservationTypeId.Value;
            var reservationTypes = await _reservationTypes.GetReservationTypes();

            string typeName = reservationTypes.FirstOrDefault(t=>t.TypeId == typeId)?.TypeName ?? "Unknown";

            DateOnly reservationDate = addReservationRecord.ReservationDate.Value;
             
            // Reservation Dates Rules (a) and (b)
            DateOnly expectedDate = typeName switch
            {
                "Train" or "Bus" => fromDate.AddDays(-1),   // (a) Train and Bus must be one day before
                "Hotel" or "Flight" or "Cab" => fromDate,  // assumed same day
                _ => fromDate
            };

            if (reservationDate != expectedDate)
            {
                string msg = typeName switch
                {
                    "Train" or "Bus" => "The Reservation Date for the train or bus should be one day prior to the From Date",
                    "Hotel" or "Cab" or "Flight" => "The Reservation Date should be the same as the From Date",
                    _ => $"Invalid Reservation Date for type {typeName}"
                };
                throw new ArgumentException($"{msg}. Expected: {expectedDate:yyyy-MM-dd}");
            }

            // 5-Checking Reservation type rules

             await CheckReservationType(addReservationRecord.ReservationTypeId!.Value,
                     addReservationRecord.TravelRequestId!.Value);

            // 6 - Budget 
            int amount = addReservationRecord.Amount.Value;
            int approvedBudget = await _tpServices.CalculateBudget(travelRequestId);
            int maxTotalAmount = (int)(approvedBudget * 0.7);

            int maxForTravel = (int)(maxTotalAmount * 0.4);
            int maxForHotel = (int)(maxTotalAmount * 0.5);
            int maxForCab = (int)(maxTotalAmount * 0.1);

            

            if ((typeId == 1 || typeId == 2 || typeId == 3) && amount > maxForTravel)
                throw new ArgumentOutOfRangeException("amount should be less than " + maxForTravel);
            else if ((typeId == 4) && amount > maxForCab)
                throw new ArgumentOutOfRangeException("amount should be less than " + maxForCab);
            else if (typeId == 5 && amount > maxForHotel)
            {

                throw new Exception("amount should be less than " + maxForHotel);
            }

            // ── Step 7: Build and save entity ──
            var reservationEntity = new Reservation
            {
                ReservationDoneByEmployeeId = addReservationRecord.ReservationDoneByEmployeeId,
                TravelRequestId = addReservationRecord.TravelRequestId,
                ReservationTypeId = addReservationRecord.ReservationTypeId,
                CreatedOn = DateOnly.FromDateTime(DateTime.Now),
                ReservationDoneWithEntity = addReservationRecord.ReservationDoneWithEntity,
                ReservationDate = addReservationRecord.ReservationDate,
                Amount = addReservationRecord.Amount,
                ConfirmationId = addReservationRecord.ConfirmationId,
                Remarks = addReservationRecord.Remarks

            };
 
            var addedReservation = await _reservationRepos.AddReservations(reservationEntity);

            // Returning Response back to Client

            ReservationResponseDTO reservationResponse = new ReservationResponseDTO
            {
                Id = addedReservation.Id,
                ReservationDoneByEmployeeId = addedReservation.ReservationDoneByEmployeeId,
                TravelRequestId = addedReservation.TravelRequestId,
                ReservationTypeId = addedReservation.ReservationTypeId,
                CreatedOn = addedReservation.CreatedOn,
                ReservationDoneWithEntity = addedReservation.ReservationDoneWithEntity,
                ReservationDate = addedReservation.ReservationDate,
                Amount = addedReservation.Amount,
                ConfirmationId = addedReservation.ConfirmationId,
                Remarks = addedReservation.Remarks
            };
            return reservationResponse;
        }

        public async Task CheckReservationType(int typeId, int travelRequestId)
        {
            const int Flight = 1, Train = 2, Bus = 3, Cab = 4, Hotel = 5;

            int reservationCount = await _reservationRepos.CountReservationsByTravelRequestId(travelRequestId);

            // Enforce max 3 reservations
            if (reservationCount >= 3)
                throw new InvalidOperationException("This travel already has the maximum of three reservations (Transport, Hotel, Cab).");

            bool newIsTransport = (typeId == Flight || typeId == Train || typeId == Bus);
            bool newIsHotel = (typeId == Hotel);
            bool newIsCab = (typeId == Cab);

            if (newIsTransport && await _reservationRepos.ExistsReservationOfAnyType(travelRequestId, Flight, Train, Bus))
                throw new InvalidOperationException("A Transport reservation (Flight/Train/Bus) already exists for this travel.");

            if (newIsHotel && await _reservationRepos.ExistsReservationOfAnyType(travelRequestId, Hotel))
                throw new InvalidOperationException("A Hotel reservation already exists for this travel.");

            if (newIsCab && await _reservationRepos.ExistsReservationOfAnyType(travelRequestId, Cab))
                throw new InvalidOperationException("A Cab reservation already exists for this travel.");
        }

        //public ReservationResponseDTO GetTrackReservationByTrid(int trid)
        //{
        //    var trackingList = _reservationRepos.GetReservationByTravelRequestId(trid);

           
            
        //        var trackingReservation = new ReservationResponseDTO
        //        {
        //            Id = trackingList.Id,
        //            ReservationDoneByEmployeeId = trackingList.ReservationDoneByEmployeeId,
        //            TravelRequestId = trackingList.TravelRequestId,
        //            ReservationTypeId = trackingList.ReservationTypeId,
        //            CreatedOn = trackingList.CreatedOn,
        //            ReservationDoneWithEntity = trackingList.ReservationDoneWithEntity,
        //            ReservationDate = trackingList.ReservationDate,
        //            Amount = trackingList.Amount,
        //            ConfirmationId = trackingList.ConfirmationId,
        //            Remarks = trackingList.Remarks

        //        };
        //    return trackingReservation;
         
        //}

        public async Task<ReservationResponseDTO> GetReservationDetails(int reservationId)
        {
            var trackingList = await _reservationRepos.GetReservationDetails(reservationId);



            var trackingReservation = new ReservationResponseDTO
            {
                Id = trackingList.Id,
                ReservationDoneByEmployeeId = trackingList.ReservationDoneByEmployeeId,
                TravelRequestId = trackingList.TravelRequestId,
                ReservationTypeId = trackingList.ReservationTypeId,
                CreatedOn = trackingList.CreatedOn,
                ReservationDoneWithEntity = trackingList.ReservationDoneWithEntity,
                ReservationDate = trackingList.ReservationDate,
                Amount = trackingList.Amount,
                ConfirmationId = trackingList.ConfirmationId,
                Remarks = trackingList.Remarks

            };
            return trackingReservation;

        }


    }
}


