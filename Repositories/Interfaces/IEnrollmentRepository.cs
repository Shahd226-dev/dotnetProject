public interface IEnrollmentRepository
{
    Task<bool> EnrollmentExistsAsync(int studentId, int courseId);
    Task<Enrollment?> GetByKeyAsync(int studentId, int courseId);
    Task<List<Enrollment>> GetByStudentIdAsync(int studentId);
    Task<List<Enrollment>> GetByInstructorIdAsync(int instructorId);
    Task AddAsync(Enrollment enrollment);
    Task DeleteAsync(Enrollment enrollment);
    Task SaveChangesAsync();
}
