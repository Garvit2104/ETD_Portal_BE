using ETD_Portal.HR_Management.BLL.Interfaces;
using ETD_Portal.HR_Management.DAL.Interfaces;
using ETD_Portal.HR_Management.DTOs.ResponseDTO;
using ETD_Portal.Models;
using ETD_Portal.TravelPlanner.BLL.Interfaces;
using ETD_Portal.TravelPlanner.DAL.Interfaces;

namespace ETD_Portal.TravelPlanner.BLL.Classes
{
    public class BudgetAllocationServices : IBudgetAllocationServices
    {
        private readonly ITravelRequestRepo _travelRequestRepo;
        private readonly ITravelBudgetRepo _travelBudgetRepo;
        private readonly IUserRepo _userRepo;
       

        public BudgetAllocationServices(
            ITravelRequestRepo travelRequestRepo,
            ITravelBudgetRepo travelBudgetRepo,
            IUserRepo userRepo
            )
        {
            this._travelRequestRepo = travelRequestRepo;
            this._travelBudgetRepo = travelBudgetRepo;
            this._userRepo = userRepo;
       
        }

        public async Task<int> CalculateApprovedBudget(int employeeId, string? priority, int days)
        {
            var user = await _userRepo.GetEmployeeById(employeeId);
            if (user == null)
                throw new KeyNotFoundException($"Employee with ID {employeeId} not found.");

            string? gradeName = user.CurrentGrade?.Name;
            if (string.IsNullOrEmpty(gradeName))
                throw new ArgumentException("Employee grade is not assigned.");

            int maxBudgetByGrade = gradeName switch
            {
                "Grade-1" => 15000,
                "Grade-2" => 12500,
                "Grade-3" => 10000,
                _ => throw new ArgumentException($"Invalid grade: {gradeName}")
            };

            int maxDaysByPriority = priority switch
            {
                "One" => 30,
                "Two" => 20,
                "Three" => 10,
                _ => throw new ArgumentException($"Invalid priority: {priority}")
            };

            if (days > maxDaysByPriority)
                throw new ArgumentException(
                    $"Travel duration of {days} days exceeds allowed {maxDaysByPriority} days for priority {priority}.");

            return days * maxBudgetByGrade;
        }

        public async Task<string> CalculateHotelStarRating(int employeeId)
        {
            var user = await _userRepo.GetEmployeeById(employeeId);
            if (user == null)
                throw new KeyNotFoundException($"Employee with ID {employeeId} not found.");

            var rand = new Random();
            if (user.Role.Equals("HR", StringComparison.OrdinalIgnoreCase))
            {
                string[] hrHotels = { "5-Star", "7-Star" };
                return hrHotels[rand.Next(hrHotels.Length)];
            }
            else
            {
                string[] otherHotels = { "3-Star", "5-Star" };
                return otherHotels[rand.Next(otherHotels.Length)];
            }
        }

        public Task<string> CalculateModeOfTravel()
        {
            var rand = new Random();
            string[] modes = { "Air", "Train", "Bus" };
            return Task.FromResult(modes[rand.Next(modes.Length)]);
        }

        public async Task AddBudgetAllocation(TravelRequest approvedRequest)
        {
            if (approvedRequest.RaisedByEmployeeId == null) 
                throw new ArgumentException("Employee ID cannot be null."); 
            int employeeId = approvedRequest.RaisedByEmployeeId.Value;

            int days = approvedRequest.ToDate!.Value.DayNumber - approvedRequest.FromDate!.Value.DayNumber;

            var travelBudgetAllocation = new TravelBudgetAllocation
            {
                TravelRequestId = approvedRequest.RequestId,
                ApprovedBudget = await CalculateApprovedBudget(
                                            employeeId,
                                            approvedRequest.Priority,
                                            days),
                ApprovedModeOfTravel = await CalculateModeOfTravel(),
                ApprovedHotelStarRating = await CalculateHotelStarRating(employeeId)
            };
            await _travelBudgetRepo.AddBudgetAllocation(travelBudgetAllocation);
        }

        public async Task<int> CalculateBudget(int travelRequestId)
        {
            var travelRequest = await _travelRequestRepo.getTravelRequestById(travelRequestId);
            if (travelRequest == null)
                throw new KeyNotFoundException($"Travel request with ID {travelRequestId} not found.");

            if (!travelRequest.RequestStatus.Equals("Approved", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Budget can only be calculated for approved travel requests.");

            if (travelRequest.RaisedByEmployeeId == null)
                throw new ArgumentException("Employee Id cannot be null");
            if (travelRequest.FromDate == null || travelRequest.ToDate == null)
                throw new ArgumentException("Travel Date cannot be null");

            int employeeId = travelRequest.RaisedByEmployeeId.Value;
            int days = travelRequest.ToDate.Value.DayNumber - travelRequest.FromDate.Value.DayNumber;
            return await CalculateApprovedBudget(
                            employeeId,
                            travelRequest.Priority,
                            days);
        }

        
        
    }
}
