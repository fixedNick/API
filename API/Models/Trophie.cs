using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Trophie
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
