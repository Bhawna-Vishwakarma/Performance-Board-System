using Dapper;
using Performance_Board_System.DBContext;
using Performance_Board_System.Models;
using Performance_Board_System.Repository.Interfaces;
using System.Data;

namespace Performance_Board_System.Repository.Implementations
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly DapperContext _context;
        public DepartmentRepository(DapperContext context) => _context = context;

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Department>("Select DepartmentID, DepartmentName from Department  order by DepartmentID asc").ConfigureAwait(false);
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Department>(
                "SELECT DepartmentID, DepartmentName FROM Department WHERE DepartmentID = @Id", new { Id = id }).ConfigureAwait(false);
        }

        public async Task<int> AddAsync(Department department)
        {
            //using var connection = _context.CreateConnection();
            //var query = "INSERT INTO Department (DepartmentName) VALUES (@DepartmentName)";
            //return await connection.ExecuteAsync(query, department).ConfigureAwait(false);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@DepartmentName", department.DepartmentName);
                    parameters.Add("@Result", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

                    await connection.ExecuteAsync("SP_InsertDepartment", parameters, commandType: System.Data.CommandType.StoredProcedure).ConfigureAwait(false);
                    return parameters.Get<int>("@Result");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in Insert Department: " + ex.Message);
                    return 0;
                }
            }
        }

        public async Task<int> UpdateAsync(Department department)
        {
            //using var connection = _context.CreateConnection();
            //var query = "UPDATE Department SET DepartmentName = @DepartmentName WHERE DepartmentID = @DepartmentID";
            //return await connection.ExecuteAsync(query, department).ConfigureAwait(false);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@DepartmentID", department.DepartmentID);
                    parameters.Add("@DepartmentName", department.DepartmentName);
                    parameters.Add("@Result", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

                    await connection.ExecuteAsync("SP_UpdateDepartment", parameters, commandType: System.Data.CommandType.StoredProcedure).ConfigureAwait(false);
                    return parameters.Get<int>("@Result");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in Update Department: " + ex.Message);
                    return 0;
                }
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@DepartmentID", id);
            parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("SP_DeleteDepartment", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<int>("@Result");
            //return await connection.ExecuteAsync("DELETE FROM Department WHERE DepartmentID = @Id", new { Id = id }).ConfigureAwait(false);
        }

    }
}
