public interface IEnrollmentService
{
    Task<EnrollmentDto> EnrollAsync(EnrollmentDto dto);
    Task<bool> UnenrollAsync(int courseId);
    Task<List<EnrollmentDto>> GetMyEnrollmentsAsync();
    Task<List<EnrollmentDto>> GetEnrollmentsForInstructorAsync();
}