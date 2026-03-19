using ETD_Portal.HR_Management.BLL.Classes;
using ETD_Portal.HR_Management.BLL.Interfaces;
using ETD_Portal.HR_Management.DTOs.ResponseDTO;
using ETD_Portal.Models;
using ETD_Portal.Reservation_Mgmt.BLL.Interfaces;
using ETD_Portal.Reservation_Mgmt.DAL.Interfaces;
using ETD_Portal.Reservation_Mgmt.DTOs.RequestDTO;
using ETD_Portal.Reservation_Mgmt.DTOs.ResponseDTO;
using ETD_Portal.TravelPlanner.BLL.Interfaces;
using ETD_Portal.TravelPlanner.DTOs.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETD_Portal.Reservation_Mgmt.BLL.Classes
{
    public class ReservationServices : IReservationServices
    {
        private readonly IReservationRepo _reservationRepos;
        private readonly IReservationDocRepo _reservationDocs;
        private readonly IReservationTypeServices _reservationTypes;
        private readonly IUserServices _userServices;
        private readonly ITravelRequestServices _travelRequestServices;
        private readonly IBudgetAllocationServices _budgetAllocationServices;
        public ReservationServices(IReservationRepo _reservationRepos, IReservationDocRepo _reservationDocs, IReservationTypeServices _reservationTypes, ITravelRequestServices _travelRequestServices, IBudgetAllocationServices _budgetAllocationServices, IUserServices _userServices)
        {
            this._reservationRepos = _reservationRepos;
            this._reservationDocs = _reservationDocs;
            this._reservationTypes = _reservationTypes;
            this._travelRequestServices = _travelRequestServices;
            this._budgetAllocationServices = _budgetAllocationServices;
            this._userServices = _userServices;
        }

        public async Task<List<ReservationResponseDTO>> GetReservationByTravelRequestId(int trid)
        {
            var reservations = await _reservationRepos.GetReservationByTravelRequestId(trid);


            List<ReservationResponseDTO> result = new List<ReservationResponseDTO>();

            foreach (var item in reservations)
            {
                ReservationResponseDTO dto = new ReservationResponseDTO
                {
                    id = item.Id,
                    reservation_done_by_employee_id = item.ReservationDoneByEmployeeId,
                    travel_request_id = item.TravelRequestId,
                    reservation_type_id = item.ReservationTypeId,
                    created_on = item.CreatedOn,
                    reservation_done_with_entity = item.ReservationDoneWithEntity,
                    reservation_date = item.ReservationDate,
                    amount = item.Amount,
                    remarks = item.Remarks
                };
                result.Add(dto);
            }

            return result;

        }

        public async Task<ReservationResponseDTO> GetReservationDetails(int reservationId)
        {
            var trackingList = await _reservationRepos.GetReservationDetails(reservationId);

            if (trackingList is null)
                return null;

            var trackingReservation = new ReservationResponseDTO
            {
                id = trackingList.Id,
                reservation_done_by_employee_id = trackingList.ReservationDoneByEmployeeId,
                travel_request_id = trackingList.TravelRequestId,
                reservation_type_id = trackingList.ReservationTypeId,
                created_on = trackingList.CreatedOn,
                reservation_done_with_entity = trackingList.ReservationDoneWithEntity,
                reservation_date = trackingList.ReservationDate,
                amount = trackingList.Amount,
                remarks = trackingList.Remarks

            };
            return trackingReservation;

        }


        public async Task<ReservationResponseDTO> AddReservation(ReservationRequestDTO addReservationRecord)
        {

            if (addReservationRecord is null)
                throw new ArgumentNullException(nameof(addReservationRecord), "Reservation payload is required.");

            // Validate employee exists and is TravelDeskExec
            if (!addReservationRecord.reservation_done_by_employee_id.HasValue)
                throw new ArgumentException("Employee Id is required");

            int employeeId = addReservationRecord.reservation_done_by_employee_id.Value;

            var user = await _userServices.GetEmployeeById(employeeId);

            if (user == null)
               throw new Exception("user not found");

            if (user == null || !string.Equals(user.role, "TravelDeskExe", StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("Travel Desk executive can only do reservation");

            // Check is travel request exist

            if (!addReservationRecord.travel_request_id.HasValue)
                throw new ArgumentException("TravelRequestId is required.");

            int travelRequestId = addReservationRecord.travel_request_id.Value;

            TravelResponseDTO tr = await _travelRequestServices.GetTravelRequestById(travelRequestId);
            if (tr == null)
                throw new ArgumentException("No Travel Request found for this travelRequestId");


            if (tr.from_date == null || tr.from_date == default)
                throw new InvalidOperationException("Travel request is missing FromDate.");

            DateOnly fromDate = tr.from_date!.Value;


            // --- Validate ReservationDate  and ReservatinTypeId  ---

            if (!addReservationRecord.reservation_type_id.HasValue)
                throw new ArgumentException("ReservationTypeId is required.");
            if (!addReservationRecord.reservation_date.HasValue)
                throw new ArgumentException("ReservationDate is required.");


            int typeId = addReservationRecord.reservation_type_id.Value;
            var reservationTypes = await _reservationTypes.GetReservationTypes()
                                         ?? Enumerable.Empty<ReservationTypeResponseDTO>();

            string typeName = reservationTypes.FirstOrDefault(t => t.type_id == typeId)?.type_name ?? "Unknown";

            DateOnly reservationDate = addReservationRecord.reservation_date.Value;

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

            await CheckReservationType(addReservationRecord.reservation_type_id!.Value,
                    addReservationRecord.travel_request_id!.Value);

            // 6 - Budget 
            int amount = addReservationRecord.amount.Value;
            int approvedBudget = await _budgetAllocationServices.CalculateBudget(travelRequestId);
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
                ReservationDoneByEmployeeId = addReservationRecord.reservation_done_by_employee_id,
                TravelRequestId = addReservationRecord.travel_request_id,
                ReservationTypeId = addReservationRecord.reservation_type_id,
                CreatedOn = DateOnly.FromDateTime(DateTime.Now),
                ReservationDoneWithEntity = addReservationRecord.reservation_done_with_entity,
                ReservationDate = addReservationRecord.reservation_date,
                Amount = addReservationRecord.amount,
                Remarks = addReservationRecord.remarks

            };

            var addedReservation = await _reservationRepos.AddReservations(reservationEntity);

            // Returning Response back to Client

            ReservationResponseDTO reservationResponse = new ReservationResponseDTO
            {
                id = addedReservation.Id,
                reservation_done_by_employee_id = addedReservation.ReservationDoneByEmployeeId,
                travel_request_id = addedReservation.TravelRequestId,
                reservation_type_id = addedReservation.ReservationTypeId,
                created_on = addedReservation.CreatedOn,
                reservation_done_with_entity = addReservationRecord.reservation_done_with_entity,
                reservation_date = addedReservation.ReservationDate,
                amount = addedReservation.Amount,
                
                remarks = addedReservation.Remarks
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
    }
}
