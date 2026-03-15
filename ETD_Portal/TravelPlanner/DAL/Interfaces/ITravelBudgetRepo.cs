namespace ETD_Portal.TravelPlanner.DAL.Interfaces
{
    public interface ITravelBudgetRepo
    {
        Task AddBudgetAllocation(TravelBudgetAllocation travelBudgetAllocation);
    }
}
