using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NotesDemo.Entities;
using NotesDemo.Helpers;
using NotesDemo.Models;
using NotesDemo.Services;

namespace NotesDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;

        public UsersController(IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService)
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        // POST: users
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<JwtTokenModel>> Register([FromBody]RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var gravatarUrl = GravatarHelper.GetAvatarUrl(model.Email);
                var user = new ApplicationUser
                {
                    UserName = model.Name,
                    Email = model.Email,
                    AvatarUrl = gravatarUrl
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);

                    var jwtToken = await _tokenService.GetJwtToken(user);
                    return jwtToken;
                }
                var errors = AuthHelper.GetErrors(result);
                return BadRequest(errors);
            }

            return BadRequest(ModelState);
        }
    }
}