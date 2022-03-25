using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        UsersContext usersDb;
        TrophieContext trophiesDb;
        public AdminController(UsersContext ucontext, TrophieContext tcontext)
        {
            usersDb = ucontext;
            trophiesDb = tcontext;
        }

        // GET - Method ONLY for test
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get()
            => await usersDb.Users.ToListAsync();

        [Route("GetAllUsers")]
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
            => await usersDb.Users.ToListAsync();

        [Route("DeleteUser")]
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult DeleteUser([FromBody] Delete deleteData)
        {
            var user = usersDb.Users.Where(x => x.Email == deleteData.Email).FirstOrDefault();
            if (user == null)
                return BadRequest(new
                {
                    message = $"user with email {deleteData.Email} didn't found"
                });

            usersDb.Users.Remove(user);
            usersDb.SaveChanges();
            return Ok(new
            {
                message = $"User with email {deleteData.Email} successfully deleted!"
            });
        }

        [Route("CreateTrophie")]
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult CreateTrophie([FromBody] Trophie trophie)
        {
            if (trophiesDb.Trophies.Where(t => t.Name == trophie.Name).FirstOrDefault() != null)
                return BadRequest(new {
                    message = "Trophie with that name already exists"
                });

            trophiesDb.Trophies.Add(trophie);
            trophiesDb.SaveChanges();
            return Ok(new { 
                message = $"Trophie {trophie.Name} successfully Added!"
            });
        }
    }
}
