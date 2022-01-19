using System.ComponentModel.DataAnnotations;

namespace API.Data.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }

        [StringLength(8, MinimumLength = 4)]
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
    }
}
