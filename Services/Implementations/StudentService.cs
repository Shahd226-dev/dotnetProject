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
                Name = s.Name,
                UserId = s.UserId
            })
            .ToListAsync();
    }

    public async Task<StudentResponseDto> CreateAsync(CreateStudentDto dto)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == dto.UserId);

        if (user == null)
            throw new InvalidOperationException("User not found.");

        if (!string.Equals(user.Role, RoleConstants.User, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Student must be linked to a user with role 'User'.");

        var linkedStudentExists = await _context.Students
            .AsNoTracking()
            .AnyAsync(s => s.UserId == dto.UserId);

        if (linkedStudentExists)
            throw new InvalidOperationException("This user is already linked to another student.");

        var linkedInstructorExists = await _context.Instructors
            .AsNoTracking()
            .AnyAsync(i => i.UserId == dto.UserId);

        if (linkedInstructorExists)
            throw new InvalidOperationException("This user is already linked to an instructor.");

        var student = new Student
        {
            Name = dto.Name,
            Email = dto.Email,
            UserId = dto.UserId
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        return new StudentResponseDto
        {
            Id = student.Id,
            Name = student.Name,
            UserId = student.UserId
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
                Name = s.Name,
                UserId = s.UserId
            })
            .FirstOrDefaultAsync();
    }

    public async Task<StudentResponseDto?> UpdateAsync(int id, UpdateStudentDto dto)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null) return null;

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == dto.UserId);

        if (user == null)
            throw new InvalidOperationException("User not found.");

        if (!string.Equals(user.Role, RoleConstants.User, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Student must be linked to a user with role 'User'.");

        var linkedStudentExists = await _context.Students
            .AsNoTracking()
            .AnyAsync(s => s.UserId == dto.UserId && s.Id != id);

        if (linkedStudentExists)
            throw new InvalidOperationException("This user is already linked to another student.");

        var linkedInstructorExists = await _context.Instructors
            .AsNoTracking()
            .AnyAsync(i => i.UserId == dto.UserId);

        if (linkedInstructorExists)
            throw new InvalidOperationException("This user is already linked to an instructor.");

        student.Name = dto.Name;
        student.UserId = dto.UserId;
        await _context.SaveChangesAsync();

        return new StudentResponseDto
        {
            Id = student.Id,
            Name = student.Name,
            UserId = student.UserId
        };
    }
}