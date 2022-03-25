namespace API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Pass { get; set; }
        public bool IsAdmin { get; set; }
    }
}
