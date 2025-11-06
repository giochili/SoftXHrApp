using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HrApp.Core.DTOs
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string PersonalNumber { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public DateTime BirthDate { get; set; }
        public int PositionId { get; set; }
        public string Status { get; set; } = null!;
        public DateTime? DismissalDate { get; set; }
    }
}
