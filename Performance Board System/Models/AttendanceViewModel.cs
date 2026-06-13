namespace Performance_Board_System.Models
{
    public class AttendanceViewModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public string? Remarks { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
    }
}
