 using Performance_Board_System.Models;
namespace Performance_Board_System.Repository.Interfaces
{
    public interface IAttendanceStatusRepository
    {
        Task<IEnumerable<AttendanceStatusMaster>> GetAllAsync();
        
        Task<AttendanceStatusMaster?> GetByIdAsync(int id);
        
        Task<int> AddAsync(AttendanceStatusMaster attendanceStatus);
        
        Task<int> UpdateAsync(AttendanceStatusMaster attendanceStatus);
        
        Task<int> DeleteAsync(int id);

    }
}
