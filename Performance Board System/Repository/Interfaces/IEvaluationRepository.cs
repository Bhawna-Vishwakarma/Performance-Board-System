using Performance_Board_System.Models;

namespace Performance_Board_System.Repository.Interfaces
{
    public interface IEvaluationRepository
    {
        public Task<int> InsertEvaluationAsync(EmployeeEvaluationViewModel model, int evaluatorUserId);

        public Task<IEnumerable<EvaluationDisplayViewModel>> GetEvaluationsByUserIdAsync(int userId);

        
        public Task<int> DeleteEvaluationAsync(int id);

        public Task<List<FeedbackSummaryViewModel>> GetLatestFeedbacksAsync();

        public Task<IEnumerable<dynamic>> GetRecentFeedbacksAsync(int userId);

        public Task<decimal> GetAverageRatingByUserIdAsync(int userId);

        public Task<(decimal AverageScore, DateTime LastUpdated, List<(string Category, decimal Score)> Categories)> GetUserRatingsAsync(int userId);


    }
}
