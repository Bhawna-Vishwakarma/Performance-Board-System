using Performance_Board_System.Models;
namespace Performance_Board_System.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<int> RegisterUser(User user);

        Task<int> LoginUser(string email, string passwordHash);

        Task<User?> GetUserByEmail(string email);

        int MarkAttendance(int userId, DateTime date, TimeSpan? checkIn, TimeSpan? checkOut, string status);
        List<Attendance> GetAttendanceRecords(int userId, DateTime? dateFilter = null);

        Task<List<AttendanceViewModel>> GetWeeklyAttendance(int userId, DateTime startDate, DateTime endDate);

        Task<List<AttendanceViewModel>> GetAttendanceByDateAsync(DateTime selectedDate);

    }
}
