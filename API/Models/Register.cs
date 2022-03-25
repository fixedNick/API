using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Register
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public bool? IsAdmin{ get; set; }

    }
}
