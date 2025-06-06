namespace Performance_Board_System.Models
{
    public class User
    {
        public int UserId { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required int RoleId { get; set; } 
        public int DesignationId { get; set; }
        public int DepartmentID { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
