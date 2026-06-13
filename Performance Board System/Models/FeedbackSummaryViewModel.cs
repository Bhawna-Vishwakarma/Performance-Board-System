namespace Performance_Board_System.Models
{
    public class FeedbackSummaryViewModel
    {
        public int EvaluatedUserId { get; set; }
        public string EvaluatedName { get; set; }
        public string EvaluatorName { get; set; }
        public DateTime EvaluationPeriodStart { get; set; }
        public DateTime EvaluationPeriodEnd { get; set; }
        public double AverageScore { get; set; }
        public DateTime EvaluationDate { get; set; }
        public string Comment { get; set; }
    }
}
