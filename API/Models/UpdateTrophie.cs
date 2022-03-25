using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class UpdateTrophie
    {
        [Required]
        public string NewName { get; set; }
        [Required]
        public string OldName { get; set; }
    }
}
