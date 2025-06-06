using Dapper;
using Performance_Board_System.DBContext;
using Performance_Board_System.Models;
using Performance_Board_System.Repository.Interfaces;

namespace Performance_Board_System.Repository.Implementations
{
    public class AttendanceStatusRepository : IAttendanceStatusRepository
    {
        private readonly DapperContext _context;
        public AttendanceStatusRepository(DapperContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<AttendanceStatusMaster>> GetAllAsync()
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<AttendanceStatusMaster>("SELECT StatusId, StatusName FROM AttendanceStatusMaster ORDER BY StatusId ASC").ConfigureAwait(false);
        }
        public async Task<AttendanceStatusMaster?> GetByIdAsync(int id)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<AttendanceStatusMaster>(
                "SELECT StatusId, StatusName FROM AttendanceStatusMaster WHERE StatusId = @Id", new { Id = id }).ConfigureAwait(false);
        }
        public async Task<int> AddAsync(AttendanceStatusMaster attendanceStatus)
        {
            using var connection = _context.CreateConnection();
            var query = "INSERT INTO AttendanceStatusMaster (StatusName) VALUES (@StatusName)";
            return await connection.ExecuteAsync(query, attendanceStatus).ConfigureAwait(false);
        }
        public async Task<int> UpdateAsync(AttendanceStatusMaster attendanceStatus)
        {
            using var connection = _context.CreateConnection();
            var query = "UPDATE AttendanceStatusMaster SET StatusName = @StatusName WHERE StatusId = @StatusId";
            return await connection.ExecuteAsync(query, attendanceStatus).ConfigureAwait(false);
        }
        public async Task<int> DeleteAsync(int id)
        {
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync("DELETE FROM AttendanceStatusMaster WHERE StatusId = @Id", new { Id = id }).ConfigureAwait(false);
        }
    }
}
