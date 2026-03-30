using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly JwtSettings _jwtSettings;

    public AuthService(AppDbContext context, IOptions<JwtSettings> jwtOptions)
    {
        _context = context;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Username == username);
    }

    public async Task<User> RegisterAsync(RegisterDto dto)
    {
        var user = new User
        {
            Username = dto.Username,
            PasswordHash = PasswordHasher.Hash(dto.Password),
            Role = string.IsNullOrWhiteSpace(dto.Role) ? RoleConstants.User : dto.Role
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == dto.Username);

        if (user == null)
            return null;

        if (user.PasswordHash != PasswordHasher.Hash(dto.Password))
            return null;

        var refreshTokenRaw = GenerateSecureRefreshToken();
        var refreshTokenHash = HashToken(refreshTokenRaw);

        _context.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays)
        });

        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = GenerateToken(user),
            RefreshToken = refreshTokenRaw
        };
    }

    public async Task<AuthResponseDto?> RefreshAsync(string refreshToken)
    {
        var providedHash = HashToken(refreshToken);
        var existing = await _context.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == providedHash);

        if (existing == null || !existing.IsActive)
            return null;

        var newRefreshTokenRaw = GenerateSecureRefreshToken();
        var newRefreshTokenHash = HashToken(newRefreshTokenRaw);

        existing.RevokedAtUtc = DateTime.UtcNow;
        existing.ReplacedByTokenHash = newRefreshTokenHash;

        _context.RefreshTokens.Add(new RefreshToken
        {
            UserId = existing.UserId,
            TokenHash = newRefreshTokenHash,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays)
        });

        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = GenerateToken(existing.User),
            RefreshToken = newRefreshTokenRaw
        };
    }

    public async Task<bool> RevokeAsync(string refreshToken)
    {
        var providedHash = HashToken(refreshToken);
        var existing = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == providedHash);

        if (existing == null || !existing.IsActive)
            return false;

        existing.RevokedAtUtc = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
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

    private static string GenerateSecureRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
}