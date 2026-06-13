using Performance_Board_System.Models;
namespace Performance_Board_System.Repository.Interfaces
{
    public interface IAttendanceRepository
    {
        Task<List<AttendanceStatusMaster>> GetAllStatusesAsync();

        Task<IEnumerable<AttendanceViewModel>> GetWeeklyAttendanceAsync(int userId);

        Task<List<AttendanceViewModel>> GetAttendanceForUserInDateRange(int userId, DateTime start, DateTime end);

        Task<int> CheckInAsync(AttendanceCheckInModel model);
     
        Task<int> CheckOutAsync(int userId);
    }
}
