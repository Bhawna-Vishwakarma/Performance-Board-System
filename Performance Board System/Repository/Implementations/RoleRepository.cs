using Dapper;
using Performance_Board_System.DBContext;
using Performance_Board_System.Models;
using Performance_Board_System.Repository.Interfaces;

namespace Performance_Board_System.Repository.Implementations
{
    public class RoleRepository : IRoleRepository
    {
        private readonly DapperContext _context;
        public RoleRepository(DapperContext context) => _context = context;

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Role>("SELECT RoleId, RoleName FROM Role ORDER BY RoleId ASC").ConfigureAwait(false);
        }

        public async Task<Role?> GetByIdAsync(int id)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Role>(
                "SELECT RoleId, RoleName FROM Role WHERE RoleId = @Id", new { Id = id }).ConfigureAwait(false);
        }

        public async Task<int> AddAsync(Role role)
        {
            using var connection = _context.CreateConnection();
            var query = "INSERT INTO Role (RoleName) VALUES (@RoleName)";
            return await connection.ExecuteAsync(query, role).ConfigureAwait(false);
        }

        public async Task<int> UpdateAsync(Role role)
        {
            using var connection = _context.CreateConnection();
            var query = "UPDATE Role SET RoleName = @RoleName WHERE RoleId = @RoleId";
            return await connection.ExecuteAsync(query, role).ConfigureAwait(false);
        }

        public async Task<int> DeleteAsync(int id)
        {
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync("DELETE FROM Role WHERE RoleId = @Id", new { Id = id }).ConfigureAwait(false);
        }
    }
}