using System.ComponentModel.DataAnnotations;

namespace HrApp.MVC.Models
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "პირადი ნომერი უნდა იყოს 11 სიმბოლო")]
        public string PersonalNumber { get; set; } = null!;

        [Required] public string FirstName { get; set; } = null!;
        [Required] public string LastName { get; set; } = null!;
        public string Gender { get; set; } = "Other";
        [DataType(DataType.Date)] public DateTime BirthDate { get; set; }
        [Required, EmailAddress] public string Email { get; set; } = null!;
        [Required, DataType(DataType.Password)] public string Password { get; set; } = null!;
        [Required, DataType(DataType.Password), Compare("Password")] public string ConfirmPassword { get; set; } = null!;
    }
}
