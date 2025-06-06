using Performance_Board_System.Models;

namespace Performance_Board_System.Repository.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAllAsync();

        Task<Department?> GetByIdAsync(int id);

        Task<int> AddAsync(Department department);
        
        Task<int> UpdateAsync(Department department);
        
        Task<int> DeleteAsync(int id);
    }
}
