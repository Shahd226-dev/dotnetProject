public interface IStudentRepository
{
    Task<List<Student>> GetAllAsync();
    Task<Student?> GetByIdAsync(int id);
    Task<Student?> GetByIdForUpdateAsync(int id);
    Task<Student?> GetByUserIdAsync(int userId);
    Task<bool> ExistsAsync(int id);
    Task<bool> UserLinkedToStudentAsync(int userId, int? excludeStudentId = null);
    Task AddAsync(Student student);
    Task SaveChangesAsync();
}
