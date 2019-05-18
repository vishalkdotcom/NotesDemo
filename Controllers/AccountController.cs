using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotesDemo.Entities;

namespace NotesDemo.Controllers
{
    [Authorize]
    [Route("api/my-profile")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: me
        public async Task<IActionResult> Get()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var userInfo = new
                {
                    email = user.Email,
                    name = user.UserName,
                    avatar_url = user.AvatarUrl
                };
                return Ok(userInfo);
            }

            return BadRequest();
        }
    }
}
