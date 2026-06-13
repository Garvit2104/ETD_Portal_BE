using ETD_Portal.HR_Management.DAL.Interfaces;
using ETD_Portal.Models;
using ETD_Portal.TravelPlanner.BLL.Interfaces;
using ETD_Portal.TravelPlanner.DAL.Interfaces;
using ETD_Portal.TravelPlanner.DTOs.RequestDTO;
using ETD_Portal.TravelPlanner.DTOs.ResponseDTO;

namespace ETD_Portal.TravelPlanner.BLL.Classes
{
    public class TravelRequestServices : ITravelRequestServices
    {
        private readonly ITravelRequestRepo _travelRequestRepo;
        private readonly IUserRepo _userRepo;
        private readonly IBudgetAllocationServices _budgetAllocationServices;

        public TravelRequestServices(
            ITravelRequestRepo travelRequestRepo,
            IUserRepo userRepo,
            IBudgetAllocationServices budgetAllocationServices)
        {
            this._travelRequestRepo = travelRequestRepo;
            this._userRepo = userRepo;
            this._budgetAllocationServices = budgetAllocationServices;
        }

        public static TravelRequest RequestDtoToEntity(TravelRequestDTO travelRequestDTO)
        {
            if (travelRequestDTO.from_date < DateOnly.FromDateTime(DateTime.Now))
                throw new ArgumentException("FromDate must be greater than today's date.");

            if (travelRequestDTO.to_date <= travelRequestDTO.from_date)
                throw new ArgumentException("ToDate must be greater than FromDate.");

            return new TravelRequest
            {
                RaisedByEmployeeId = travelRequestDTO.raised_by_employee_id,
                ToBeApprovedByHrId = travelRequestDTO.to_be_approved_by_hr_id,
                FromDate = travelRequestDTO.from_date,
                ToDate = travelRequestDTO.to_date,
                PurposeOfTravel = travelRequestDTO.purpose_of_travel,
                Priority = travelRequestDTO.priority,
                LocationId = travelRequestDTO.location_id,
                RequestStatus = "New",
                RequestRaisedOn = DateOnly.FromDateTime(DateTime.Now),
                RequestApprovedOn = null
            };
        }

        public static TravelResponseDTO EntityToResponseDto(TravelRequest entity)
        {
            return new TravelResponseDTO
            {
                request_id = entity.RequestId?? 0,
                raised_by_employee_id = entity.RaisedByEmployeeId,
                to_be_approved_by_hr_id = entity.ToBeApprovedByHrId,
                request_raised_on = entity.RequestRaisedOn,
                from_date = entity.FromDate,
                to_date = entity.ToDate,
                purpose_of_travel = entity.PurposeOfTravel,
                priority = entity.Priority,
                request_status = entity.RequestStatus,
                location_name = entity.Location?.Name,
                request_approved_on = entity.RequestApprovedOn
            };
        }

        public static TravelRequestDetailsRespDTO EntityToDetailsRespDto(TravelRequest entity)
        {
            return new TravelRequestDetailsRespDTO
            {
                request_id = entity.RequestId ?? 0,
                request_raised_on = entity.RequestRaisedOn,
                from_date = entity.FromDate ?? DateOnly.MinValue,
                to_date = entity.ToDate ?? DateOnly.MinValue,
                request_status = entity.RequestStatus ?? string.Empty,
                request_approved_on = entity.RequestApprovedOn,
                location_name = entity.Location?.Name ?? string.Empty,
                approved_budget = entity.TravelBudgetAllocations?.FirstOrDefault()?.ApprovedBudget,
                approved_mode_of_travel = entity.TravelBudgetAllocations?.FirstOrDefault()?.ApprovedModeOfTravel,
                approved_hotel_star_rating = entity.TravelBudgetAllocations?.FirstOrDefault()?.ApprovedHotelStarRating
            };
        }

        public async Task<TravelResponseDTO> CreateTravelRequest(TravelRequestDTO travelRequestDTO)
        {
            var employee = await _userRepo.GetEmployeeById(travelRequestDTO.raised_by_employee_id);
            if (employee == null)
                throw new KeyNotFoundException("Employee with this ID does not exist.");

            var hr = await _userRepo.GetEmployeeById(travelRequestDTO.to_be_approved_by_hr_id);
            if (hr == null)
                throw new KeyNotFoundException("No user found with this HR ID.");

            if (!hr.Role.Equals("HR", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("The provided ID does not belong to an HR role user.");

            var entity = RequestDtoToEntity(travelRequestDTO);
            var result = await _travelRequestRepo.CreateTravelRequest(entity);
            return EntityToResponseDto(result);
        }

        public async Task<IEnumerable<TravelResponseDTO>> GetAllPendingRequests(int hrId)
        {
            var hr = await _userRepo.GetEmployeeById(hrId);
            if (hr == null)
                throw new KeyNotFoundException($"No user found with ID {hrId}.");

            if (!hr.Role.Equals("HR", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("The provided ID does not belong to an HR role user.");

            var result = await _travelRequestRepo.GetAllPendingRequests(hrId);
            var pendingReqList = new List<TravelResponseDTO>();
            foreach (var item in result)
                pendingReqList.Add(EntityToResponseDto(item));
            return pendingReqList;
        }

        public async Task<TravelResponseDTO> GetTravelRequestById(int? trid)
        {
            var result = await _travelRequestRepo.GetTravelRequestById(trid);
            if (result == null)
                throw new KeyNotFoundException($"No travel request found with ID {trid}.");
            return EntityToResponseDto(result);
        }

        public async Task<TravelRequestDetailsRespDTO> GetTravelRequestDetailsById(int trid)
        {
            var result = await _travelRequestRepo.GetTravelRequestDetailsById(trid);
            if (result == null)
                throw new KeyNotFoundException($"No travel request found with ID {trid}.");

            return EntityToDetailsRespDto(result);
        }

        public async Task<TravelRequestDetailsRespDTO> UpdateRequestStatus(int trid, UpdateRequestDTO updateDTO)
        {
            var travelRequestId = await _travelRequestRepo.GetTravelRequestById(trid);
            if (travelRequestId == null)
                throw new KeyNotFoundException($"Travel request with ID {trid} not found.");

            if (!travelRequestId.RequestStatus.Equals("New", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Only travel requests with status 'New' can be updated.");

            var validStatuses = new[] { "Approved", "Rejected" };
            if (!validStatuses.Contains(updateDTO.request_status, StringComparer.OrdinalIgnoreCase))
                throw new ArgumentException("Status must be either Approved or Rejected.");

            travelRequestId.RequestStatus = updateDTO.request_status;

            if (travelRequestId.RequestStatus.Equals("Approved", StringComparison.OrdinalIgnoreCase))
                travelRequestId.RequestApprovedOn = DateOnly.FromDateTime(DateTime.Now);

            var updatedResult = await _travelRequestRepo.getUpdateRequestStatus(travelRequestId);

            if (updatedResult.RequestStatus.Equals("Approved", StringComparison.OrdinalIgnoreCase))
                await _budgetAllocationServices.AddBudgetAllocation(updatedResult);

            return EntityToDetailsRespDto(updatedResult);
        }

    }
}
