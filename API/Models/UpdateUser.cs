using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class UpdateUser
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public bool? IsAdmin { get; set; }
    }
}
