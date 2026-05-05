public interface ICourseRepository
{
    Task<List<Course>> GetAllAsync();
    Task<List<Course>> GetByInstructorIdAsync(int instructorId);
    Task<Course?> GetByIdAsync(int id);
    Task<Course?> GetByIdForUpdateAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task AddAsync(Course course);
    Task DeleteAsync(Course course);
    Task SaveChangesAsync();
}
