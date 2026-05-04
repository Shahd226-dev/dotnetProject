public interface ICourseRepository
{
    Task<List<Course>> GetAllAsync();
    Task<Course?> GetByIdAsync(int id);
    Task<Course?> GetByIdForUpdateAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task AddAsync(Course course);
    Task SaveChangesAsync();
}
