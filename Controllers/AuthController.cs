using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly JwtSettings _jwtSettings;

    public AuthController(IAuthService authService, IOptions<JwtSettings> jwtOptions)
    {
        _authService = authService;
        _jwtSettings = jwtOptions.Value;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var authResponse = await _authService.LoginAsync(dto);
        if (authResponse == null)
            return Unauthorized(ApiResponse<object?>.Fail("Invalid username or password."));

        SetAuthCookie(authResponse.AccessToken);
        return Ok(ApiResponse<AuthResponseDto>.Ok(authResponse, "Login successful."));
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var usernameExists = await _authService.UsernameExistsAsync(dto.Username);
        if (usernameExists)
            return Conflict(ApiResponse<object?>.Fail("Username already exists."));

        var registeredUser = await _authService.RegisterAsync(dto);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<AuthResponseDto>.Ok(registeredUser, "User registered."));
    }

    [Authorize]
    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke()
    {
        var revoked = await _authService.RevokeAsync();
        if (!revoked)
            return Unauthorized(ApiResponse<object?>.Fail("Unauthorized."));

        DeleteAuthCookie();
        return Ok(ApiResponse<object?>.Ok(null, "Token revoked."));
    }

    private void SetAuthCookie(string accessToken)
    {
        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            Path = "/"
        };

        Response.Cookies.Append("access_token", accessToken, options);
    }

    private void DeleteAuthCookie()
    {
        Response.Cookies.Delete("access_token", new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Path = "/"
        });
    }
}
