using System.Security.Claims;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IInstructorRepository _instructorRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CourseService(
        ICourseRepository courseRepository,
        IInstructorRepository instructorRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _courseRepository = courseRepository;
        _instructorRepository = instructorRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<CourseResponseDto>> GetAllAsync()
    {
        var courses = await _courseRepository.GetAllAsync();

        return courses
            .Select(c => new CourseResponseDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                InstructorId = c.InstructorId
            })
            .ToList();
    }

    public async Task<CourseResponseDto?> GetByIdAsync(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id);

        if (course == null)
            return null;

        return new CourseResponseDto
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            InstructorId = course.InstructorId
        };
    }

    public async Task<CourseResponseDto> CreateAsync(CourseCreateDto dto)
    {
        var instructorId = await ResolveInstructorIdAsync(dto.InstructorId);

        var course = new Course
        {
            Title = dto.Title.Trim(),
            Description = dto.Description.Trim(),
            InstructorId = instructorId
        };

        await _courseRepository.AddAsync(course);
        await _courseRepository.SaveChangesAsync();

        return new CourseResponseDto
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            InstructorId = course.InstructorId
        };
    }

    public async Task<CourseResponseDto?> UpdateAsync(int id, CourseUpdateDto dto)
    {
        var course = await _courseRepository.GetByIdForUpdateAsync(id);
        if (course == null)
            return null;

        var (userId, role) = GetCurrentUser();
        if (string.Equals(role, RoleConstants.Instructor, StringComparison.OrdinalIgnoreCase))
        {
            var instructor = await _instructorRepository.GetByUserIdAsync(userId);
            if (instructor == null)
                throw new InvalidOperationException("Instructor not found.");

            if (course.InstructorId != instructor.Id)
                throw new UnauthorizedAccessException("You can only update your own courses.");

            course.Title = dto.Title.Trim();
            course.Description = dto.Description.Trim();
        }
        else if (string.Equals(role, RoleConstants.Admin, StringComparison.OrdinalIgnoreCase))
        {
            var instructorId = await ResolveInstructorIdAsync(dto.InstructorId);
            course.Title = dto.Title.Trim();
            course.Description = dto.Description.Trim();
            course.InstructorId = instructorId;
        }
        else
        {
            throw new UnauthorizedAccessException("Unauthorized.");
        }

        await _courseRepository.SaveChangesAsync();

        return new CourseResponseDto
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            InstructorId = course.InstructorId
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var course = await _courseRepository.GetByIdForUpdateAsync(id);
        if (course == null)
            return false;

        var (userId, role) = GetCurrentUser();
        if (string.Equals(role, RoleConstants.Instructor, StringComparison.OrdinalIgnoreCase))
        {
            var instructor = await _instructorRepository.GetByUserIdAsync(userId);
            if (instructor == null)
                throw new InvalidOperationException("Instructor not found.");

            if (course.InstructorId != instructor.Id)
                throw new UnauthorizedAccessException("You can only delete your own courses.");
        }
        else if (!string.Equals(role, RoleConstants.Admin, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("Unauthorized.");
        }

        await _courseRepository.DeleteAsync(course);
        await _courseRepository.SaveChangesAsync();
        return true;
    }

    private async Task<int> ResolveInstructorIdAsync(int? instructorId)
    {
        var (userId, role) = GetCurrentUser();

        if (string.Equals(role, RoleConstants.Instructor, StringComparison.OrdinalIgnoreCase))
        {
            var instructor = await _instructorRepository.GetByUserIdAsync(userId);
            if (instructor == null)
                throw new InvalidOperationException("Instructor not found.");

            return instructor.Id;
        }

        if (!string.Equals(role, RoleConstants.Admin, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("Unauthorized.");

        if (!instructorId.HasValue)
            throw new InvalidOperationException("InstructorId is required for admin operations.");

        var instructorExists = await _instructorRepository.ExistsAsync(instructorId.Value);
        if (!instructorExists)
            throw new InvalidOperationException("Instructor not found.");

        return instructorId.Value;
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
