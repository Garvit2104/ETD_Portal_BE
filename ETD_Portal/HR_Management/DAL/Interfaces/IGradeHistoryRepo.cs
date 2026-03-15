namespace ETD_Portal.HR_Management.DAL.Interfaces
{
    public interface IGradeHistoryRepo
    {
        public Task AddGradeHistory(GradeHistory gradeHistory);

        Task<IEnumerable<GradeHistory>> GetAllGradeHistoryByEmployeeId(int? id);
    }
}
