using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotesDemo.Entities;
using NotesDemo.Models;
using NotesDemo.Services;

namespace NotesDemo.Controllers
{
    [Authorize]
    [Route("api/access-tokens")]
    [ApiController]
    public class AccessTokensController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AccessTokensController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        // POST: /access-tokens
        [Consumes("application/json")]
        [Produces("application/json")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<JwtTokenModel>> Post([FromBody] LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                    if (result.Succeeded)
                    {
                        var jwtToken = await _tokenService.GetJwtToken(user);
                        return jwtToken;
                    }
                }
                return BadRequest("Invalid username or password");
            }

            return BadRequest(ModelState);
        }

        // POST: /access-tokens/refresh
        [Consumes("application/json")]
        [Produces("application/json")]
        [HttpPost("{refresh_token}", Name = "Refresh")]
        public async Task<ActionResult<JwtTokenModel>> Post([FromBody] string refresh_token)
        {
            var refreshTokenFromDb = await _tokenService.GetRefreshToken(refresh_token);
            if (refreshTokenFromDb == null)
            {
                return BadRequest();
            }
            if (refreshTokenFromDb.ExpiresUtc < DateTime.Now.ToUniversalTime())
                return Unauthorized();

            if (!await _signInManager.CanSignInAsync(refreshTokenFromDb.User))
                return Unauthorized();

            var user = refreshTokenFromDb.User;
            var jwtToken = await _tokenService.GetJwtToken(user);
            return jwtToken;
        }

        // DELETE: /access-tokens
        [Consumes("application/json")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody]string refresh_token)
        {
            var refreshToken = await _tokenService.GetRefreshToken(refresh_token);
            if (refreshToken == null)
            {
                return BadRequest();
            }

            await _tokenService.RemoveRefreshToken(refreshToken.Token);
            return NoContent();
        }
    }
}