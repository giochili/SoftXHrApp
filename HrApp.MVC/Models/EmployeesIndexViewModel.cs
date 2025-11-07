using System.Collections.Generic;

namespace HrApp.MVC.Models
{
    public class EmployeesIndexViewModel
    {
        public List<EmployeeViewModel> Employees { get; set; } = new();
        public List<PositionTreeViewModel> Positions { get; set; } = new();
        public string? Query { get; set; }
    }

    public class PositionTreeViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int? ParentPositionId { get; set; }
        public List<PositionTreeViewModel> Children { get; set; } = new();
    }
}


