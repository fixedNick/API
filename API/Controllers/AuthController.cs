using API.Data;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        IOptions<AuthOptions> authOpts;
        UsersContext db;
        public AuthController(UsersContext context, IOptions<AuthOptions> authOpts)
        {
            this.authOpts = authOpts;
            db = context;
            if (db.Users.Any() == false)
            {
                db.Users.Add(new User() { Email = "anast", Pass = "123", IsAdmin = true });
                db.Users.Add(new User() { Email = "konst", Pass = "321", IsAdmin = false });
                db.SaveChanges();
            }
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<User>>> Get()
        //    => await db.Users.ToListAsync();


        [Route("login")]
        [HttpPost]
        public IActionResult Login([FromBody] Login request)
        {
            var user = AuthUser(request.Email, request.Password);
            if (user != null)
            {
                var token = GenerateJWT(user);
                return Ok(new
                {
                    access_token = token
                });
            }
            return Unauthorized(); // 401
        }

        private User AuthUser(string email, string pass)
            => db.Users.SingleOrDefault(u => u.Email == email && u.Pass == pass);

        private string GenerateJWT(User user)
        {
            var authParams = authOpts.Value;
            var secretKey = authParams.GetSymmetricSecurityKey();
            var creds = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Clain(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.IsAdmin ? "admin" : "user")
            };

            var token = new JwtSecurityToken(
                    authParams.Issuer,
                    authParams.Audience,
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(authParams.TokenLifetime),
                    signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
