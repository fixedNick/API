using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Data
{
    public class AuthOptions
    {
        public string Issuer { get; set; } = "authServer";
        public string Audience { get; set; } = "resourceServer";
        public string Secret { get; set; } = "superscretkey123456";
        public int TokenLifetime { get; set; } = 60;
        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Secret));
        }

    }
}
