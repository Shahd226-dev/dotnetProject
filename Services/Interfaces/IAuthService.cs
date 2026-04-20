public interface IAuthService
{
    Task<bool> UsernameExistsAsync(string username);
    Task<User> RegisterAsync(RegisterDto dto);
    Task<string?> LoginAsync(LoginDto dto);
    Task<bool> RevokeAsync();
}