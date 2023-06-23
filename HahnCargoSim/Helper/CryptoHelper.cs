using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace HahnCargoSim.Helper
{
  public static class CryptoHelper
  {
    public static string GetPasswordHash(string password)
    {
      var salt = RandomNumberGenerator.GetBytes(16);

      var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
      var hash = pbkdf2.GetBytes(20);

      var hashBytes = new byte[36];
      Array.Copy(salt, 0, hashBytes, 0, 16);
      Array.Copy(hash, 0, hashBytes, 16, 20);

      return Convert.ToBase64String(hashBytes);
    }

    public static bool IsAuthorized(string password, string passwordHash)
    {
      var hashBytes = Convert.FromBase64String(passwordHash);

      var salt = new byte[16];
      Array.Copy(hashBytes, 0, salt, 0, 16);

      var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
      var hash = pbkdf2.GetBytes(20);

      var differenceFound = false;
      for (var i = 0; i < 20; i++)
      {
        if (hashBytes[i + 16] == hash[i]) continue;
        differenceFound = true;
        break;
      }
      return !differenceFound;
    }

    public static string GetJwtTokenString(string tokenSecret, string userName)
    {
      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.ASCII.GetBytes(tokenSecret);
      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new Claim[] {
                        new Claim(ClaimTypes.Name, userName.ToString())
                    }),
        Expires = DateTime.UtcNow.AddDays(7),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
      };

      var token = tokenHandler.CreateToken(tokenDescriptor);
      return tokenHandler.WriteToken(token);
    }
  }
}
