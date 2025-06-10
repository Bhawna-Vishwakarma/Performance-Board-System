using Dapper;
using Performance_Board_System.DBContext;
using Performance_Board_System.Models;
using Performance_Board_System.Repository.Interfaces;
using System.Data;

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
            //var query = "INSERT INTO Role (RoleName) VALUES (@RoleName)";
            //return await connection.ExecuteAsync(query, role).ConfigureAwait(false);
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@RoleName", role.RoleName);
                parameters.Add("@Result", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

                await connection.ExecuteAsync("SP_InsertRole", parameters, commandType: System.Data.CommandType.StoredProcedure).ConfigureAwait(false);
                return parameters.Get<int>("@Result");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in Insert Role: " + ex.Message);
                return 0;
            }
        }

        public async Task<int> UpdateAsync(Role role)
        {
            using var connection = _context.CreateConnection();
            //var query = "UPDATE Role SET RoleName = @RoleName WHERE RoleId = @RoleId";
            //return await connection.ExecuteAsync(query, role).ConfigureAwait(false);
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@RoleId", role.RoleId);
                parameters.Add("@RoleName", role.RoleName);
                parameters.Add("@Result", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

                await connection.ExecuteAsync("SP_UpdateRole", parameters, commandType: System.Data.CommandType.StoredProcedure).ConfigureAwait(false);
                return parameters.Get<int>("@Result");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in Update Role: " + ex.Message);
                return 0;
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@RoleID", id);
            parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("SP_DeleteRole", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

            return parameters.Get<int>("@Result");
            //return await connection.ExecuteAsync("DELETE FROM Role WHERE RoleId = @Id", new { Id = id }).ConfigureAwait(false);
        }
    }
}