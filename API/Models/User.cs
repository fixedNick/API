using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Pass { get; set; }
        public bool IsAdmin { get; set; }
        public string Trophies { get; set; } = "";

        [NotMapped]
        private List<Trophie> TrophiesList { get; set; } = new List<Trophie>();

        public void InitializeTrophiesList()
        {
            TrophiesList = JsonConvert.DeserializeObject<List<Trophie>>(Trophies);
        }

        // Добавляем новое достижение
        public bool AddTrophie(Trophie trophie)
        {
            if (TrophiesList.Where(t => t.Name == trophie.Name).FirstOrDefault() != null)
                return false;

            TrophiesList.Add(trophie);
            Trophies = JsonConvert.SerializeObject(TrophiesList, Formatting.Indented);
            return true;
        }

        // Удаляем достижение по его названию
        public bool RemoveTrophieByName(string name)
        {
            int? trophieId = TrophiesList.Where(x => x.Name == name).FirstOrDefault().Id;
            if (trophieId == null)
                return false;

            TrophiesList.Remove(TrophiesList.Where(x => x.Id == trophieId).FirstOrDefault());
            Trophies = JsonConvert.SerializeObject(TrophiesList, Formatting.Indented);
            return true;
        }

    }
}
