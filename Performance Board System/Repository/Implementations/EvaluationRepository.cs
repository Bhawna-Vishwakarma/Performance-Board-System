using Dapper;
using Performance_Board_System.DBContext;
using Performance_Board_System.Models;
using Performance_Board_System.Repository.Interfaces;
using System.Data;

namespace Performance_Board_System.Repository.Implementations
{
    public class EvaluationRepository : IEvaluationRepository
    {
        private readonly DapperContext _context;

        public EvaluationRepository(DapperContext context)
        { 
            _context = context;
        }

        public async Task<int> InsertEvaluationAsync(EmployeeEvaluationViewModel model, int evaluatorUserId)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@EvaluationPeriodStart", model.EvaluationPeriodStart);
            parameters.Add("@EvaluationPeriodEnd", model.EvaluationPeriodEnd);
            parameters.Add("@EvaluatedUserId", model.EvaluatedUserId);
            parameters.Add("@EvaluatorUserId", evaluatorUserId);
            parameters.Add("@CommunicationRatingId", model.CommunicationRatingId);
            parameters.Add("@PunctualityRatingId", model.PunctualityRatingId);
            parameters.Add("@TeamworkRatingId", model.TeamworkRatingId);
            parameters.Add("@TaskCompletionRatingId", model.TaskCompletionRatingId);
            parameters.Add("@AttendanceRatingId", model.AttendanceRatingId);
            parameters.Add("@Comment", model.Comment);
            parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("SP_InsertEmployeeEvaluation", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Result");
        }


        public async Task<IEnumerable<EvaluationDisplayViewModel>> GetEvaluationsByUserIdAsync(int userId)
        {
            using var connection = _context.CreateConnection();
            var evaluations = await connection.QueryAsync<EvaluationDisplayViewModel>(
                "SP_GetEvaluationsByUserId",
                new { EvaluatedUserId = userId },
                commandType: CommandType.StoredProcedure);

            return evaluations;
        }

        public async Task<int> DeleteEvaluationAsync(int evaluationId)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@EvaluationId", evaluationId);
            parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("SP_DeleteEmployeeEvaluation", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@Result");
        }

        public async Task<List<FeedbackSummaryViewModel>> GetLatestFeedbacksAsync()
        {
            using var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<FeedbackSummaryViewModel>(
                "SP_GetLatestFeedbacks", commandType: CommandType.StoredProcedure);
            return result.ToList();
        }


        public async Task<IEnumerable<dynamic>> GetRecentFeedbacksAsync(int userId)
        {
            using var connection = _context.CreateConnection();
            string query = @"
                SELECT TOP 5 
                    e.Comment, 
                    ev.FullName AS EvaluatorName
                FROM EmployeeEvaluation e
                INNER JOIN [User] ev ON e.EvaluatorUserId = ev.UserId
                WHERE e.EvaluatedUserId = @userId 
                  AND e.IsActive = 1 
                  AND ISNULL(e.Comment, '') <> '' 
                ORDER BY e.EvaluationDate DESC";

            return await connection.QueryAsync<dynamic>(query, new { userId });
        }

        public async Task<decimal> GetAverageRatingByUserIdAsync(int userId)
        {
            using var connection = _context.CreateConnection();
            string query = @"
        SELECT 
            AVG(
                (
                    ISNULL(r1.RatingScore, 0) + 
                    ISNULL(r2.RatingScore, 0) + 
                    ISNULL(r3.RatingScore, 0) + 
                    ISNULL(r4.RatingScore, 0) + 
                    ISNULL(r5.RatingScore, 0)
                ) / 1.0
            ) AS AverageScore
        FROM EmployeeEvaluation e
        LEFT JOIN Rating r1 ON e.CommunicationRatingId = r1.RatingId
        LEFT JOIN Rating r2 ON e.PunctualityRatingId = r2.RatingId
        LEFT JOIN Rating r3 ON e.TeamworkRatingId = r3.RatingId
        LEFT JOIN Rating r4 ON e.TaskCompletionRatingId = r4.RatingId
        LEFT JOIN Rating r5 ON e.AttendanceRatingId = r5.RatingId
        WHERE e.EvaluatedUserId = @userId AND e.IsActive = 1";

            return await connection.ExecuteScalarAsync<decimal>(query, new { userId });
        }

        public async Task<(decimal AverageScore, DateTime LastUpdated, List<(string Category, decimal Score)> Categories)> GetUserRatingsAsync(int userId)
        {
            using var connection = _context.CreateConnection();

            var scores = await connection.QueryAsync<(string Category, decimal Score)>(@"
                SELECT 'Communication', AVG(CommunicationRatingId * 1.0) FROM EmployeeEvaluation WHERE EvaluatedUserId = @userId
                UNION ALL
                SELECT 'Teamwork', AVG(TeamworkRatingId * 1.0) FROM EmployeeEvaluation WHERE EvaluatedUserId = @userId
                UNION ALL
                SELECT 'Punctuality', AVG(PunctualityRatingId * 1.0) FROM EmployeeEvaluation WHERE EvaluatedUserId = @userId
                UNION ALL
                SELECT 'Task Completion', AVG(TaskCompletionRatingId * 1.0) FROM EmployeeEvaluation WHERE EvaluatedUserId = @userId
                UNION ALL
                SELECT 'Attendance', AVG(AttendanceRatingId * 1.0) FROM EmployeeEvaluation WHERE EvaluatedUserId = @userId", new { userId });

            var avg = scores.Average(r => r.Score);
            var lastUpdated = await connection.ExecuteScalarAsync<DateTime>(
                "SELECT MAX(EvaluationDate) FROM EmployeeEvaluation WHERE EvaluatedUserId = @userId", new { userId });

            return (avg, lastUpdated, scores.ToList());
        }


    }
}
