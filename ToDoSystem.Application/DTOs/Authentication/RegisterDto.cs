#nullable disable
using System.ComponentModel.DataAnnotations;

namespace ToDoSystem.Application.DTOs.Authentication
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "UserName is required"), StringLength(50)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Phone Number is required"), Phone]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email is required"), StringLength(128), EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required"), StringLength(256)]
        public string Password { get; set; }
    }
}
