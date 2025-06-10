namespace Performance_Board_System.Models
{
    public class UserRolesAssignViewModel
    {
        public int UserId { get; set; }

        public required string FullName { get; set; }

        public int RoleId { get; set; }

        public int DepartmentID { get; set; }

        public int DesignationId { get; set; }

        public List<Department> Department { get; set; } = new();
        public List<Designation> Designation { get; set; } = new();
        public List<Role> Role { get; set; } = new();

    }
}
