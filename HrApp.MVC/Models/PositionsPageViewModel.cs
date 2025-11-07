using System.Collections.Generic;

namespace HrApp.MVC.Models
{
    public class PositionsPageViewModel
    {
        public List<PositionViewModel> Positions { get; set; } = new();
        public List<PositionTreeViewModel> Tree { get; set; } = new();
    }
}

