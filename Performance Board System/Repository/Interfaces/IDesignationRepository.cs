using Performance_Board_System.Models;

namespace Performance_Board_System.Repository.Interfaces
{
    public interface IDesignationRepository
    {
        Task<IEnumerable<Designation>> GetAllAsync();

        Task<Designation?> GetByIdAsync(int id);

        Task<int> AddAsync(Designation designation);

        Task<int> UpdateAsync(Designation designation);

        Task<int> DeleteAsync(int id);
    }
}
