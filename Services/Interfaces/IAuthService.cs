public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    Task<List<UserResponseDto>> GetAllUsersAsync();
    Task<bool> DeleteUserAsync(int userId);
}
