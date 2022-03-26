using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        UsersContext UsersDb { get; set; }
        TrophieContext TrophieDb { get; set; }

        public UserController(UsersContext userContext, TrophieContext trophieContext)
        {
            UsersDb = userContext;
            TrophieDb = trophieContext;
        }

        [Route("GetAllTrophies")]
        [Authorize(Roles = "admin,user")]
        [HttpPost]
        public IActionResult GetAllTrophies()
        {
            if (Request.Cookies.TryGetValue("Id", out string? cookie) == false)
                return Unauthorized(new { error = "Вы не авторизованы" });

            return Ok(TrophieDb.Trophies);
        }

        [Route("AddTrophie/{id}")]
        [Authorize(Roles = "admin,user")]
        [HttpPut]
        public IActionResult AddTrophie(int id)
        {
            if (Request.Cookies.TryGetValue("Id", out string? cookie) == false)
                return Unauthorized(new { error = "Вы не авторизованы" });

            var trophie = TrophieDb.Trophies.Where(t => t.Id == id).FirstOrDefault();
            if (trophie == null)
                return NotFound(new { error = $"Trophie with id: '{id}' not found" });

            if (Request.Cookies.TryGetValue("Id", out string? currentUserId) == false)
                return BadRequest(new
                {
                    error = "Login first"
                });

            var user = UsersDb.Users.Where(u => u.Id == Convert.ToInt32(currentUserId)).FirstOrDefault();
            if (user == null)
                return NotFound(new
                {
                    error = $"Cannot find user with id: '{currentUserId}'. Id got from cookies"
                });

            var trophieById = Trophie.GetTrophieById(TrophieDb, id);
            var res = user.AddTrophie(trophie);
            if (res == false)
                return BadRequest(new
                {
                    error = $"Trophie with name: '{trophieById.Name}' already added to user '{user.Email}'"
                });
            UsersDb.SaveChanges();
            return Ok(new
            {
                message = $"Trophie with name '{trophieById.Name} successfully added to user with email '{user.Email}'"
            });
        }

        [Route("RemoveTrophie/{id}")]
        [Authorize(Roles = "admin,user")]
        [HttpPut]
        public IActionResult RemoveTrophie(int id)
        {
            if (Request.Cookies.TryGetValue("Id", out string? cookie_uid) == false)
                return Unauthorized(new { error = "Вы не авторизованы" });

            // LINQ Select & Where - А мог ли я придумать применение тупее?

            var user = UsersDb.Users.Where(u => u.Id == Convert.ToInt32(cookie_uid)).FirstOrDefault();
            var trophie = user.TrophiesList.Where(t => t.Id == id).FirstOrDefault();
            if (trophie == null)
                return NotFound(new { error = $"User '{user.Email}' doesn't have trophie with id'{id}'" });

            var trophieName = trophie.Name;

            user.RemoveTrophieByName(trophieName);

            UsersDb.SaveChanges();
            return Ok(new
            {
                message = $"Trophie '{trophieName}' removed from account '{user.Email}'"
            });
        }
    }
}
