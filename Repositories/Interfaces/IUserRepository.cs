public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByUsernameForUpdateAsync(string username);
    Task<User?> GetByIdAsync(int id);
    Task<bool> UsernameExistsAsync(string username);
    Task AddAsync(User user);
    Task SaveChangesAsync();
}
