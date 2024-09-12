using System.ComponentModel.DataAnnotations;

namespace alot.Data
{
    public class RegisterDto
    {
        [Required]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "The username can only contain letters and digits.")]
        public required string UserName { get; set; }
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}
