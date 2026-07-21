using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace TestApp.Server.Auth
{
  /// <summary>
  /// Issues signed JWT access tokens for authenticated users.
  /// </summary>
  public interface IJwtTokenService
  {
    /// <summary>
    /// Creates a signed JWT for the given username.
    /// </summary>
    (string Token, DateTime ExpiresAtUtc) GenerateToken(string username);
  }

  /// <inheritdoc/>
  public class JwtTokenService : IJwtTokenService
  {
    private readonly IConfiguration configuration;

    public JwtTokenService(IConfiguration configuration)
    {
      this.configuration = configuration;
    }

    /// <inheritdoc/>
    public (string Token, DateTime ExpiresAtUtc) GenerateToken(string username)
    {
      var jwtSection = configuration.GetSection("Jwt");
      var key = jwtSection["Key"]
          ?? throw new InvalidOperationException("Jwt:Key is not configured.");
      var issuer = jwtSection["Issuer"];
      var audience = jwtSection["Audience"];
      var expiryMinutes = int.TryParse(jwtSection["ExpiryMinutes"], out var m) ? m : 60;

      var expiresAtUtc = DateTime.UtcNow.AddMinutes(expiryMinutes);

      var claims = new[]
      {
        new Claim(JwtRegisteredClaimNames.Sub, username),
        new Claim(ClaimTypes.Name, username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
      };

      var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
      var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

      var token = new JwtSecurityToken(
          issuer: issuer,
          audience: audience,
          claims: claims,
          expires: expiresAtUtc,
          signingCredentials: credentials);

      var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
      return (tokenString, expiresAtUtc);
    }
  }
}
