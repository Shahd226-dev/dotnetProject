using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtSettings _jwtSettings;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(
        IUserRepository userRepository,
        IOptions<JwtSettings> jwtOptions,
        IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _jwtSettings = jwtOptions.Value;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _userRepository.UsernameExistsAsync(username);
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var resolvedRole = ResolveRole(dto.Role);
        if (resolvedRole == null)
            throw new InvalidOperationException("Invalid role.");

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = PasswordHasher.Hash(dto.Password),
            Role = resolvedRole,
            TokenVersion = 0
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = string.Empty,
            Username = user.Username,
            Role = user.Role
        };
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByUsernameAsync(dto.Username);

        if (user == null)
            return null;

        if (user.PasswordHash != PasswordHasher.Hash(dto.Password))
            return null;

        return new AuthResponseDto
        {
            AccessToken = GenerateToken(user),
            Username = user.Username,
            Role = user.Role
        };
    }

    public async Task<bool> RevokeAsync()
    {
        var username = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
        if (string.IsNullOrWhiteSpace(username))
            return false;

        var user = await _userRepository.GetByUsernameForUpdateAsync(username);

        if (user == null)
            return false;

        user.TokenVersion += 1;
        await _userRepository.SaveChangesAsync();
        return true;
    }

    private string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("token_version", user.TokenVersion.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string? ResolveRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            return RoleConstants.User;

        if (string.Equals(role, RoleConstants.Admin, StringComparison.OrdinalIgnoreCase))
            return RoleConstants.Admin;

        if (string.Equals(role, RoleConstants.Instructor, StringComparison.OrdinalIgnoreCase))
            return RoleConstants.Instructor;

        if (string.Equals(role, RoleConstants.User, StringComparison.OrdinalIgnoreCase))
            return RoleConstants.User;

        return null;
    }
}
