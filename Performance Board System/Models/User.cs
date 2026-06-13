namespace Performance_Board_System.Models
{
    public class User
    {
        public int UserId { get; set; }
        public required string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public required int RoleId { get; set; } 
        public int DesignationId { get; set; }
        public int DepartmentID { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
