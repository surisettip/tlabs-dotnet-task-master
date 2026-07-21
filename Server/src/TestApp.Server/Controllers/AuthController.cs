using Microsoft.AspNetCore.Mvc;
using TestApp.Server.Auth;

namespace TestApp.Server.Controllers
{
  /// <summary>
  /// Handles authentication. Demo-only: validates against a single
  /// configured user (see appsettings.json -> DemoUser). In a real
  /// application this would check a user store with hashed passwords.
  /// </summary>
  [ApiController]
  [Route("api/auth")]
  public class AuthController : ControllerBase
  {
    private readonly IJwtTokenService tokenService;
    private readonly IConfiguration configuration;

    public AuthController(IJwtTokenService tokenService, IConfiguration configuration)
    {
      this.tokenService = tokenService;
      this.configuration = configuration;
    }

    /// <summary>
    /// Exchanges username/password for a JWT bearer token.
    /// Use the returned token in the "Authorize" button in Swagger
    /// (as "Bearer &lt;token&gt;") to call the protected /api/tasks endpoints.
    /// </summary>
    [HttpPost("login")]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
    {
      var demoUsername = configuration["DemoUser:Username"];
      var demoPassword = configuration["DemoUser:Password"];

      if (request is null
          || string.IsNullOrEmpty(request.Username)
          || string.IsNullOrEmpty(request.Password)
          || !string.Equals(request.Username, demoUsername, StringComparison.Ordinal)
          || !string.Equals(request.Password, demoPassword, StringComparison.Ordinal))
      {
        return Unauthorized(new { message = "Invalid username or password." });
      }

      var (token, expiresAtUtc) = tokenService.GenerateToken(request.Username);
      return Ok(new LoginResponse { Token = token, ExpiresAtUtc = expiresAtUtc });
    }
  }
}
