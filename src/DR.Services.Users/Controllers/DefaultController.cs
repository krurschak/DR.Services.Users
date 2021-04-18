using Microsoft.AspNetCore.Mvc;

namespace DR.Services.Users.Controllers
{
    [ApiController]
    [Route("")]
    public class DefaultController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
            => Ok("DR Users Service");
    }
}
