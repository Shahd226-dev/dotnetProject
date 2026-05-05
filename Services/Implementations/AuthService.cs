using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IInstructorRepository _instructorRepository;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        IUserRepository userRepository,
        IStudentRepository studentRepository,
        IInstructorRepository instructorRepository,
        IOptions<JwtSettings> jwtOptions)
    {
        _userRepository = userRepository;
        _studentRepository = studentRepository;
        _instructorRepository = instructorRepository;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var resolvedRole = ResolveRole(dto.Role);
        if (resolvedRole == null)
            throw new InvalidOperationException("Invalid role.");

        var normalizedUsername = dto.Username.Trim();
        var normalizedEmail = dto.Email.Trim();

        if (await _userRepository.UsernameExistsAsync(normalizedUsername))
            throw new ConflictException("Username already exists.");

        if (await _userRepository.EmailExistsAsync(normalizedEmail))
            throw new ConflictException("Email already exists.");

        if (resolvedRole != RoleConstants.Admin && string.IsNullOrWhiteSpace(dto.FullName))
            throw new InvalidOperationException("Full name is required for students and instructors.");

        var user = new User
        {
            Username = normalizedUsername,
            Email = normalizedEmail,
            PasswordHash = PasswordHasher.Hash(dto.Password),
            Role = resolvedRole,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        if (resolvedRole == RoleConstants.Student)
        {
            var student = new Student
            {
                FullName = dto.FullName!.Trim(),
                UserId = user.Id
            };

            await _studentRepository.AddAsync(student);
            await _studentRepository.SaveChangesAsync();
        }

        if (resolvedRole == RoleConstants.Instructor)
        {
            var instructor = new Instructor
            {
                FullName = dto.FullName!.Trim(),
                Bio = dto.Bio?.Trim() ?? string.Empty,
                UserId = user.Id
            };

            await _instructorRepository.AddAsync(instructor);
            await _instructorRepository.SaveChangesAsync();
        }

        return new AuthResponseDto
        {
            AccessToken = GenerateToken(user),
            User = MapUser(user)
        };
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByUsernameAsync(dto.Username.Trim());
        if (user == null)
            return null;

        if (!PasswordHasher.Verify(dto.Password, user.PasswordHash))
            return null;

        return new AuthResponseDto
        {
            AccessToken = GenerateToken(user),
            User = MapUser(user)
        };
    }

    public async Task<List<UserResponseDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapUser).ToList();
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        var user = await _userRepository.GetByIdForUpdateAsync(userId);
        if (user == null)
            return false;

        await _userRepository.DeleteAsync(user);
        await _userRepository.SaveChangesAsync();
        return true;
    }

    private string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
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

    private static string? ResolveRole(string role)
    {
        if (string.Equals(role, RoleConstants.Admin, StringComparison.OrdinalIgnoreCase))
            return RoleConstants.Admin;

        if (string.Equals(role, RoleConstants.Instructor, StringComparison.OrdinalIgnoreCase))
            return RoleConstants.Instructor;

        if (string.Equals(role, RoleConstants.Student, StringComparison.OrdinalIgnoreCase))
            return RoleConstants.Student;

        return null;
    }

    private static UserResponseDto MapUser(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }
}
