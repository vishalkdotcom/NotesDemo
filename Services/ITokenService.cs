using System.Security.Claims;
using System.Threading.Tasks;
using NotesDemo.Entities;

namespace NotesDemo.Services
{
    public interface ITokenService
    {
        Task<JwtTokenModel> GetJwtToken(ApplicationUser user);

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

        Task AddOrUpdateRefreshToken(string userId, string token);

        Task<RefreshToken> GetRefreshToken(string token);

        Task RemoveRefreshToken(string userId);
    }

}