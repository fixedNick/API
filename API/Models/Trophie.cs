using API.Data;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Trophie
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public static Trophie GetTrophieById(TrophieContext context, int id)
            => context.Trophies.Where(t => t.Id == id).FirstOrDefault();
    }
}
