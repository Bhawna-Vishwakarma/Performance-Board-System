using Performance_Board_System.Models;

namespace Performance_Board_System.Repository.Interfaces
{
    public interface IRatingRepository
    {
        Task<IEnumerable<Rating>> GetAllAsync();
        Task<Rating?> GetByIdAsync(int id);
        Task<int> AddAsync(Rating rating);
        Task<int> UpdateAsync(Rating rating);
        Task<int> DeleteAsync(int id);

    }
}
