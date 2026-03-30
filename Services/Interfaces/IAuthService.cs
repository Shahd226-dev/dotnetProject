public interface IAuthService
{
    Task<User?> GetByUsernameAsync(string username);
    Task<bool> UsernameExistsAsync(string username);
    Task<User> RegisterAsync(RegisterDto dto);
    string GenerateToken(User user);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    Task<AuthResponseDto?> RefreshAsync(string refreshToken);
    Task<bool> RevokeAsync(string refreshToken);
}