using Reimbursement__Managment.Models;
namespace Reimbursement__Managment.DAL
{

    public interface IReimbursementTypeRepo
    {
        public Task<List<ReimbursementType>> GetAllReimbursementType();
        public Task<ReimbursementType> GetTypeById(int id);
    }
}
