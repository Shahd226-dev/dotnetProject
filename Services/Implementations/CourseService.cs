public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IInstructorRepository _instructorRepository;

    public CourseService(
        ICourseRepository courseRepository,
        IInstructorRepository instructorRepository)
    {
        _courseRepository = courseRepository;
        _instructorRepository = instructorRepository;
    }

    public async Task<List<CourseResponseDto>> GetAllAsync()
    {
        var courses = await _courseRepository.GetAllAsync();

        return courses
            .Select(c => new CourseResponseDto
            {
                Id = c.Id,
                Title = c.Title,
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
            InstructorId = course.InstructorId
        };
    }

    public async Task<CourseResponseDto> CreateAsync(CreateCourseDto dto)
    {
        var instructorExists = await _instructorRepository.ExistsAsync(dto.InstructorId);
        if (!instructorExists)
            throw new InvalidOperationException("Instructor not found.");

        var course = new Course
        {
            Title = dto.Title,
            InstructorId = dto.InstructorId
        };

        await _courseRepository.AddAsync(course);
        await _courseRepository.SaveChangesAsync();

        return new CourseResponseDto
        {
            Id = course.Id,
            Title = course.Title,
            InstructorId = course.InstructorId
        };
    }

    public async Task<CourseResponseDto?> UpdateAsync(int id, UpdateCourseDto dto)
    {
        var course = await _courseRepository.GetByIdForUpdateAsync(id);
        if (course == null)
            return null;

        var instructorExists = await _instructorRepository.ExistsAsync(dto.InstructorId);
        if (!instructorExists)
            throw new InvalidOperationException("Instructor not found.");

        course.Title = dto.Title;
        course.InstructorId = dto.InstructorId;

        await _courseRepository.SaveChangesAsync();

        return new CourseResponseDto
        {
            Id = course.Id,
            Title = course.Title,
            InstructorId = course.InstructorId
        };
    }
}
