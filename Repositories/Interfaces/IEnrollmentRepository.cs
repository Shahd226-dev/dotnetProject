public interface IEnrollmentRepository
{
    Task<bool> EnrollmentExistsAsync(int studentId, int courseId);
    Task AddAsync(Enrollment enrollment);
    Task SaveChangesAsync();
}
