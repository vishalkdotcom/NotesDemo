using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NotesDemo.Entities;
using NotesDemo.Helpers;

namespace NotesDemo.Services
{
    public class TokenService : ITokenService
    {
        private readonly NotesDbContext _dbContext;

        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration, NotesDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public async Task<JwtTokenModel> GetJwtToken(ApplicationUser user)
        {
            var claims = AuthHelper.GetClaims(user);

            var jwtToken = this.GenerateAccessToken(claims);
            var refreshToken = this.GenerateRefreshToken();

            await this.AddOrUpdateRefreshToken(user.Id, refreshToken);

            return new JwtTokenModel
            {
                AccessToken = jwtToken,
                RefreshToken = refreshToken
            };
        }

        private string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["JwtExpireMinutes"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtIssuer"],
                audience: _configuration["JwtIssuer"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["JwtIssuer"],
                ValidAudience = _configuration["JwtIssuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        public async Task AddOrUpdateRefreshToken(string userId, string token)
        {
            await RemoveRefreshToken(userId);

            var newRefreshToken = new RefreshToken
            {
                UserId = userId,
                Token = token,
                IssuedUtc = DateTime.Now.ToUniversalTime(),
                ExpiresUtc = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["JwtExpireMinutes"]))
            };

            await _dbContext.RefreshTokens.AddAsync(newRefreshToken);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<RefreshToken> GetRefreshToken(string token)
        {
            return await _dbContext.RefreshTokens.SingleOrDefaultAsync(i => i.Token == token);
        }

        public async Task RemoveRefreshToken(string userId)
        {
            var refreshTokenToRemove = await GetRefreshTokenByUserId(userId);
            if (refreshTokenToRemove != null)
            {
                _dbContext.RefreshTokens.Remove(refreshTokenToRemove);
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task<RefreshToken> GetRefreshTokenByUserId(string userId)
        {
            return await _dbContext.RefreshTokens.SingleOrDefaultAsync(i => i.UserId == userId);
        }
    }
}