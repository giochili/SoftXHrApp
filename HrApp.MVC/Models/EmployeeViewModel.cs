namespace HrApp.MVC.Models
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Position { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
