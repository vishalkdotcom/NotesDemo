using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using NotesDemo.Common.Auth;
using NotesDemo.Entities;

namespace NotesDemo.Helpers
{
    public static class AuthHelper
    {
        internal static Claim[] GetClaims(ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(UserClaimTypes.Gravatar, user.AvatarUrl)
            };

            return claims;
        }

        internal static string[] GetErrors(IdentityResult result)
        {
            return result.Errors.Select(x => x.Description).ToArray(); ;
        }
    }
}