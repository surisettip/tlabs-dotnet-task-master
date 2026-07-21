namespace TestApp.Server.Auth
{
  /// <summary>
  /// Login request payload.
  /// </summary>
  public class LoginRequest
  {
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
  }

  /// <summary>
  /// Login response payload with the issued bearer token.
  /// </summary>
  public class LoginResponse
  {
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
  }
}
