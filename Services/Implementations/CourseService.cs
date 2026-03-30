using Microsoft.EntityFrameworkCore;

public class CourseService : ICourseService
{
    private readonly AppDbContext _context;

    public CourseService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CourseResponseDto>> GetAllAsync()
    {
        return await _context.Courses
            .AsNoTracking()
            .Select(c => new CourseResponseDto
            {
                Id = c.Id,
                Title = c.Title
            })
            .ToListAsync();
    }

    public async Task<CourseResponseDto?> GetByIdAsync(int id)
    {
        var course = await _context.Courses
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CourseResponseDto
            {
                Id = c.Id,
                Title = c.Title
            })
            .FirstOrDefaultAsync();

        return course;
    }

    public async Task<CourseResponseDto> CreateAsync(CreateCourseDto dto)
    {
        var course = new Course
        {
            Title = dto.Title,
            InstructorId = dto.InstructorId
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        return new CourseResponseDto { Id = course.Id, Title = course.Title };
    }

    public async Task<CourseResponseDto?> UpdateAsync(int id, UpdateCourseDto dto)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null)
            return null;

        course.Title = dto.Title;
        course.InstructorId = dto.InstructorId;

        await _context.SaveChangesAsync();

        return new CourseResponseDto { Id = course.Id, Title = course.Title };
    }
}