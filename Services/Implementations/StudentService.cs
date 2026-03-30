using Microsoft.EntityFrameworkCore;

public class StudentService : IStudentService
{
    private readonly AppDbContext _context;

    public StudentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<StudentResponseDto>> GetAllAsync()
    {
        return await _context.Students
            .AsNoTracking()
            .Select(s => new StudentResponseDto
            {
                Id = s.Id,
                Name = s.Name
            })
            .ToListAsync();
    }

    public async Task<StudentResponseDto> CreateAsync(CreateStudentDto dto)
    { 
        var student = new Student
        {
            Name = dto.Name,
            Email = dto.Email
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        return new StudentResponseDto
        {
            Id = student.Id,
            Name = student.Name
        };
    }

    public async Task<StudentResponseDto?> GetByIdAsync(int id)
    {
        return await _context.Students
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new StudentResponseDto
            {
                Id = s.Id,
                Name = s.Name
            })
            .FirstOrDefaultAsync();
    }

    public async Task<StudentResponseDto?> UpdateAsync(int id, UpdateStudentDto dto)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null) return null;

        student.Name = dto.Name;
        await _context.SaveChangesAsync();

        return new StudentResponseDto
        {
            Id = student.Id,
            Name = student.Name
        };
    }
}