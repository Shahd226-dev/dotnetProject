public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ICourseRepository _courseRepository;

    public EnrollmentService(
        IEnrollmentRepository enrollmentRepository,
        IStudentRepository studentRepository,
        ICourseRepository courseRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _studentRepository = studentRepository;
        _courseRepository = courseRepository;
    }

    public async Task EnrollAsync(EnrollStudentDto dto)
    {
        var studentExists = await _studentRepository.ExistsAsync(dto.StudentId);

        if (!studentExists)
            throw new InvalidOperationException("Student not found.");

        var courseExists = await _courseRepository.ExistsAsync(dto.CourseId);

        if (!courseExists)
            throw new InvalidOperationException("Course not found.");

        var alreadyEnrolled = await _enrollmentRepository.EnrollmentExistsAsync(dto.StudentId, dto.CourseId);

        if (alreadyEnrolled)
            throw new InvalidOperationException("Student is already enrolled in this course.");

        var enrollment = new Enrollment
        {
            StudentId = dto.StudentId,
            CourseId = dto.CourseId
        };

        await _enrollmentRepository.AddAsync(enrollment);
        await _enrollmentRepository.SaveChangesAsync();
    }
}
