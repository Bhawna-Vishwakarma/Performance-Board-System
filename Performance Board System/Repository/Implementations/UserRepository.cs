using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
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
        public UserRepository(DapperContext context) 
        {
            _context = context;
        }

        #region Ragister User Method

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
                    parameters.Add("@DepartmentID", user.DepartmentID);
                    parameters.Add("@DesignationId", user.DesignationId);
                    parameters.Add("@RoleId", 3);
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
        #endregion


        #region Login User Method

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

        #endregion


        #region Get All Users Method
        public async Task<IEnumerable<UsersViewModel>> GetAllActiveUsersAsync()
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<UsersViewModel>(
                "Select u.FullName, u.Email, u.UserId, u.RoleId, u.IsActive, r.RoleName, dept.DepartmentName, d.Title from [User] as u " +
                "LEFT join [Role] as r on u.RoleID = r.RoleID " +
                "LEFT join Department as dept on dept.DepartmentID = u.DepartmentId " +
                "LEFT join Designation as d on d.DesignationId = u.DesignationId " +
                //"where u.IsActive =1 " + 
                "order by r.RoleId ASC;",
                commandType: CommandType.Text).ConfigureAwait(false);
        }

        #endregion


        #region Get User by ID Method

        public async Task<UserRolesAssignViewModel?> GetUserById(int userId)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<UserRolesAssignViewModel>(
                "SELECT * FROM [User] WHERE UserId = @UserId",
                new { UserId = userId }).ConfigureAwait(false);
        }


        #endregion


        #region Get User by email Method

        public async Task<UsersViewModel?> GetUserByEmail(string email)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<UsersViewModel>(
                "SELECT UserId, FullName, Email, RoleId FROM [User] WHERE Email = @Email",
                new { Email = email }).ConfigureAwait(false);
        }

        #endregion


        #region Update User Roles Method
        
        public async Task<int> UpdateUserRoleAsync(User user)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", user.UserId);
            parameters.Add("@DepartmentId", user.DepartmentID);
            parameters.Add("@DesignationId", user.DesignationId);
            parameters.Add("@RoleId", user.RoleId);
            parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("SP_UpdateUserRole", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<int>("@Result");
        }

        #endregion


        #region Soft Delete User Method

        public async Task<int> SoftDeleteUserAsync(int userId)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await connection.ExecuteAsync("SP_SoftDeleteUser", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            return parameters.Get<int>("@Result");
        }

        #endregion


        #region Add User Method

        public async Task<int> AddUser(int userId)
        {
            using var connection = _context.CreateConnection();
            var query = "UPDATE [User] SET IsActive = 1 WHERE UserId = @UserId";
            return await connection.ExecuteAsync(query, new { UserId = userId }).ConfigureAwait(false);
        }

        #endregion








        #region Get All Department Method

        //public async Task<IEnumerable<Department>> GetAllDepartment()
        //{
        //    using var connection = _context.CreateConnection();
        //    return await connection.QueryAsync<Department>("SELECT DepartmentID,DepartmentName FROM [Department]").ConfigureAwait(false);
        //}
        #endregion


        #region Get All Designation Method

        //public async Task<IEnumerable<Designation>> GetAllDesignation()
        //{
        //    using var connection = _context.CreateConnection();
        //    return await connection.QueryAsync<Designation>("SELECT DesignationId, Title FROM Designation").ConfigureAwait(false);
        //}
        #endregion


        #region Mark Attendance Method
        //public int MarkAttendance(int userId, DateTime date, TimeSpan? checkIn, TimeSpan? checkOut, string status)
        //{
        //    using var connection = _context.CreateConnection();
        //    var parameters = new DynamicParameters();
        //    parameters.Add("@UserId", userId);
        //    parameters.Add("@Date", date.Date);
        //    parameters.Add("@CheckIn", checkIn);
        //    parameters.Add("@CheckOut", checkOut);
        //    parameters.Add("@Status", status);
        //    parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);

        //    connection.Execute("MarkAttendance", parameters, commandType: CommandType.StoredProcedure);
        //    return parameters.Get<int>("@Result");
        //}
        #endregion


        #region Attendance Record Method 
        //public List<Attendance> GetAttendanceRecords(int userId, DateTime? dateFilter = null)
        //{
        //    using var connection = _context.CreateConnection();
        //    var query = "SELECT * FROM Attendance WHERE UserId = @UserId";
        //    if (dateFilter.HasValue)
        //    {
        //        query += " AND Date = @Date";
        //        return connection.Query<Attendance>(query, new { UserId = userId, Date = dateFilter.Value.Date }).AsList();
        //    }

        //    return connection.Query<Attendance>(query, new { UserId = userId }).AsList();
        //}

        #endregion


        #region Get Weekly Attendance Method

        //public async Task<List<AttendanceViewModel>> GetWeeklyAttendance(int userId, DateTime startDate, DateTime endDate)
        //{
        //    var result = new List<AttendanceViewModel>();
        //    using (var connection = _context.CreateConnection())
        //    {
        //        var parameters = new DynamicParameters();
        //        parameters.Add("@UserId", userId);
        //        parameters.Add("@StartDate", startDate);
        //        parameters.Add("@EndDate", endDate);

        //        var data = await connection.QueryAsync<AttendanceViewModel>(
        //            "GetWeeklyAttendance",
        //            parameters,
        //            commandType: CommandType.StoredProcedure
        //        ).ConfigureAwait(false);

        //        result = data.ToList();
        //    }

        //    return result;
        //}
        #endregion


        #region Get Attenance By Date Method
        //public async Task<List<AttendanceViewModel>> GetAttendanceByDateAsync(DateTime selectedDate)
        //{
        //    using (var connection = _context.CreateConnection())
        //    {
        //        string procedure = "GetAttendanceByDate";
        //        var parameters = new { SelectedDate = selectedDate.Date };

        //        var result = await connection.QueryAsync<AttendanceViewModel>(
        //            procedure,
        //            parameters,
        //            commandType: CommandType.StoredProcedure
        //        ).ConfigureAwait(false);

        //        return result.ToList();
        //    }
        //}
        #endregion

    }
}
