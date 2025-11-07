using System.ComponentModel.DataAnnotations;

namespace HrApp.MVC.Models
{
    public class PositionViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "დასახელება სავალდებულოა")]
        public string Title { get; set; } = null!;
        public int? ParentPositionId { get; set; }
        public string? ParentTitle { get; set; }
    }
}

