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

        [Route("GetAllUsers")]
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            if (Request.Cookies.TryGetValue("Id", out string? cookie) == false)
                return Unauthorized(new { error = "Вы не авторизованы" });

            return await usersDb.Users.ToListAsync();
        }

        [Route("DeleteUser")]
        [Authorize(Roles = "admin")]
        [HttpDelete]
        public IActionResult DeleteUser([FromBody] Delete deleteData)
        {
            if (Request.Cookies.TryGetValue("Id", out string? cookie) == false)
                return Unauthorized(new { error = "Вы не авторизованы" });

            var user = usersDb.Users.Where(x => x.Email == deleteData.Email).FirstOrDefault();
            if (user == null)
                return NotFound(new
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

        [Route("UpdateUser")]
        [Authorize(Roles = "admin")]
        [HttpPut]
        public IActionResult UpdateUser([FromBody] UpdateUser updateInfo)
        {
            if (Request.Cookies.TryGetValue("Id", out string? cookie) == false)
                return Unauthorized(new { error = "Вы не авторизованы" });

            var user = usersDb.Users.Where(u => u.Email == updateInfo.Email).FirstOrDefault();

            if (updateInfo.IsAdmin == null)
                return BadRequest(new { error = "Field 'IsAdmin' required"});

            if (user == null)
                return NotFound(new { message = $"User with email '{updateInfo.Email}' not found"});
            user.IsAdmin = (bool)updateInfo.IsAdmin;
            usersDb.Update(user);
            usersDb.SaveChanges();
            return Ok(new { 
                message = "User info updated successfully!", 
                users = usersDb.Users
            });
        }

        [Route("CreateTrophie")]
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult CreateTrophie([FromBody] Trophie trophie)
        {
            if (Request.Cookies.TryGetValue("Id", out string? cookie) == false)
                return Unauthorized(new { error = "Вы не авторизованы" });

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

        [Route("DeleteTrophie")]
        [Authorize(Roles = "admin")]
        [HttpDelete]
        public IActionResult DeleteTrophie([FromBody] Trophie tr)
        {
            if (Request.Cookies.TryGetValue("Id", out string? cookie) == false)
                return Unauthorized(new { error = "Вы не авторизованы" });

            var trop = trophiesDb.Trophies.Where(t => t.Name == tr.Name).FirstOrDefault();
            if (trop == null)
                return NotFound(new { 
                    message = "Trophie with that name doesn't exists"
                });

            trophiesDb.Trophies.Remove(trop);
            trophiesDb.SaveChanges();
            return Ok(new { 
                message = $"Trophie '{tr.Name}' have been deleted"
            });
        }

        [Route("UpdateTrophie")]
        [Authorize(Roles = "admin")]
        [HttpPut]
        public IActionResult UpdateTrophie([FromBody] UpdateTrophie updateInfo)
        {
            if (Request.Cookies.TryGetValue("Id", out string? cookie) == false)
                return Unauthorized(new { error = "Вы не авторизованы" });

            var trop = trophiesDb.Trophies.Where(t => t.Name == updateInfo.OldName).FirstOrDefault();
            if (trop == null)
                return NotFound(new
                {
                    message = "Trophie with that name doesn't exists"
                });

            trop.Name = updateInfo.NewName;
            trophiesDb.SaveChanges();
            return Ok(new { 
                TableView = trophiesDb.Trophies 
            });
        }

        [Route("GetAllUsersAndAllTrophies")]
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult GetAllUsersAndAllTrophies()
        {
            if (Request.Cookies.TryGetValue("Id", out string? cookie_uid) == false)
                return Unauthorized(new { error = "Вы не авторизованы" });

            // Применяем LINQ Select, чтобы из одной обширной коллекции сформировать более узконаправленную коллекцию
            // Каждый вызов new {} внутри Select создает новый элемент коллекции, из этих элементов и формируется коллекция
            // Коллекция в данном случае = vector(c++) <-> List <-> Array - Набор элементов


            return Ok(
                new {
                    users = usersDb
                                .Users
                                .Select(u => // Идея данного селекта в том, чтобы скрыть(сформировать новую коллекцию) пароль, оставить: id/email/trophieslist/isadmin
                                new {
                                    id = u.Id,
                                    email = u.Email,
                                    trophies = u.TrophiesList,
                                    isAdmin = u.IsAdmin
                                }),
                    trophies = trophiesDb
                                .Trophies
                                .Select(t => // Идея данного селекта в том, чтобы скрыть(сформировать новую коллекцию) id заслуги
                                    new { 
                                        name = t.Name
                                    })
                });
        }
    }
}
