namespace Performance_Board_System.Models
{
    public class UsersViewModel
    {
            public int UserId { get; set; }
            public required string FullName { get; set; }
            public required string Email { get; set; }
            public required int RoleId { get; set; }
            public required string RoleName { get; set; }
            public required string DepartmentName { get; set; }
            public required string Title { get; set; }
            public bool IsActive { get; set; }

    }
}
