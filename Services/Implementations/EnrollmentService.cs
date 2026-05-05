using System.Security.Claims;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IInstructorRepository _instructorRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EnrollmentService(
        IEnrollmentRepository enrollmentRepository,
        IStudentRepository studentRepository,
        ICourseRepository courseRepository,
        IInstructorRepository instructorRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _enrollmentRepository = enrollmentRepository;
        _studentRepository = studentRepository;
        _courseRepository = courseRepository;
        _instructorRepository = instructorRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<EnrollmentDto> EnrollAsync(EnrollmentDto dto)
    {
        var (userId, role) = GetCurrentUser();
        if (!string.Equals(role, RoleConstants.Student, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("Only students can enroll.");

        var student = await _studentRepository.GetByUserIdAsync(userId);
        if (student == null)
            throw new InvalidOperationException("Student not found.");

        var courseExists = await _courseRepository.ExistsAsync(dto.CourseId);
        if (!courseExists)
            throw new InvalidOperationException("Course not found.");

        var alreadyEnrolled = await _enrollmentRepository.EnrollmentExistsAsync(student.Id, dto.CourseId);
        if (alreadyEnrolled)
            throw new ConflictException("Student is already enrolled in this course.");

        var enrollment = new Enrollment
        {
            StudentId = student.Id,
            CourseId = dto.CourseId,
            EnrolledAt = DateTime.UtcNow
        };

        await _enrollmentRepository.AddAsync(enrollment);
        await _enrollmentRepository.SaveChangesAsync();

        return new EnrollmentDto
        {
            StudentId = enrollment.StudentId,
            CourseId = enrollment.CourseId,
            EnrolledAt = enrollment.EnrolledAt
        };
    }

    public async Task<bool> UnenrollAsync(int courseId)
    {
        var (userId, role) = GetCurrentUser();
        if (!string.Equals(role, RoleConstants.Student, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("Only students can unenroll.");

        var student = await _studentRepository.GetByUserIdAsync(userId);
        if (student == null)
            throw new InvalidOperationException("Student not found.");

        var enrollment = await _enrollmentRepository.GetByKeyAsync(student.Id, courseId);
        if (enrollment == null)
            return false;

        await _enrollmentRepository.DeleteAsync(enrollment);
        await _enrollmentRepository.SaveChangesAsync();
        return true;
    }

    public async Task<List<EnrollmentDto>> GetMyEnrollmentsAsync()
    {
        var (userId, role) = GetCurrentUser();
        if (!string.Equals(role, RoleConstants.Student, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("Only students can view their enrollments.");

        var student = await _studentRepository.GetByUserIdAsync(userId);
        if (student == null)
            throw new InvalidOperationException("Student not found.");

        var enrollments = await _enrollmentRepository.GetByStudentIdAsync(student.Id);
        return enrollments
            .Select(e => new EnrollmentDto
            {
                StudentId = e.StudentId,
                CourseId = e.CourseId,
                EnrolledAt = e.EnrolledAt
            })
            .ToList();
    }

    public async Task<List<EnrollmentDto>> GetEnrollmentsForInstructorAsync()
    {
        var (userId, role) = GetCurrentUser();
        if (!string.Equals(role, RoleConstants.Instructor, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("Only instructors can view course enrollments.");

        var instructor = await _instructorRepository.GetByUserIdAsync(userId);
        if (instructor == null)
            throw new InvalidOperationException("Instructor not found.");

        var enrollments = await _enrollmentRepository.GetByInstructorIdAsync(instructor.Id);
        return enrollments
            .Select(e => new EnrollmentDto
            {
                StudentId = e.StudentId,
                CourseId = e.CourseId,
                EnrolledAt = e.EnrolledAt
            })
            .ToList();
    }

    private (int userId, string role) GetCurrentUser()
    {
        var userIdRaw = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrWhiteSpace(userIdRaw) || !int.TryParse(userIdRaw, out var userId) || string.IsNullOrWhiteSpace(role))
            throw new UnauthorizedAccessException("Unauthorized.");

        return (userId, role);
    }
}
