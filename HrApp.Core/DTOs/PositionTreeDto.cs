using System.Collections.Generic;

namespace HrApp.Core.DTOs
{
    public class PositionTreeDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int? ParentPositionId { get; set; }
        public List<PositionTreeDto> Children { get; set; } = new();
    }
}


