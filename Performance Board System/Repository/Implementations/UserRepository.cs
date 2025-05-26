using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Performance_Board_System.DBContext;
using Performance_Board_System.Models;
using Performance_Board_System.Repository.Interfaces;
using System.Data;
using System.Text;

namespace Performance_Board_System.Repository.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;
        public UserRepository(DapperContext context) {

            // Constructor logic here
            _context = context;
        }

        public async Task<int> RegisterUser(User user)
        {
            user.PasswordHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.PasswordHash));

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@FullName", user.FullName);
                    parameters.Add("@Email", user.Email);
                    parameters.Add("@PasswordHash", user.PasswordHash);
                    parameters.Add("@Role", user.Role);
                    parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    await connection.ExecuteAsync("RegisterUser", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

                    int result = parameters.Get<int>("@Result");
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in RegisterUser: " + ex.Message);
                    return 0;
                }
            }
        }

        public async Task<int> LoginUser(string email, string passwordHash)
        {
            using var connection = _context.CreateConnection();
            passwordHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(passwordHash));
            var parameters = new DynamicParameters();
            parameters.Add("@Email", email);
            parameters.Add("@PasswordHash", passwordHash);
            parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("LoginUser", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            return parameters.Get<int>("@Result");
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<User>(
                "SELECT Id, FullName, Email, Role FROM Users WHERE Email = @Email",
                new { Email = email }).ConfigureAwait(false);
        }

        public int MarkAttendance(int userId, DateTime date, TimeSpan? checkIn, TimeSpan? checkOut, string status)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@Date", date.Date);
            parameters.Add("@CheckIn", checkIn);
            parameters.Add("@CheckOut", checkOut);
            parameters.Add("@Status", status);
            parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);

            connection.Execute("MarkAttendance", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Result");
        }

        public List<Attendance> GetAttendanceRecords(int userId, DateTime? dateFilter = null)
        {
            using var connection = _context.CreateConnection();
            var query = "SELECT * FROM Attendance WHERE UserId = @UserId";
            if (dateFilter.HasValue)
            {
                query += " AND Date = @Date";
                return connection.Query<Attendance>(query, new { UserId = userId, Date = dateFilter.Value.Date }).AsList();
            }

            return connection.Query<Attendance>(query, new { UserId = userId }).AsList();
        }

        public async Task<List<AttendanceViewModel>> GetWeeklyAttendance(int userId, DateTime startDate, DateTime endDate)
        {
            var result = new List<AttendanceViewModel>();
            using (var connection = _context.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@UserId", userId);
                parameters.Add("@StartDate", startDate);
                parameters.Add("@EndDate", endDate);

                var data = await connection.QueryAsync<AttendanceViewModel>(
                    "GetWeeklyAttendance",
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).ConfigureAwait(false);

                result = data.ToList();
            }

            return result;
        }

        public async Task<List<AttendanceViewModel>> GetAttendanceByDateAsync(DateTime selectedDate)
        {
            using (var connection = _context.CreateConnection())
            {
                string procedure = "GetAttendanceByDate";
                var parameters = new { SelectedDate = selectedDate.Date };

                var result = await connection.QueryAsync<AttendanceViewModel>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).ConfigureAwait(false);

                return result.ToList();
            }
        }


    }
}
