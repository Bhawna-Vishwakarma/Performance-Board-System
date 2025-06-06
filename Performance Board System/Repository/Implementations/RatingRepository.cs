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
            var query = "INSERT INTO Rating (RatingLabel, RatingScore) VALUES (@RatingLabel, @RatingScore)";
            return await connection.ExecuteAsync(query, rating).ConfigureAwait(false);
        }


        public async Task<int> UpdateAsync(Rating rating)
        {
            using var connection = _context.CreateConnection();
            var query = "UPDATE Rating SET RatingLabel = @RatingLabel, RatingScore = @RatingScore WHERE RatingId = @RatingId";
            return await connection.ExecuteAsync(query, rating).ConfigureAwait(false);
        }

        public async Task<int> DeleteAsync(int id)
        {
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync("DELETE FROM Rating WHERE RatingId = @Id", new { Id = id }).ConfigureAwait(false);

        }
    }
}
