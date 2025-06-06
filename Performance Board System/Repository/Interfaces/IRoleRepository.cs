using Performance_Board_System.Models;

namespace Performance_Board_System.Repository.Interfaces
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllAsync();

        Task<Role?> GetByIdAsync(int id);
        
        Task<int> AddAsync(Role role);
        
        Task<int> UpdateAsync(Role role);
        
        Task<int> DeleteAsync(int id);
    }
}
