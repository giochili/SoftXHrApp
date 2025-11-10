using System;

namespace HrApp.MVC.Models
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }
        public string PersonalNumber { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; } = null!;
        public int PositionId { get; set; }
        public int Status { get; set; }
        public DateTime? DismissalDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public PositionSummaryViewModel? Position { get; set; }
        public string PositionTitle => Position?.Title ?? string.Empty;
    }

    public class PositionSummaryViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int? ParentPositionId { get; set; }
    }
}
