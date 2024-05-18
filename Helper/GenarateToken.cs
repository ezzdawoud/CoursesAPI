using Courses.Data;
using Courses.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Courses.Helper
{
    public class GenarateToken
    {
        private readonly UserManager<Users> _userManeger;
        private readonly Connections _dbContext;

        public GenarateToken(UserManager<Users> userManeger, Connections dbContext)
        {
            _userManeger = userManeger;
            _dbContext = dbContext;

        }
        public async Task<string> GenerateJwtToken(Users user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // Define your secret key 
            var key = "YourSuperSecretKey123"; // Replace with your secret key

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
            // Add more claims as needed
        }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(jwtToken));

            // Store the token for the user
            await StoreTokenInUser(user, encodedToken);

            return encodedToken;
        }

        public async Task<Users> GetUserFromToken(string token)
        {
            // Find the token in the database
            var userToken = await _dbContext.Set<IdentityUserToken<string>>()
                .FirstOrDefaultAsync(t => t.Value == token);

            if (userToken != null)
            {
                // Retrieve the user based on the user ID associated with the token
                var user = await _userManeger.FindByIdAsync(userToken.UserId);
                return user;
            }
            else
            {
                // Token not found or user not associated with the token
                return null;
            }
        }

        public async Task<bool> StoreTokenInUser(Users user, string token)
        {
            var result = await _userManeger.SetAuthenticationTokenAsync(user, "MyAuth", "TokenName", token);

            if (result.Succeeded)
            {
                // Token stored successfully
                return true;
            }
            else
            {
                // Token storage failed
                return false;
            }
        }
        public async Task RemoveTokenFromUser(Users user)
        {
            await _userManeger.RemoveAuthenticationTokenAsync(user, "MyAuth", "TokenName");
        }
    }
}
