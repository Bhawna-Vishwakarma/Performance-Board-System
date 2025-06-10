using Dapper;
using Performance_Board_System.DBContext;
using Performance_Board_System.Models;
using Performance_Board_System.Repository.Interfaces;
using System.Data;
using System.Linq.Expressions;

namespace Performance_Board_System.Repository.Implementations
{
    public class DesignationRepository : IDesignationRepository
    {
        private readonly DapperContext _context;
        public DesignationRepository(DapperContext context) => _context = context;

        public async Task<IEnumerable<Designation>> GetAllAsync()
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Designation>("Select DesignationId, Title from Designation  order by DesignationId asc").ConfigureAwait(false);
        }

        public async Task<Designation?> GetByIdAsync(int id)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Designation>(
                "SELECT DesignationId, Title FROM Designation WHERE DesignationId = @Id", new { Id = id }).ConfigureAwait(false);
        }

        public async Task<int> AddAsync(Designation designation)
        {
            using (var connection = _context.CreateConnection())
            {
                try{
                    var parameters = new DynamicParameters();
                    parameters.Add("@Title", designation.Title);
                    parameters.Add("@Result", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

                    await connection.ExecuteAsync("SP_InsertDesignation", parameters, commandType: System.Data.CommandType.StoredProcedure).ConfigureAwait(false);
                    return parameters.Get<int>("@Result");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in insert Designation: " + ex.Message);
                    return 0;
                }
            }
        }

        public async Task<int> UpdateAsync(Designation designation)
        {
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@DesignationId", designation.DesignationId);
                    parameters.Add("@Title", designation.Title);
                    parameters.Add("@Result", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

                    await connection.ExecuteAsync("SP_UpdateDesignation", parameters, commandType: System.Data.CommandType.StoredProcedure).ConfigureAwait(false);
                    return parameters.Get<int>("@Result");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in Update Designation: " + ex.Message);
                    return 0;
                }
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@DesignationId", id);
            parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("SP_DeleteDesignation", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

            return parameters.Get<int>("@Result");
        }


        //public async Task<int> DeleteAsync(int id)
        //{
        //    using var connection = _context.CreateConnection();
        //    return await connection.ExecuteAsync("DELETE FROM Designation WHERE DesignationId = @Id", new { Id = id }).ConfigureAwait(false);
        //}
    }
}