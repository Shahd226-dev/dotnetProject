public interface IEnrollmentService
{
    Task<EnrollmentResponseDto> EnrollAsync(CreateEnrollmentDto dto);
    Task<bool> UnenrollAsync(int courseId);
    Task<List<EnrollmentResponseDto>> GetMyEnrollmentsAsync();
    Task<List<EnrollmentResponseDto>> GetEnrollmentsForInstructorAsync();
}