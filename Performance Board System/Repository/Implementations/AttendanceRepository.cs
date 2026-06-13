using Dapper;
using Performance_Board_System.DBContext;
using Performance_Board_System.Models;
using Performance_Board_System.Repository.Interfaces;
using System.Data;

namespace Performance_Board_System.Repository.Implementations
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly DapperContext _context;

        public AttendanceRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<List<AttendanceStatusMaster>> GetAllStatusesAsync()
        {
            using var connection = _context.CreateConnection();
            var query = "SELECT StatusId, StatusName FROM AttendanceStatusMaster";
            return (await connection.QueryAsync<AttendanceStatusMaster>(query)).ToList();
        }

        public async Task<IEnumerable<AttendanceViewModel>> GetWeeklyAttendanceAsync(int userId)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<AttendanceViewModel>(
                "SP_GetWeeklyAttendanceByUser",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> CheckInAsync(AttendanceCheckInModel model)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("UserId", model.UserId);
            parameters.Add("StatusId", model.StatusId);
            parameters.Add("Remarks", model.Remarks);
            parameters.Add("Result", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("SP_CheckInAttendance", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("Result");
        }

        public async Task<int> CheckOutAsync(int userId)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId);
            parameters.Add("Result", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("SP_CheckOutAttendance", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("Result");
        }

        public async Task<List<AttendanceViewModel>> GetAttendanceForUserInDateRange(int userId, DateTime start, DateTime end)
        {
            using var connection = _context.CreateConnection();
            var query = @"SELECT A.Date, A.CheckInTime, A.CheckOutTime, A.Remarks, M.StatusName 
                  FROM Attendance A
                  JOIN AttendanceStatusMaster M ON A.StatusId = M.StatusId
                  WHERE A.UserId = @UserId AND A.Date BETWEEN @Start AND @End";

            var result = await connection.QueryAsync<AttendanceViewModel>(query, new { UserId = userId, Start = start, End = end });
            return result.ToList();
        }
    }
}
