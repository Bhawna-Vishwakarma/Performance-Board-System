using Dapper;
using Performance_Board_System.DBContext;
using Performance_Board_System.Models;
using Performance_Board_System.Repository.Interfaces;

namespace Performance_Board_System.Repository.Implementations
{
    public class RatingRepository : IRatingRepository
    {
        private readonly DapperContext _context;
        public RatingRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Rating>> GetAllAsync()
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Rating>("SELECT RatingId, RatingLabel, RatingScore FROM Rating ORDER BY RatingId ASC").ConfigureAwait(false);
        }

        public async Task<Rating?> GetByIdAsync(int id)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Rating>(
                "SELECT RatingId, RatingLabel, RatingScore FROM Rating WHERE RatingId = @Id", new { Id = id }).ConfigureAwait(false);
        }

        public async Task<int> AddAsync(Rating rating)
        {
            using var connection = _context.CreateConnection();
            //var query = "INSERT INTO Rating (RatingLabel, RatingScore) VALUES (@RatingLabel, @RatingScore)";
            //return await connection.ExecuteAsync(query, rating).ConfigureAwait(false);
            try{
                var parameter = new DynamicParameters();
                parameter.Add("@RatingLabel", rating.RatingLabel);
                parameter.Add("@RatingScore", rating.RatingScore);
                parameter.Add("@Result", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

                await connection.ExecuteAsync("SP_InsertRatings", parameter, commandType: System.Data.CommandType.StoredProcedure);
                return parameter.Get<int>("@Result"); 
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error in insert Rating: " + ex.Message);
                return 0;
            }
        }


        public async Task<int> UpdateAsync(Rating rating)
        {
            using var connection = _context.CreateConnection();
            //var query = "UPDATE Rating SET RatingLabel = @RatingLabel, RatingScore = @RatingScore WHERE RatingId = @RatingId";
            //return await connection.ExecuteAsync(query, rating).ConfigureAwait(false);
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@RatingID", rating.RatingId);
                parameter.Add("@RatingLabel", rating.RatingLabel);
                parameter.Add("@RatingScore", rating.RatingScore);
                parameter.Add("@Result", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

                await connection.ExecuteAsync("SP_UpdateRating", parameter, commandType: System.Data.CommandType.StoredProcedure);
                return parameter.Get<int>("@Result");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error in Update Rating" + ex.Message);
                return 0;
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            using var connection = _context.CreateConnection();
            //return await connection.ExecuteAsync("DELETE FROM Rating WHERE RatingId = @Id", new { Id = id }).ConfigureAwait(false);
            var parameter = new DynamicParameters();
            parameter.Add("@RatingID", id);
            parameter.Add("@Result", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

            await connection.ExecuteAsync("SP_DeleteRatings", parameter, commandType: System.Data.CommandType.StoredProcedure);
            return parameter.Get<int>("@Result");
        }
    }
}
