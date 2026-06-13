namespace Performance_Board_System.Models
{
    public class EvaluationDisplayViewModel
    {
        public int EvaluationId { get; set; }
        public int EvaluatedUserId { get; set; }
        public DateTime EvaluationPeriodStart { get; set; }
        public DateTime EvaluationPeriodEnd { get; set; }
        public string EvaluatorName { get; set; }
        public string CommunicationRating { get; set; }
        public string PunctualityRating { get; set; }
        public string TeamworkRating { get; set; }
        public string TaskCompletionRating { get; set; }
        public string AttendanceRating { get; set; }
        public string Comment { get; set; }
        public DateTime EvaluationDate { get; set; }
    }
}
