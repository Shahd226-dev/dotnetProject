using Microsoft.AspNetCore.Authorization;
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
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var accessToken = await _authService.LoginAsync(dto);
        if (accessToken == null)
            return Unauthorized();

        SetAuthCookie(accessToken);
        return Ok("Login successful");
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var usernameExists = await _authService.UsernameExistsAsync(dto.Username);
        if (usernameExists)
            return Conflict("Username already exists");

        await _authService.RegisterAsync(dto);

        return Ok("User Registered");
    }

    [Authorize]
    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke()
    {
        var revoked = await _authService.RevokeAsync();
        if (!revoked)
            return Unauthorized();

        DeleteAuthCookie();
        return Ok("Token revoked");
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