using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var tokens = await _authService.LoginAsync(dto);
        if (tokens == null)
            return Unauthorized();

        return Ok(tokens);
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

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var tokens = await _authService.RefreshAsync(dto.RefreshToken);
        if (tokens == null)
            return Unauthorized("Invalid or expired refresh token");

        return Ok(tokens);
    }

    [AllowAnonymous]
    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke(RevokeTokenRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var revoked = await _authService.RevokeAsync(dto.RefreshToken);
        if (!revoked)
            return NotFound("Refresh token not found or already inactive");

        return Ok("Refresh token revoked");
    }
}