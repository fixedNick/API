using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authorization;
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
        UsersContext db;
        public AuthController(UsersContext context)
        {
            db = context;
            if (db.Users.Any() == false)
            {
                db.Users.Add(new User() { Email = "anast", Pass = "123", IsAdmin = true });
                db.Users.Add(new User() { Email = "konst", Pass = "321", IsAdmin = false });
                db.SaveChanges();
            }
        }

        // GET - Method ONLY for test
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get()
            => await db.Users.ToListAsync();

        [Route("GetAllUsers")]
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
            => await db.Users.ToListAsync();

        [Route("DeleteUser")]
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult DeleteUser([FromBody] Delete deleteData)
        {
            var user = db.Users.Where(x => x.Email == deleteData.Email).FirstOrDefault();
            if(user == null)
                return BadRequest(new { 
                    message = $"user with email {deleteData.Email} didn't found"
                });

            db.Users.Remove(user);
            db.SaveChanges();
            return Ok(new {
                message = $"User with email {deleteData.Email} successfully deleted!"
            });
        }

        [Route("SignIn")]
        [HttpPost]
        public IActionResult SignIn([FromBody] Login request)
        {
            var user = AuthUser(request.Email, request.Password);
            if (user != null)
            {
                string responseMessage = "signed-in successfully passed! Your Role is ";
                responseMessage += user.IsAdmin ? "Admin" : "User";

                var token = GenerateJWT(user);
                return Ok(new
                {
                    message = responseMessage,
                    access_token = token
                });
            }
            return Unauthorized(); // 401
        }

        [Route("SignUp")]
        [HttpPost]
        public IActionResult SignUp([FromBody] Register request)
        {
            var user = db.Users.Where(x => x.Email == request.Email).FirstOrDefault();
            if (user == null)
            {
                var addedUser = AddUser(request.Email, request.Password);
                return Ok(new { message = "user successfully added!", user = addedUser });
            }
            return Unauthorized(new { error = "Email already registered" });
        }

        private User AddUser(string email, string password)
        {
            var userToAdd = new User { Email = email, Pass = password, IsAdmin = false };
            db.Users.Add(userToAdd);
            db.SaveChanges();
            return userToAdd;
        }

        private User AuthUser(string email, string pass)
            => db.Users.Where(u => u.Email == email && u.Pass == pass).FirstOrDefault();

        private string GenerateJWT(User user)
        {
            var secretKey = AuthOptions.GetSymmetricSecurityKey();
            var creds = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.IsAdmin ? "admin" : "user")
            };

            var token = new JwtSecurityToken(
                    AuthOptions.ISSUER,
                    AuthOptions.AUDIENCE,
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(AuthOptions.LIFETIME),
                    signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
