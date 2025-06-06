namespace Performance_Board_System.Models
{
    public class Rating
    {
        public int RatingId { get; set; }
        public required string RatingLabel { get; set; }
        public required int RatingScore { get; set; }
    }
}
