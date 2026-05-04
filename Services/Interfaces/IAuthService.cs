public interface IAuthService
{
    Task<bool> UsernameExistsAsync(string username);
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    Task<bool> RevokeAsync();
}
