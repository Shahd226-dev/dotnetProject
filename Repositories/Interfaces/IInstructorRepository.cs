public interface IInstructorRepository
{
    Task<List<Instructor>> GetAllAsync();
    Task<Instructor?> GetByIdAsync(int id);
    Task<Instructor?> GetByIdForUpdateAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> UserLinkedToInstructorAsync(int userId, int? excludeInstructorId = null);
    Task AddAsync(Instructor instructor);
    Task SaveChangesAsync();
}
