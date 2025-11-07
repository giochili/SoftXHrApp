using System;

namespace HrApp.Core.DTOs
{
    public class CreateEmployeeDto
    {
        public string PersonalNumber { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; } = null!;
        public int PositionId { get; set; }
        public int Status { get; set; }
        public DateTime? DismissalDate { get; set; }
    }
}


