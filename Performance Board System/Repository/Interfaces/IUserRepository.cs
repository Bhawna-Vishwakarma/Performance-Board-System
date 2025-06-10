using Performance_Board_System.Models;
namespace Performance_Board_System.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<int> RegisterUser(User user);

        Task<int> LoginUser(string email, string passwordHash);

        Task<UserRolesAssignViewModel?> GetUserById(int userId);

        Task<UsersViewModel?> GetUserByEmail(string email);

        Task<IEnumerable<UsersViewModel>> GetAllActiveUsersAsync();

        Task<IEnumerable<Department>> GetAllDepartment();
        
        Task<IEnumerable<Designation>> GetAllDesignation();

        Task<int> UpdateUserRoleAsync(User user);

        Task<int> SoftDeleteUserAsync(int userId);

        Task<int> AddUser(int userId);

        int MarkAttendance(int userId, DateTime date, TimeSpan? checkIn, TimeSpan? checkOut, string status);
        
        List<Attendance> GetAttendanceRecords(int userId, DateTime? dateFilter = null);

        Task<List<AttendanceViewModel>> GetWeeklyAttendance(int userId, DateTime startDate, DateTime endDate);

        Task<List<AttendanceViewModel>> GetAttendanceByDateAsync(DateTime selectedDate);

    }
}
