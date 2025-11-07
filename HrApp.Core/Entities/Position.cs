using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HrApp.Core.Entities
{
    public class Position
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int? ParentPositionId { get; set; }

        public Position? ParentPosition { get; set; }
        public ICollection<Position>? SubPositions { get; set; } = new List<Position>();
    }
}
