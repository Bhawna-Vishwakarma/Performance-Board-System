using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Performance_Board_System.Models
{
    public class EmployeeEvaluationViewModel
    {
        public int EvaluatedUserId { get; set; }
        public DateTime EvaluationPeriodStart { get; set; }
        public DateTime EvaluationPeriodEnd { get; set; }
        public int CommunicationRatingId { get; set; }
        public int PunctualityRatingId { get; set; }
        public int TeamworkRatingId { get; set; }
        public int TaskCompletionRatingId { get; set; }
        public int AttendanceRatingId { get; set; }
        public string Comment { get; set; }

        [BindNever]
        public List<Rating>? RatingList { get; set; }
    }
}
