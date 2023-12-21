using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Task_Manager.Authentication
{
    public class JwtService
    {
        private readonly AuthOptions _authOptions;

        public JwtService(AuthOptions authOptions)
        {
            _authOptions = authOptions;
        }

        public string GenerateJwtToken(Guid userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = _authOptions.Key; // Получение ключа

            if (string.IsNullOrEmpty(key))
            {
                // Генерация нового ключа
                var newKey = AuthOptions.GenerateBase64Key();
                _authOptions.Key = newKey;
                key = newKey;
            }

            var keyBytes = Convert.FromBase64String(key);
            var securityKey = new SymmetricSecurityKey(keyBytes);

            var claims = new[]
            {
            new Claim("UserId", userId.ToString())
        };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(60),
                NotBefore = DateTime.UtcNow,
                Issuer = _authOptions.Issuer,
                Audience = _authOptions.Audience,
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
