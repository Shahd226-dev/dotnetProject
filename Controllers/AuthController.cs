using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        var authResponse = await _authService.LoginAsync(dto);
        if (authResponse == null)
            return Unauthorized(ApiResponse<object?>.Fail("Invalid username or password."));
        return Ok(ApiResponse<AuthResponseDto>.Ok(authResponse, "Login successful."));
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var registeredUser = await _authService.RegisterAsync(dto);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<AuthResponseDto>.Ok(registeredUser, "User registered."));
    }

    [Authorize(Roles = RoleConstants.Admin)]
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _authService.GetAllUsersAsync();
        return Ok(ApiResponse<List<UserResponseDto>>.Ok(users, "Users retrieved."));
    }

    [Authorize(Roles = RoleConstants.Admin)]
    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var deleted = await _authService.DeleteUserAsync(id);
        if (!deleted)
            return NotFound(ApiResponse<object?>.Fail("User not found."));

        return Ok(ApiResponse<object?>.Ok(null, "User deleted."));
    }
}
