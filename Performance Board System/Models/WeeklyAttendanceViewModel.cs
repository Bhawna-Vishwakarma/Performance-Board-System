namespace Performance_Board_System.Models
{
    public class WeeklyAttendanceViewModel
    {
        public DateTime Date { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public string? StatusName { get; set; }
        public string? Remarks { get; set; }
        public bool IsToday { get; set; }
        public bool IsCheckedIn { get; set; }
    }
}
