using API.Data;
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
        TrophieContext TrophieDb { get; set;}

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
            return Ok(TrophieDb.Trophies);
        }

    }
}
