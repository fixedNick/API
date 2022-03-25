using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Data
{
    public class AuthOptions
    {
        public const string ISSUER = "authServer"; // издатель токена
        public const string AUDIENCE = "resourceServer"; // потребитель токена
        const string KEY = "superscretkey123456!123";   // ключ для шифрации
        public const int LIFETIME = 360; // время жизни токена - 6 часов
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
