using Microsoft.AspNetCore.Mvc;

namespace ADF.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class UserController : BaseController
    {
        [HttpGet]
        public IActionResult Hello()
        {
            return Ok("Hello world!");
        }
    }
}