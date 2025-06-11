namespace Performance_Board_System.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public string? Remarks { get; set; }
        public int? StatusId { get; set; }
    }
}
